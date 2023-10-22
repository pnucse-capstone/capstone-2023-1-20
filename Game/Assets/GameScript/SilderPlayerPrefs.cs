using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SilderPlayerPrefs : MonoBehaviour
{
    [SerializeField]
    int precision =2;
    [SerializeField]
    string prefix;
    public string key = "";
    public float default_value = 0.1f;

    [SerializeField]
    string postfix;

    [SerializeField]
    float multiplier = 1;

    Slider slider;
    Text txt;
    void Start()
    {
        slider = gameObject.GetComponent<Slider>();
        slider.value= PlayerPrefs.GetFloat(key, default_value);
        slider.onValueChanged.AddListener(togglePrefs);
        txt = gameObject.GetComponentInChildren<Text>();
        txt.text = prefix+string.Format("{0:f"+precision+"}", slider.value * multiplier) +postfix;
    }
    public void togglePrefs(float value)
    {
        PlayerPrefs.SetFloat(key, value );
        txt.text = prefix + string.Format("{0:f" + precision + "}", slider.value*multiplier) + postfix;

    }
}
