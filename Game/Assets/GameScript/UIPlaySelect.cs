using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
public class UIPlaySelect : MonoBehaviour
{
    public GameObject steam_check;

    // Start is called before the first frame update
    void Start()
    {
        steam_check.SetActive(!SteamClient.IsValid);    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
