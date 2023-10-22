using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ScreenResolutionDropdown : MonoBehaviour
{
    // Start is called before the first frame update
    Resolution[] res;
    Dropdown drop;
    void Start()
    {
        Debug.Log("Current Resolution:" + (Screen.width, Screen.height));
        res = Screen.resolutions;
        res = Screen.resolutions.ToList().FindAll(x => x.refreshRateRatio.value == Screen.currentResolution.refreshRateRatio.value).ToArray();
        drop = gameObject.GetComponent<Dropdown>();
        drop.ClearOptions();
        drop.AddOptions(res.Select(x=>x.ToString()).ToList());
        

        int select =Array.FindIndex(res, x =>
            x.width == Screen.currentResolution.width&&
            x.height == Screen.currentResolution.height &&
            x.refreshRateRatio.value == Screen.currentResolution.refreshRateRatio.value
         );
        drop.value = select;
    }
    public void Refresh() 
    {
        Resolution now = res[drop.value];
        Screen.SetResolution(now.width,now.height, Screen.fullScreenMode);
    }
}
