using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSpeedSetting : MonoBehaviour
{
    [SerializeField]
    Text txt;
    [SerializeField]
    Slider slider;
    
    // Start is called before the first frame update
    void Start()
    {
        Show(Game.playSpeed);
    }
    void Show(float value)
    {
        txt.text = "x" + value;
        slider.value = value;
        warning.SetActive(value < 1.0F);
        FMODWrapper.SetSpeed(Game.playSpeed);
    }
    public void SetSpeed(float value)
    {
        Game.playSpeed = Mathf.Round(value * 10) / 10;
        Show(Game.playSpeed);
        FMODWrapper.SetSpeed(Game.playSpeed);
    }
    [SerializeField] GameObject warning;
}
