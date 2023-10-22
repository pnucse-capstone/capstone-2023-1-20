/*
SimpleSpectrum.cs - Part of Simple Spectrum V2.1 by Sam Boyer.
*/

#if !UNITY_WEBGL
#define MICROPHONE_AVAILABLE
#endif

#if UNITY_WEBGL && !UNITY_EDITOR 
#define WEB_MODE //different to UNITY_WEBGL, as we still want functionality in the Editor!
#endif

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;
public class CustomSpectrum: MonoBehaviour {

    public enum SourceType
    {
        AudioSource, AudioListener, MicrophoneInput, StereoMix, Custom
    }

    [SerializeField]
    public AudioMixerGroup muteGroup; //the AudioMixerGroup used for silent tracks (microphones). Don't change.

    /// <summary>
    /// Enables or disables the processing and display of spectrum data. 
    /// </summary>
    [Tooltip("Enables or disables the processing and display of spectrum data. ")]
    public bool isEnabled = true;

#region SAMPLING PROPERTIES

    /// <summary>
    /// The type of source for spectrum data.
    /// </summary>
    [Tooltip("The type of source for spectrum data.")]
    public SourceType sourceType = SourceType.AudioSource;

    /// <summary>
    /// The AudioSource to take data from. Can be empty if sourceType is not AudioSource.
    /// </summary>
    [Tooltip("The AudioSource to take data from.")]
    public AudioSource audioSource;

    /// <summary>
    /// The audio channel to use when sampling.
    /// </summary>
    [Tooltip("The audio channel to use when sampling.")]
    public int sampleChannel = 0;
    /// <summary>
    /// The number of samples to use when sampling. Must be a power of two.
    /// </summary>
    [Tooltip("The number of samples to use when sampling. Must be a power of two.")]
    public int numSamples = 256;
    /// <summary>
    /// The FFTWindow to use when sampling.
    /// </summary>
    [Tooltip("The FFTWindow to use when sampling.")]
    public FFTWindow windowUsed = FFTWindow.BlackmanHarris;
    /// <summary>
    /// If true, audio data is scaled logarithmically.
    /// </summary>
    [Tooltip("If true, audio data is scaled logarithmically.")]
    public bool useLogarithmicFrequency = true;
    /// <summary>
    /// If true, the values of the spectrum are multiplied based on their frequency, to keep the values proportionate.
    /// </summary>
    [Tooltip("If true, the values of the spectrum are multiplied based on their frequency, to keep the values proportionate.")]
    public bool multiplyByFrequency = true;

    /// <summary>
    /// The lower bound of the freuqnecy range to sample from. Leave at 0 when unused.
    /// </summary>
    [Tooltip("The lower bound of the freuqnecy range to sample from. Leave at 0 when unused.")]
    public float frequencyLimitLow = 0;

    /// <summary>
    /// The uppwe bound of the freuqnecy range to sample from. Leave at 22050 when unused.
    /// </summary>
    [Tooltip("The upper bound of the freuqnecy range to sample from. Leave at 22050 (44100/2) when unused.")]
    public float frequencyLimitHigh = 22050;

