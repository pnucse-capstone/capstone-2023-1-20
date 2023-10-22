using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNight : MonoBehaviour
{
    [SerializeField]
    GameObject Day;
    [SerializeField]
    GameObject Night;
    // Start is called before the first frame update
    void Start()
    {
        if (isDay())
        {
            Day.gameObject.SetActive(true);
            Night.gameObject.SetActive(false);
        }
        else
        {
            Day.gameObject.SetActive(false);
            Night.gameObject.SetActive(true);
        }

    }

    public static bool isDay()
    {
        return DateTime.Now.Hour < 19 && DateTime.Now.Hour >= 6;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
