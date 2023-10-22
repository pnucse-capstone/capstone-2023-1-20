using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaneCover : MonoBehaviour
{
    public static float bound = 10;
    /*
    void Update()
    {
        if (Input.GetKey(KeySetting.keyCoverup))
        {
            PlayerPrefs.SetFloat("TimeRange", Mathf.Clamp(PlayerPrefs.GetFloat("TimeRange", 10) - Time.deltaTime,0,10));
        }
        if (Input.GetKey(KeySetting.keyCoverdown))
        {
            PlayerPrefs.SetFloat("TimeRange", Mathf.Clamp(PlayerPrefs.GetFloat("TimeRange", 10) + Time.deltaTime,0,10));

        }
        bound = PlayerPrefs.GetFloat("TimeRange", 10);
    }*/
}