    /*
    /// <summary>
    /// Determines what percentage of the full frequency range to use (1 being the full range, reducing the value towards 0 cuts off high frequencies).
    /// This can be useful when using MP3 files or audio with missing high frequencies.
    /// </summary>
    [Range(0, 1)]
    [Tooltip("Determines what percentage of the full frequency range to use (1 being the full range, reducing the value towards 0 cuts off high frequencies).\nThis can be useful when using MP3 files or audio with missing high frequencies.")]
    public float highFrequencyTrim = 1;
    /// <summary>
    /// When useLogarithmicFrequency is false, this value stretches the spectrum data onto the bars.
    /// </summary>
    [Tooltip("Stretches the spectrum data when mapping onto the bars. A lower value means the spectrum is populated by lower frequencies.")]
    public float linearSampleStretch = 1;
    */
#endregion

#region BAR PROPERTIES
    /// <summary>
    /// The amount of bars to use.
    /// </summary>
    [Tooltip("The amount of bars to use. Does not have to be equal to Num Samples, but probably should be lower.")]
    public int barAmount = 128;
    /// <summary>
    /// Stretches the values of the bars.
    /// </summary>
    float barYScale = 1;
    /// <summary>
    /// Sets a minimum scale for the bars; they will never go below this scale.
    /// This value is also used when isEnabled is false.
    /// </summary>
    [Tooltip("Sets a minimum scale for the bars.")]
    public float barMinYScale = 0.1f;
    /// <summary>
    /// The prefab of bar to use when building.
    /// Refer to the documentation to use a custom prefab.
    /// </summary>
    [Range(0, 1)]
    [Tooltip("The amount of dampening used when the new scale is higher than the bar's existing scale.")]
    public float attackDamp = 0.3f;
    /// <summary>
    /// The amount of dampening used when the new scale is lower than the bar's existing scale. Must be between 0 (slowest) and 1 (fastest).
    /// </summary>
	[Range(0, 1)]
    [Tooltip("The amount of dampening used when the new scale is lower than the bar's existing scale.")]
    public float decayDamp = 0.15f;
#endregion

#region COLOR PROPERTIES
    /// <summary>
    /// Determines whether to apply a color gradient on the bars, or just use colorMin as a solid color.
    /// </summary>
    [Tooltip("Determines whether to apply a color gradient on the bars, or just use a solid color.")]
    public bool useColorGradient = false;
    /// <summary>
    /// The minimum (low value) color if useColorGradient is true, else the solid color to use.
    /// </summary>
    public Color color = Color.white;
    /// <summary>
    /// The curve that determines the interpolation between colorMin and colorMax.
    /// </summary>
    [Tooltip("The curve that determines the interpolation between colorMin and colorMax.")]
    public AnimationCurve colorValueCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0,0), new Keyframe(1,1)});
    /// <summary>
    /// The amount of dampening used when the new color value is higher than the existing color value. Must be between 0 (slowest) and 1 (fastest).
    /// </summary>
    [Range(0, 1)]
    [Tooltip("The amount of dampening used when the new color value is higher than the existing color value.")]
    public float colorAttackDamp = 1;
    /// <summary>
    /// The amount of dampening used when the new color value is lower than the existing color value. Must be between 0 (slowest) and 1 (fastest).
    /// </summary>
    [Range(0, 1)]
    [Tooltip("The amount of dampening used when the new color value is lower than the existing color value.")]
    public float colorDecayDamp = 1;
#endregion

    /// <summary>
    /// The raw audio spectrum data. Can be set to custom values if the sourceType is set to Custom.
    /// (For a 1:1 data to bar mapping, set barAmount equal to numSamples, disable useLogarithmicFrequency and set linearSampleStretch to 1)
    /// </summary>
    public float[] spectrumInputData
    {
        get
        {
            return spectrum;
        }
        set
        {
            if (sourceType == SourceType.Custom)
                spectrum = value;
            else
                Debug.LogError("Error from SimpleSpectrum: spectrumInputData cannot be set while sourceType is not Custom.");
        }
    }

    /// <summary>
    /// Returns the output float array used for bar scaling (i.e. after logarithmic scaling and attack/decay). The size of the array depends on barAmount.
    /// </summary>
    public float[] spectrumOutputData
    {
        get
        {
            return oldYScales;
        }
    }


    float[] spectrum; 

    //float lograithmicAmplitudePower = 2, multiplyByFrequencyPower = 1.5f;
