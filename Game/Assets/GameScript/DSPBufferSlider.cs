using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DSPBufferSlider : MonoBehaviour
{
    [SerializeField]
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        int value =PlayerPrefs.GetInt("dsp", 7);
        GetComponent<Slider>().value = value;
        text.text = $"{1<<value}";
    }
    public void OnValueChange(float value)
    {
        PlayerPrefs.SetInt("dsp", (int)value);
        text.text = $"{1 << (int)value}";
    }
}
