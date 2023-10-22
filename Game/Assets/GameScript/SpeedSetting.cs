using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpeedSetting : MonoBehaviour
{
    Text txt;
    // Start is called before the first frame update
    void Start()
    {
        Game.speed_multiplier = PlayerPrefs.GetFloat("NoteSpeed", 3F);
        txt = GetComponentInChildren<Text>();
        float value = Game.speed_multiplier;
        txt.text = "x"+value;
        GetComponentInChildren<Slider>().value = value;
    }
    public void SetSpeed(float multiplier)
    {
        multiplier = Mathf.Round(multiplier*10)/10;
        txt.text = "x"+multiplier;
        Game.speed_multiplier = multiplier;
        PlayerPrefs.SetFloat("NoteSpeed", Game.speed_multiplier);
    }
}
