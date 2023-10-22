using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DifficultySelectEntry : MonoBehaviour
{
    Animator ani;
    [SerializeField]
    Text text;
    [SerializeField]
    Image image;
    void Start()
    {
        ani = GetComponent<Animator>(); 
    }
    public void Set(bool isOn)
    {
        if(isOn)ani.Play("Appear",-1,0);
        else ani.Play("Disappear", -1, 0);

    }
    public void Show(string difficulty,Color color)
    {
        text.text = difficulty;
        image.color = color;
    }
}
