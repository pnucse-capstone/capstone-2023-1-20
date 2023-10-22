using FMOD;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODBGM : MonoBehaviour
{
    [SerializeField]
    AudioClip clip;
    // Start is called before the first frame update
    void Start()
    {
        FMODWrapper.SetSpeed(1F);
        float[] data = new float[clip.samples*clip.channels];
        clip.GetData(data, 0);
        var s = new PCMSamples(data, clip.samples, clip.channels, clip.frequency);
        MusicPlayer.SetMusic(new AudioData(s));
        MusicPlayer.volume = 1;
        MusicPlayer.Resume();
    }

    public float[] GetSpectrumData()
    {

        float[] temp = FMODWrapper.GetSpectrum();
        return temp;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            MusicPlayer.Pause();
        }
        else
        {
            MusicPlayer.Resume();
        }
    }
}
