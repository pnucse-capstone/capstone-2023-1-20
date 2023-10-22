using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Charote : MonoBehaviour
{
    [Serializable]
    class Entry
    {
        public Emotion emo;
        public Sprite spr;
    }
    public enum Emotion { SMILE,SAD,IDLE , SLEEP, WAKE };
    [SerializeField]
    List<Entry> day;
    [SerializeField]
    List<Entry> night;
    [SerializeField]
    Image image;
    [SerializeField]
    Animator ani;
    // Start is called before the first frame update
    public void Set(Emotion emo, bool useAni=true)
    {
        bool isAfternoon = DateTime.Now.Hour>=12;
        if (isAfternoon)
        {
            image.sprite = night.Find((x) => x.emo == emo).spr;
        }
        else
        {
            image.sprite = day.Find((x) => x.emo == emo).spr;
        }

        if (useAni)
            Bounce();
    }
    public void Bounce()
    {
        ani.Play("", -1, 0);
    }
}
