using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODSFX : MonoBehaviour
{

    [SerializeField]
    AudioClip clip;
    // Start is called before the first frame update
    FMOD.Sound sound;
    void Start()
    {
        float[] data = new float[clip.samples * clip.channels];
        clip.GetData(data, 0);
        var s = new PCMSamples(data, clip.samples, clip.channels, clip.frequency);
        sound = s.CreateFMODSound();
    }
    public void Play()
    {
        Debug.Log("play");
        FMODWrapper.PlaySimpleSound(sound);
    }

}