//    float [] values;
    Material[] barMaterials; //optimisation
    float[] oldYScales; //also optimisation
    float[] oldColorValues; //...optimisation
    int materialValId;

    bool materialColourCanBeUsed = true; //can dynamic material colouring be used?

    float highestLogFreq, frequencyScaleFactor; //multiplier to ensure that the frequencies stretch to the highest record in the array.

    string microphoneName;
    float lastMicRestartTime;
    float micRestartWait = 20;
    public bool flip = false;
    Material shader;
    void Start () {
        gameObject.SetActive(PlayerPrefs.GetInt("Spectrum", 1) == 1);
        if(audioSource==null && sourceType == SourceType.AudioSource)
            Debug.LogError("An audio source has not been assigned. Please assign a reference to a source, or set useAudioListener instead.");

        shader=gameObject.GetComponent<Renderer>().material;
        shader.SetInt("_BarAmount", barAmount);
        Color tempColor = Color.HSVToRGB(0.5F, (float)Math.Sin(Game.time), 1F);
        tempColor.a = 0.5F;
        shader.SetColor("_Color",tempColor);
        RebuildSpectrum();
	}

    /// <summary>
    /// Rebuilds this instance of Spectrum, applying any changes.
    /// </summary>
    public void RebuildSpectrum()
    {
        isEnabled = false;	//just in case

        //clear all the existing children
        int childs = transform.childCount;
        for (int i = 0; i < childs; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        RestartMicrophone();

        numSamples = Mathf.ClosestPowerOfTwo(numSamples);

#if WEB_MODE
        numSamples = SSWebInteract.SetFFTSize(numSamples);
#endif

        //initialise arrays
        spectrum = new float[numSamples];
        barMaterials = new Material[barAmount];
        oldYScales = new float[barAmount];
        oldColorValues = new float[barAmount];

        materialColourCanBeUsed = true;


        //spectrum bending calculations


        materialValId = Shader.PropertyToID("_Val");

        highestLogFreq = Mathf.Log(barAmount + 1, 2); //gets the highest possible logged frequency, used to calculate which sample of the spectrum to use for a bar
        frequencyScaleFactor = 1.0f/(AudioSettings.outputSampleRate /2)  * numSamples;


        isEnabled = true;
    }

    /// <summary>
    /// Restarts the Microphone recording.
    /// </summary>
    public void RestartMicrophone()
    {
#if MICROPHONE_AVAILABLE
        Microphone.End(microphoneName);

        //set up microphone input source if required
        if (sourceType == SourceType.MicrophoneInput || sourceType == SourceType.StereoMix)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

            if (Microphone.devices.Length == 0)
            {
                Debug.LogError("Error from SimpleSpectrum: Microphone or Stereo Mix is being used, but no Microphones are found!");
            }

            microphoneName = null; //if type is Microphone, the default microphone will be used. If StereoMix, 'Stereo Mix' will be searched for in the list.


            if (sourceType == SourceType.StereoMix) //find stereo mix
            {
                foreach (string name in Microphone.devices)
                    if (name.StartsWith("Stereo Mix")) //since the returned names have driver details in brackets afterwards
                        microphoneName = name;
                if(microphoneName==null)
                    Debug.LogError("Error from SimpleSpectrum: Stereo Mix not found. Reverting to default microphone.");
            }
            audioSource.loop = true;
            audioSource.outputAudioMixerGroup = muteGroup;

            AudioClip clip1 = audioSource.clip = Microphone.Start(microphoneName, true, 5, 44100);
            audioSource.clip = clip1;

            while (!(Microphone.GetPosition(microphoneName) - 0 > 0)) { }
            audioSource.Play();
            lastMicRestartTime = Time.unscaledTime;
            //print("restarted mic");
        }
        else
        {
            Destroy(GetComponent<AudioSource>());
        }
#else
        if (sourceType == SourceType.MicrophoneInput || sourceType == SourceType.StereoMix || sourceType == SourceType.AudioSource)
        {
            Debug.LogError("Error from SimpleSpectrum: Microphone, Stereo Mix or AudioSource cannot be used in WebGL!");
        }
#endif
    }


    void Update () {
		if (isEnabled) {

            if (sourceType != SourceType.Custom)
            {
                if (sourceType == SourceType.AudioListener)
                {
                    AudioListener.GetSpectrumData(spectrum, sampleChannel, windowUsed); 
                }
                else
                {
                    audioSource.GetSpectrumData(spectrum, sampleChannel, windowUsed); //get the spectrum data
                }
            }

#if UNITY_EDITOR    //allows for editing curve while in play mode, disabled in build for optimisation


                float curveAngleRads = 0, curveRadius = 0, halfwayAngleR = 0, halfwayAngleD = 0;
                Vector3 curveCentreVector = Vector3.zero;
                
#endif
#if WEB_MODE
            float freqLim = frequencyLimitHigh * 0.76f; //AnalyserNode.getFloatFrequencyData doesn't fill the array, for some reason
#else          
            float freqLim = frequencyLimitHigh;
#endif
            for (int i = 0; i < barAmount; i++) 
            {
                {

                    float value;
                    float trueSampleIndex;

                    trueSampleIndex = Mathf.Lerp(frequencyLimitLow, freqLim, (highestLogFreq - Mathf.Log(barAmount + 1 - i, 2)) / highestLogFreq) * frequencyScaleFactor;


                    //the true sample is usually a decimal, so we need to lerp between the floor and ceiling of it.

                    int sampleIndexFloor = Mathf.FloorToInt(trueSampleIndex);
                    sampleIndexFloor = Mathf.Clamp(sampleIndexFloor, 0, spectrum.Length - 2); //just keeping it within the spectrum array's range

                    value = Mathf.SmoothStep(spectrum[sampleIndexFloor], spectrum[sampleIndexFloor + 1], trueSampleIndex - sampleIndexFloor); //smoothly interpolate between the two samples using the true index's decimal.

                    //MANIPULATE & APPLY SAMPLES
                    if (multiplyByFrequency) //multiplies the amplitude by the true sample index
                    {
#if WEB_MODE
                    value = value * (Mathf.Log(trueSampleIndex + 1) + 1);  //different due to how the WebAudioAPI outputs spectrum data.

#else
                        value = value * (trueSampleIndex + 1);
#endif
                    }

#if !WEB_MODE
                    value = Mathf.Sqrt(value); //compress the amplitude values by sqrt(x)
#endif

                    //DAMPENING
                    //Vector3 oldScale = bar.localScale;
                    float oldYScale = oldYScales[i], newYScale;
                    if (value * barYScale > oldYScale)
                    {
                        newYScale = Mathf.Lerp(oldYScale, Mathf.Max(value * barYScale, barMinYScale), attackDamp);
                    }
                    else
                    {
                        newYScale = Mathf.Lerp(oldYScale, Mathf.Max(value * barYScale, barMinYScale), decayDamp);
                    }
//                    Color tempColor = Color.HSVToRGB(0.5F, (float)Math.Sin(Game.time), 1F);
//                    tempColor.a = Math.Min(newYScale, 0.1F);
                    oldYScales[i] = newYScale;
                   
                }
            }
            shader.SetFloatArray("_Value", oldYScales);

        }
        if ((Time.unscaledTime - lastMicRestartTime)>micRestartWait)
            RestartMicrophone();
	}

    /// <summary>
    /// Returns a logarithmically scaled and proportionate array of spectrum data from the AudioSource. Doesn't work in WebGL.
    /// </summary>
    /// <param name="source">The AudioSource to take data from.</param>
    /// <param name="spectrumSize">The size of the returned array.</param>
    /// <param name="sampleSize">The size of sample to take from the AudioSource. Must be a power of two.</param>
    /// <param name="windowUsed">The FFTWindow to use when sampling.</param>
    /// <param name="channelUsed">The audio channel to use when sampling.</param>
    /// <returns>A logarithmically scaled and proportionate array of spectrum data from the AudioSource.</returns>
    public static float[] GetLogarithmicSpectrumData(AudioSource source, int spectrumSize, int sampleSize, FFTWindow windowUsed = FFTWindow.BlackmanHarris, int channelUsed = 0)
    {
#if UNITY_WEBGL
        Debug.LogError("Error from SimpleSpectrum: Spectrum data cannot be retrieved from a single AudioSource in WebGL!");
        return null;
#endif
        float[] spectrum = new float[spectrumSize];

        channelUsed = Mathf.Clamp(channelUsed, 0, 1);

        float[] samples = new float[Mathf.ClosestPowerOfTwo(sampleSize)];

        source.GetSpectrumData(samples, channelUsed, windowUsed);

        float highestLogSampleFreq = Mathf.Log(spectrum.Length + 1, 2); //gets the highest possible logged frequency, used to calculate which sample of the spectrum to use for a bar

        float logSampleFreqMultiplier = sampleSize / highestLogSampleFreq;

        for (int i = 0; i < spectrum.Length; i++) //for each float in the output
        {

            float trueSampleIndex = (highestLogSampleFreq - Mathf.Log(spectrum.Length + 1 - i, 2)) * logSampleFreqMultiplier; //gets the index equiv of the logified frequency

            //the true sample is usually a decimal, so we need to lerp between the floor and ceiling of it.

            int sampleIndexFloor = Mathf.FloorToInt(trueSampleIndex);
            sampleIndexFloor = Mathf.Clamp(sampleIndexFloor, 0, samples.Length - 2); //just keeping it within the spectrum array's range

            float value = Mathf.SmoothStep(spectrum[sampleIndexFloor], spectrum[sampleIndexFloor + 1], trueSampleIndex - sampleIndexFloor); //smoothly interpolate between the two samples using the true index's decimal.

            value = value * trueSampleIndex; //multiply value by its position to make it proportionate;

            value = Mathf.Sqrt(value); //compress the amplitude values by sqrt(x)

            spectrum[i] = value;
        }
        return spectrum;
    }

    /// <summary>
    /// Returns a logarithmically scaled and proportionate array of spectrum data from the AudioListener.
    /// </summary>
    /// <param name="spectrumSize">The size of the returned array.</param>
    /// <param name="sampleSize">The size of sample to take from the AudioListener. Must be a power of two. Will only be used in WebGL if no samples have been taken yet.</param>
    /// <param name="windowUsed">The FFTWindow to use when sampling. Unused in WebGL.</param>
    /// <param name="channelUsed">The audio channel to use when sampling. Unused in WebGL.</param>
    /// <returns>A logarithmically scaled and proportionate array of spectrum data from the AudioListener.</returns>
    public static float[] GetLogarithmicSpectrumData(int spectrumSize, int sampleSize, FFTWindow windowUsed = FFTWindow.BlackmanHarris, int channelUsed = 0)
    {
#if WEB_MODE
        sampleSize = SSWebInteract.SetFFTSize(sampleSize); //set the WebGL sampleSize if not already done, otherwise get the current sample size.
#endif
        float[] spectrum = new float[spectrumSize];

        channelUsed = Mathf.Clamp(channelUsed, 0, 1);

        float[] samples = new float[Mathf.ClosestPowerOfTwo(sampleSize)];

#if WEB_MODE
        SSWebInteract.GetSpectrumData(samples); //get the spectrum data from the JS lib
#else
        AudioListener.GetSpectrumData(samples, channelUsed, windowUsed);
#endif

        float highestLogSampleFreq = Mathf.Log(spectrum.Length + 1, 2); //gets the highest possible logged frequency, used to calculate which sample of the spectrum to use for a bar

        float logSampleFreqMultiplier = sampleSize / highestLogSampleFreq;

        for (int i = 0; i < spectrum.Length; i++) //for each float in the output
        {

            float trueSampleIndex = (highestLogSampleFreq - Mathf.Log(spectrum.Length + 1 - i, 2)) * logSampleFreqMultiplier; //gets the index equiv of the logified frequency

            //the true sample is usually a decimal, so we need to lerp between the floor and ceiling of it.

            int sampleIndexFloor = Mathf.FloorToInt(trueSampleIndex);
            sampleIndexFloor = Mathf.Clamp(sampleIndexFloor, 0, samples.Length - 2); //just keeping it within the spectrum array's range

            float value = Mathf.SmoothStep(spectrum[sampleIndexFloor], spectrum[sampleIndexFloor + 1], trueSampleIndex - sampleIndexFloor); //smoothly interpolate between the two samples using the true index's decimal.

#if WEB_MODE
            value = value * (Mathf.Log(trueSampleIndex + 1) + 1); //different due to how the WebAudioAPI outputs spectrum data.

#else
            value = value * (trueSampleIndex + 1); //multiply value by its position to make it proportionate
            value = Mathf.Sqrt(value); //compress the amplitude values by sqrt(x)
#endif
            spectrum[i] = value;
        }
        return spectrum;
    }
}
