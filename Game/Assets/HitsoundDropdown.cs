using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
[RequireComponent(typeof(Dropdown))]
public class HitsoundDropdown : MonoBehaviour
{
    Dropdown dropdown;
    // Start is called before the first frame update
    bool preview = false;
    void Awake()
    {   
        dropdown = GetComponent<Dropdown>();    

        dropdown.ClearOptions();
        string folderPath = Path.Combine(Application.streamingAssetsPath,"Hitsound");
        string[] fileNames = Directory.GetFiles(folderPath);
        var list = from i in fileNames where i.EndsWith(".wav") select Path.GetFileNameWithoutExtension(i);
        dropdown.AddOptions(list.ToList());
        dropdown.onValueChanged.AddListener(OnChange);
        string now = PlayerPrefs.GetString("hitsoundtype", "tamb");
        var value= dropdown.options.FindIndex((x)=>x.text == now );
        if(value == -1)
        {
            PlayerPrefs.SetString("hitsoundtype", dropdown.options[0].text);
            dropdown.value = 0;
        }
        else
        {
            dropdown.value = value;
        }
        preview = true;
    }
    void OnChange(int value)
    {
        PlayerPrefs.SetString("hitsoundtype", dropdown.options[value].text);
        string path = Path.Combine(Application.streamingAssetsPath, "Hitsound", dropdown.options[value].text+".wav");
        if (preview)
        {
            FMODWrapper.GetSystem().createSound(path, FMOD.MODE.CREATESAMPLE, out FMOD.Sound snd);
            FMODWrapper.PlaySimpleSound(snd);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
