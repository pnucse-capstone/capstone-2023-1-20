using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using System;
public class steamProfile : MonoBehaviour
{
    public Text name;
    public Text date;
    public Image profile;
    // Start is called before the first frame update
    void Start()
    {
        DateTime Date = DateTime.Now;
        date.text = Date.ToString();
        if (SteamClient.IsValid) name.text = "Player- " + SteamClient.Name;
        else name.text = "offline";
        //        Debug.Log(date.Year+"/"+date.Month+"/"+date.Day+" "+date.DayOfYear)
    }

    // Update is called once per frame
    void Update()
    {
    }
}
