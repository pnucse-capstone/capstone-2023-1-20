using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SimpleAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    public bool FADE_IN = false;
    public bool FADE_OUT= false;
    public bool EXPAND= false;
    public bool SHRINK = false;
    public float delay = 0F;
    public float front_duration = 1F;
    public float middle_duration = 2F;
    public float back_duration = 1F;
    public float alpha = 1F;
    public bool lerp = false;
    Vector3 init_scale;
    public AnimationCurve curve;
    private SpriteRenderer render;
    void Awake()
    {
        init_scale = gameObject.transform.localScale;
        setAlpha(0);
        if(EXPAND)StartCoroutine(expandAnimation());
        StartCoroutine(fadeInOut(FADE_IN, FADE_IN,FADE_OUT,FADE_OUT));
    }
    public void Play(bool useDelay=true,bool useFadeIn=true, bool useWait=true,bool useFadeOut=true)
    {
        StartCoroutine(fadeInOut(useDelay,useFadeIn,useWait,useFadeOut));
    }
    IEnumerator fadeInOut(bool useDelay = true, bool useFadeIn = true, bool useWait = true, bool useFadeOut = true)
    {
        float l, r;
        if (useDelay)
        {
            yield return new WaitForSeconds(delay);
        }
        if (useFadeIn)
        {
            float t = 0;
            while (0<= t && t <= front_duration)
            {
                setAlpha(t / front_duration * alpha);
                t += Time.deltaTime;
                yield return null;
            }
            setAlpha(alpha);
        }
        setAlpha(alpha);
        if (useWait)
        {
            yield return new WaitForSeconds(middle_duration);
        }

        if (useFadeOut)
        {
            float t = 0;
            while (0 <= t && t <= back_duration)
            {
                setAlpha((1F - t / back_duration) * alpha);
                t += Time.deltaTime;
                yield return null;
            }
            setAlpha(0);
            gameObject.SetActive(false);
        }

    }
    public void playExapnd()
    {
//        gameObject.transform.localScale = new Vector3(0, 0, 0);
        StartCoroutine(expandAnimation());
    }
    public void playShrink()
    {
        //        gameObject.transform.localScale = new Vector3(0, 0, 0);
        StartCoroutine(shrinkAnimation());
    }
    public void playFadein()
    {
        setAlpha(0);
        StartCoroutine(fadeInOut(true,true,false,false));
    }
    public void playFadeOut()
    {
        StartCoroutine(fadeInOut(false,false,true,true));
    }
    public void setAlpha(float a)
    {
        foreach (var i in gameObject.GetComponentsInChildren<Image>())
        {
            Color temp = i.color;
            temp.a = a;
            i.color = temp;
        }
        foreach (var i in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            Color temp = i.color;
            temp.a = a;
            i.color = temp;
        }
        foreach (var i in gameObject.GetComponentsInChildren<Text>())
        {
            Color temp = i.color;
            temp.a = a;
            i.color = temp;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator expandAnimation()
    {
        for(float t=0F;t<delay;t+=Time.deltaTime)
        {
            yield return null;
        }
        for(float t=0F;t<front_duration;t+= Time.deltaTime)
        {
            float sizex = init_scale.x * t / front_duration;
            float sizey = init_scale.y * t / front_duration;
            gameObject.transform.localScale =new Vector3(sizex, sizey, 1);
            yield return null;
        };
        gameObject.transform.localScale = init_scale;
    }
    IEnumerator shrinkAnimation()
    {
        for (float t = 0F; t < delay; t += Time.deltaTime)
        {
            yield return null;
        }
        for (float t = 0F; t < back_duration; t += Time.deltaTime)
        {
            gameObject.transform.localScale = new Vector3(init_scale.x * (1F-t / back_duration), init_scale.y * (1F-t / back_duration), 1);
            yield return null;
        };
        gameObject.transform.localScale = new Vector3(0,0,0);
    }
}
