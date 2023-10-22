using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharoteEyecolorChanger : MonoBehaviour
{
    [SerializeField]
    Image image;
    [SerializeField]
    Sprite Day, Night;
    // Start is called before the first frame update
    void Start()
    {
        if(DateTime.Now.Hour >= 12)
        {
            image.sprite = Night;
        }
        else
        {
            image.sprite = Day;
        }
    }

    // Update is called once per frame
}
