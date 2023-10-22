using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[Serializable]
class KeySettingEntry
{
    public string key;
    public Button bt;
    public void initText()
    {
        bt.GetComponentInChildren<Text>().text = KeySetting.GetMappedKey(key).ToString();
    }
    public string text
    {
        get => bt.GetComponentInChildren<Text>().text;
        set => bt.GetComponentInChildren<Text>().text = value;
    }
    public Color color
    {
        get => bt.GetComponentInChildren<Text>().color;
        set => bt.GetComponentInChildren<Text>().color = value;
    }

}
public class KeySetting : MonoBehaviour
{
    [SerializeField]
    KeySettingEntry[] entries;
    // Start is called before the first frame update
    KeySettingEntry focus = null;
    void Start()
    {
        foreach(var i in entries)
        {
            i.initText();
            i.bt.onClick.AddListener(() => { 
                focus = i;
                Refresh(); 
            });
        }
    }
    public static KeyCode keyRestart
    {
        get => GetMappedKey("keyRestart");
    }

    public static KeyCode keyJudgeUp
    {
        get => GetMappedKey("keyJudgeUp");
    }
    public static KeyCode keyJudgeDown
    {
        get => GetMappedKey("keyJudgeDown");
    }

    public static KeyCode keyJudgeUIUp
    {
        get => GetMappedKey("keyJudgeUIUp");
    }
    public static KeyCode keyJudgeUIDown
    {
        get => GetMappedKey("keyJudgeUIDown");
    }
    public static KeyCode keySpeeddown
    {
        get => GetMappedKey("keySpeeddown");
    }
    public static KeyCode keySpeedup
    {
        get => GetMappedKey("keySpeedup");
    }
    public static KeyCode keyCoverup
    {
        get => GetMappedKey("keyCoverup");
    }
    public static KeyCode keyCoverdown
    {
        get => GetMappedKey("keyCoverdown");
    }
    public static KeyCode GetMappedKey(string key)
    {
        return (KeyCode)PlayerPrefs.GetInt(key, (int)GameInit.defaultKeySetting.GetDefaultKey(key));
    }
    public void SetMappedKey(string key,KeyCode keycode)
    {
        var keys = GetKeyNames();
        keys = keys.Where((x) => GetMappedKey(x) == keycode);
        foreach (var duplicateKey in keys)
        {
            PlayerPrefs.SetInt(duplicateKey, (int)GetMappedKey(key));
        }
        PlayerPrefs.SetInt(key, (int)keycode);
    }

    void Refresh()
    {
        foreach (var i in entries)
        {
            i.color = Color.black;
            i.text = ""+GetMappedKey(i.key);
        }
        if (focus != null)
        {
            focus.color = Color.red;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) focus = null;
    }

    private static IEnumerable<string> GetKeyNames()
    {
        return GameInit.defaultKeySetting.entries.Select((x) => x.key);
    }

    public void OnGUI()
    {
        if (focus != null && Event.current.isKey && Event.current.keyCode != KeyCode.Escape)
        {
            KeyCode key = Event.current.keyCode;
            focus.text = key.ToString();
            SetMappedKey(focus.key,key);
            focus = null;
            Refresh();
        }
    }

}