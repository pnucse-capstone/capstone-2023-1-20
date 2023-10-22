using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPreview : MonoBehaviour
{
    [SerializeField]
    SimpleSpectrum spectrum;
    public static bool pause =false;
    float multiply = 1F;
    [SerializeField] AnimationCurve curve;
    // Start is called before the first frame update
    public void SetPCM(AudioData pcm)
    {
        FMODWrapper.SetSpeed(Game.playSpeed);
        MusicPlayer.SetMusic(pcm);
        MusicPlayer.volume = 0;

        MusicPlayer.Resume();
    }

    AudioClip prev;
    // Update is called once per frame

    public float[] GetSpectrumData()
    {

        float[] temp = new float[512];
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = 0;
        }
        return temp;
    }
    void Update()
    {

        FMODComponent.mutex.WaitOne();
        if (MusicPlayer.isReady)
        {
            if (!MusicPlayer.isPlaying) MusicPlayer.Resume();
            float t = MusicPlayer.time/9F;
            if (MusicPlayer.time > 9)
            {
                MusicPlayer.time = 0;
            }
            MusicPlayer.volume = curve.Evaluate(t)*multiply;

            spectrum.spectrumInputData = GetSpectrumData();
        };
        FMODComponent.mutex.ReleaseMutex();
    }
    public void FadeOut()
    {
        StartCoroutine(CoFadeOut());
    }
    IEnumerator CoFadeOut()
    {
        for(float t = 0; t < 1F; t += Time.deltaTime)
        {
            multiply = 1F - t;
            yield return null;
        }
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
