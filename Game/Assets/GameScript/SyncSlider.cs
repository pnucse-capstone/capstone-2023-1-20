using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class SyncSlider : MonoBehaviour
{
    // Start is called before the first frame update
    public Text txt;
    public Slider slider;
    void Start()
    {
        txt.text = formatSync(PlayerPrefs.GetFloat("sync", 0F));
        slider.value = 1000*PlayerPrefs.GetFloat("sync", 0F);
    }
    public void ChangeSlider(float sync)
    {
        txt.text = formatSync(sync / 1000F);
        PlayerPrefs.SetFloat("sync", sync/1000F);
    }
    string formatSync(double value)
    {
        return string.Format((value > 0 ? "+" : "") + "{0:F3}" + "s", value);
    }
}
