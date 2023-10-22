using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamworksButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(PackSelectUI.pack is MusicPackCustom);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
