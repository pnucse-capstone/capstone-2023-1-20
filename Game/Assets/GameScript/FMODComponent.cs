using FMOD;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class FMODComponent : MonoBehaviour
{
    public static Mutex mutex = new Mutex();
    // Start is called before the first frame update
    void Start()
    {
        FMODWrapper.Init();
    }

    // Update is called once per frame
    void Update()
    {
        FMODWrapper.Update();
    }
    private void OnApplicationQuit()
    {
        FMODWrapper.Release();
    }
}
public static class MusicPlayer
{

    static Channel channel;
    static AudioData audio;
    public static float expandedlength => audio == null ? 0 : audio.length + Game.table.offset + Game.table.length_offset;
    public static float length => audio.length;
    public static bool isReady { get => _isReady; }
    public static bool loop
    {
        get
        {
            if (isReady)
            {
                MODE mask;
                channel.getMode(out mask);
                return (mask & MODE.LOOP_NORMAL) != 0;
            }
            else return false;
        }
        set 
        {
            if (isReady)
            {
                if (value)
                {
                    channel.setMode(MODE.LOOP_NORMAL);
                    channel.setLoopCount(-1);
                }
                else
                {
                    channel.setMode(MODE.LOOP_OFF);
                    channel.setLoopCount(0);
                }
            }
        }
    }
    public static bool isPlaying 
    { 
        get 
        {
            if (isReady)
            {
                bool value;
                channel.isPlaying(out value);
                return value;
            }
            else
            {
                return false;
            }
        } 
    }
    public static float time
    {
        get
        {
            return GetPosition();
        }
        set
        {
            SetPosition(value);
        }
    }

    public static float volume
    {
        get
        {
            if (isReady)
            {
                return _volume;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            if (isReady)
            {
                _volume = value;
                channel.setVolume(_volume);
            }
        }
    }
    static float _volume = 1F;

    private static float GetPosition()
    {
        if (isReady)
        {
            uint pos;
            channel.getPosition(out pos, TIMEUNIT.MS);
            return pos / 1000F;
        }
        else
        {
            return 0;
        }
    }
    static bool _isReady = false;
    public static AudioData GetAudioData()
    {
        return audio;
    }
    public static void SetMusic(AudioData audio)
    {
        UnityEngine.Debug.Log("Music Set");
        MusicPlayer.audio = audio;

        if (_isReady)
        {
            UnityEngine.Debug.Log("Music Stop");
            channel.stop();

            _isReady = false;
        }
        if (audio == null)
        {
            UnityEngine.Debug.Log("Music Empty");
            return;
        }
        _isReady = true;
        var snd = audio.GetSound();
        UnityEngine.Debug.Log("Music Set");
        channel = FMODWrapper.PlayMusic(snd);
        UnityEngine.Debug.Log("Music Set");
    }
    public static void SetPosition(float time)
    {
        UnityEngine.Debug.Log("Music Set Position");
        if (isReady)
        {
            channel.setPosition((uint)(time * 1000), TIMEUNIT.MS);
        }
    }
    public static void Reset()
    {
        UnityEngine.Debug.Log("Music Reset");

        if (isReady)
        {
            Mute(false);
            channel.removeFadePoints(0, ulong.MaxValue);
            channel.setVolume(1F);
        }
    }
    internal static void Init()
    {
        UnityEngine.Debug.Log("Music Init");
        if (_isReady)
        {
            audio = null;
            channel.stop();
        }
        _isReady = false;
    }

    internal static void Pause()
    {
        UnityEngine.Debug.Log("Music Pause");
        if (isReady)
        {
            channel.setPaused(true);
        }
    }
    internal static void Resume()
    {
        UnityEngine.Debug.Log("Music Resume");
        if (isReady)
        {
            var result = channel.setPaused(false);
            if (result != RESULT.OK)
            {
                UnityEngine.Debug.Log(result);
                channel = FMODWrapper.PlayMusic(audio.GetSound());
            }
        }
    }

    internal static void Mute(bool value)
    {
        if (isReady)
        {
            channel.setMute(value);
        }
    }

    internal static void FadeOut()
    {
        if (isReady)
        {
            ulong t;
            int rate;
            channel.getDSPClock(out _, out t);
            FMODWrapper.GetSystem().getSoftwareFormat(out rate, out _, out _);
            channel.addFadePoint(t, 1);
            channel.addFadePoint(t + (ulong)rate, 0F);
        }
        //        channel.setDelay(t, t + (ulong)rate, false);
    }
}
public static class FMODWrapper
{

    static FMOD.System sys;
    static ChannelGroup keySoundChannelgroup;
    static ChannelGroup musicChannelGroup;
    static ChannelGroup sfxChannelGroup;

    static DSP mFFT;
    const int WindowSize = 512;

