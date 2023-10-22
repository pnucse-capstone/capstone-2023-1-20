using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class togglePlayerPrefs : MonoBehaviour
{
    public string key = "";
    Toggle toggle;
    public bool default_value = true;
    void Start()
    {
        toggle = gameObject.GetComponent<Toggle>();
        toggle.isOn = PlayerPrefs.GetInt(key, default_value?1:0) == 1 ;
        toggle.onValueChanged.AddListener(togglePrefs);
    }
    public void togglePrefs(bool value)
    {
        PlayerPrefs.SetInt(key, value?1:0);
    }
}
