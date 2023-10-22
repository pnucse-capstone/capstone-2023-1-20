using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class MouseSenseSlider : MonoBehaviour
{
    public Text text;
    // Update is called once per frame
    void Start()
    {
        gameObject.GetComponent<Slider>().value = (PlayerPrefs.GetFloat("sens", 0.4F)-0.1F)*10F/9F;
    }
    void Update()
    {
        Game.mouse_sens=(float)Math.Round(gameObject.GetComponent<Slider>().value*0.9F+0.1F,2);
        PlayerPrefs.SetFloat("sens",Game.mouse_sens);
        text.GetComponent<Text>().text = string.Format("{0:f2}",Game.mouse_sens);
    }
}
