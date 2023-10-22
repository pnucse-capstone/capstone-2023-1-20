using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class GameInit : MonoBehaviour
{
    public static KeySettingPreset defaultKeySetting;
    public static DifficultyData difficultySet;
    public static NoteColorMap colors;
    public static string persistentDataPath;
    static List<NoteColorMap> eventColorsets;
    // Start is called before the first frame update

    void Start()
    {
        DisplaySetting.Apply();
        defaultKeySetting = Resources.Load("DefaultKeySetting") as KeySettingPreset;
        difficultySet = Resources.Load("DifficultySet") as DifficultyData;
        colors = Resources.Load("NoteColorPreset/ColorSet") as NoteColorMap;
        persistentDataPath = Application.persistentDataPath;

        var colorsets = Resources.LoadAll<NoteColorMap>("ColorSet").ToList();
        if (PlayerPrefs.HasKey("ColorSet"))
        {
            colors = colorsets.Find((x) => x.presetName == PlayerPrefs.GetString("ColorSet"));
            if (colors == null)
            {
                colors = colorsets[0];
                PlayerPrefs.SetString("ColorSet", colors.presetName);
            }
        }
        else
        {
            colors = colorsets[0];
            Debug.Log("Colorset:"+colors);
        }
        eventColorsets = Resources.LoadAll<NoteColorMap>("EventColorSet").ToList();

//        var task = CacheAvartars();

#if !UNITY_EDITOR
        Application.runInBackground = false;
#endif
    }
    public static NoteColorMap GetColorPreset(int code)
    {
        return eventColorsets[code];
    }

}
