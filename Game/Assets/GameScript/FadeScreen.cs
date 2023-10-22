using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class FadeScreen : MonoBehaviour
{
    public enum Motion { FadeIn1, FadeIn2, FadeOut1, FadeOutSimple , FadeInSimple ,None}
    [SerializeField]
    Motion init;
    public GameObject cover;
    public bool useSoundFade = true;
    void Start()
    {
        //        Debug.Log(a.GetComponent<Image>().material);
        Animate(init, () => { });
    }

    IEnumerator SoundfadeIn()
    {
        for (float t = 0; t < 0.5F; t += Time.deltaTime)
        {
//            MusicPlayer.volume = t / 0.5F;
            yield return null;
        }
    }
    IEnumerator SoundfadeOut()
    {
        for (float t = 0; t < 0.5F; t += Time.deltaTime)
        {
            MusicPlayer.volume = 1 - t / 0.5F;
            yield return null;
        }
        MusicPlayer.volume = 0;
        MusicPlayer.Pause();
    }
    public void Animate(Motion motion,Action callback)
    {
        switch (motion)
        {
            case Motion.FadeIn2:
                FadeIn(callback);
                break;
            case Motion.FadeIn1:
                FadeIn2(callback);
                break;
            case Motion.FadeOut1:
                FadeOut(callback);
                break;
            case Motion.FadeOutSimple:
                FadeOutSimple(callback);
                break;
            case Motion.FadeInSimple:
                FadeInSimple(callback);
                break;
        }
    }

    // Start is called before the first frame update
    public void FadeIn(Action callback)
    {
        StartCoroutine(fadein(callback));
    }
    public void FadeIn2(Action callback)
    {
        StartCoroutine(fadein2(callback));
    }
    public void FadeOut(Action callback)
    {
        StopAllCoroutines();
        StartCoroutine(fadeout(callback));
    }

    public void FadeOutSimple(Action callback)
    {
        StartCoroutine(fadeoutSimple(callback));
    }

    public void FadeInSimple(Action callback)
    {
        StartCoroutine(fadeInSimple(callback));
    }

    IEnumerator fadein(Action callback)
    {
        if (useSoundFade)
        {
            StartCoroutine(SoundfadeIn());
        }

        cover.SetActive(true);
        GetComponent<Animator>().Play("Disappear", -1, 0);
        yield return new WaitForSeconds(1);
        callback();
        cover.SetActive(false);
    }
    IEnumerator fadein2(Action callback)
    {
        if (useSoundFade)
        {
            StartCoroutine(SoundfadeIn());
        }

        cover.SetActive(true);
        GetComponent<Animator>().Play("Disappear2", -1, 0);

        yield return new WaitForSeconds(1);
        callback();
        cover.SetActive(false);
    }
    IEnumerator fadeout(Action callback)
    {
        if (useSoundFade)
        {
            StartCoroutine(SoundfadeOut());
        }

        cover.SetActive(true);
        GetComponent<Animator>().Play("Appear", -1, 0);
        yield return new WaitForSeconds(1);
        callback();

        //        cover.SetActive(false);
    }

    IEnumerator fadeoutSimple(Action callback)
    {
        if (useSoundFade)
        {
            StartCoroutine(SoundfadeOut());
        }

        cover.SetActive(true);
        GetComponent<Animator>().Play("SimpleFadeOut", -1, 0);
        yield return new WaitForSeconds(1);
        callback();
        //        cover.SetActive(false);
    }

    IEnumerator fadeInSimple(Action callback)
    {

        if (useSoundFade)
        {
            StartCoroutine(SoundfadeIn());
        }
        cover.SetActive(true);
        GetComponent<Animator>().Play("SimpleFadeIn", -1, 0);
        yield return new WaitForSeconds(1F);
        callback();
        cover.SetActive(false);
    }
}