    public static void InitSpectrum()
    {
        // FFT DSP 만듬
        if (sys.createDSPByType(FMOD.DSP_TYPE.FFT, out mFFT) == FMOD.RESULT.OK) //
        {
            //윈도우 타입, 윈도우 크기를 지정
            mFFT.setParameterInt((int)FMOD.DSP_FFT.WINDOWTYPE, (int)FMOD.DSP_FFT_WINDOW.HANNING);
            mFFT.setParameterInt((int)FMOD.DSP_FFT.WINDOWSIZE, WindowSize * 2);
            
            musicChannelGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, mFFT);
        }
        else
        {
            UnityEngine.Debug.LogError("Fail to Create DSP ");
        }
    }
    static float[] mFFTSpectrum;
    public static float[] GetSpectrum()
    {
        IntPtr ptr;
        uint length;
        mFFT.getParameterData((int)DSP_FFT.SPECTRUMDATA, out ptr, out length);
        DSP_PARAMETER_FFT fftData = (FMOD.DSP_PARAMETER_FFT)Marshal.PtrToStructure(ptr, typeof( DSP_PARAMETER_FFT ));
        if (fftData.numchannels > 0)
        {
            if (mFFTSpectrum == null)
            {
                // Allocate the fft spectrum buffer once
                for (int i = 0; i < fftData.numchannels; ++i)
                {
                    mFFTSpectrum = new float[fftData.length];
                }
            }
            fftData.getSpectrum(0, ref mFFTSpectrum);
            /*

            var pos = Vector3.zero;
            pos.x = WIDTH * -0.5f;

            for (int i = 0; i < WindowSize; ++i)
            {
                pos.x += (WIDTH / WindowSize);

                float level = lin2dB(mFFTSpectrum[i]);
                pos.y = (80 + level) * HEIGHT;

                mLineRenderer.SetPosition(i, pos);
            }
            */
        }
        return mFFTSpectrum;
    }


    static bool isReady = false;
    public static void SetSpeed(float speed)
    {
        musicChannelGroup.setPitch(speed);
    }
    static FMODWrapper()
    {
        Init();
    }
    public static void SetKeySoundVolume(float v)
    {

        keySoundChannelgroup.setVolume(Mathf.Clamp(v, 0, 1));
    }
    public static void SetSfxVolume(float v)
    {

        sfxChannelGroup.setVolume(Mathf.Clamp(v, 0, 1));
    }
    public static void SetMusicVolume(float v)
    {

        musicChannelGroup.setVolume(Mathf.Clamp(v, 0, 1));
    }
    public static void Refresh()
    {

        var master = PlayerPrefs.GetFloat("mastervolume", 0.5F);
        var keysoundVolume = PlayerPrefs.GetFloat("HitSoundVolume", 1F);
        var music_volume = PlayerPrefs.GetFloat("volume", 1F);
        var sfx_volume = PlayerPrefs.GetFloat("sfx", 1F);
        SetKeySoundVolume(keysoundVolume);
        SetSfxVolume(sfx_volume);
        SetMusicVolume(music_volume);
        SetMasterVolume(master);
        SetSpeed(Game.playSpeed);

    }
    public static void SetMasterVolume(float volume)
    {
        ChannelGroup cg;
        sys.getMasterChannelGroup(out cg);
        cg.setVolume(volume);
    }
    public static void Init()
    {
        if (!isReady)
        {
            UnityEngine.Debug.Log("hello");
            FMOD.Factory.System_Create(out sys);

            int value = PlayerPrefs.GetInt("dsp", 7);
            sys.setDSPBufferSize((uint)(1<<value), 4);
            sys.init(256, INITFLAGS.NORMAL, IntPtr.Zero);

            sys.createChannelGroup("ch", out keySoundChannelgroup);
            sys.createChannelGroup("music", out musicChannelGroup);
            sys.createChannelGroup("sfx", out sfxChannelGroup);

            InitSpectrum();
            InitNormalize();

            Refresh();

            isReady = true;
        }
    }

    static DSP normalize, limiter;
    private static void InitNormalize()
    {

        /*
        if (sys.createDSPByType(DSP_TYPE.NORMALIZE, out normalize) == RESULT.OK) //
        {
            int count;
            normalize.setParameterFloat((int)DSP_NORMALIZE.FADETIME, 0);
            normalize.setParameterFloat((int)DSP_NORMALIZE.THRESHOLD,  0.5F);
            normalize.setParameterFloat((int)DSP_NORMALIZE.MAXAMP, 1);

            keySoundChannelgroup.addDSP(CHANNELCONTROL_DSP_INDEX.TAIL, normalize);
            keySoundChannelgroup.getNumDSPs(out count);

        }
        else
        {
            UnityEngine.Debug.LogError("Fail to Create DSP ");
        }*/

    }

    public static void Update()
    {
        sys.update();
    }
    public static void Release()
    {
        if (isReady)
        {
            isReady = false;
            KeySoundPallete.Release();
            sys.release();

            musicChannelGroup.removeDSP(mFFT);
        }
    }
    public static FMOD.System GetSystem() // 싱글턴
    {
        if (isReady) return sys;
        else throw new Exception("FMOD System이 초기화되지 않았습니다.");
    }
    public static Channel PlayKeySound(FMOD.Sound sound)
    {
        Channel ch;
        sys.playSound(sound, keySoundChannelgroup, false, out ch);
        return ch;
    }
    public static Channel PlayMusic(FMOD.Sound sound)
    {
        
        Channel ch;
        sound.setMode(MODE.LOOP_NORMAL);
        sound.setLoopCount(-1);
        sound.getLength(out uint length, TIMEUNIT.MS);
        UnityEngine.Debug.Log("PlayMusic:"+length);
        var r = sys.playSound(sound, musicChannelGroup, true, out ch);
        UnityEngine.Debug.Log("PlayMusic:"+r);
        return ch;
    }
    public static Sound CreateKeySound(string path, SoundInfo info)
    {
        Sound temp;
        sys.createSound(path, MODE.CREATESAMPLE, out temp);
        return temp;
    }
    public static Sound CreateSimpleSound(string path)
    {
        Sound sound;
        sys.createSound(path,MODE.CREATESAMPLE,out sound);
        return sound;
    }
    public static Channel PlaySimpleSound(Sound snd)
    {
        Channel ch;
        sys.playSound(snd, sfxChannelGroup,false,out ch);
        return ch;
    }
}