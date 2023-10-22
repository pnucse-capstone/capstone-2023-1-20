using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "DefaultKeysetting", menuName = "Scriptable Objects/Key Setting Preset", order = 1)]
public class KeySettingPreset : ScriptableObject
{
    [SerializeField]
    public KeyMapping[] entries;
    [Serializable]
    public class KeyMapping
    {
        public string key;
        public KeyCode default_key;
    }
    public KeyCode GetDefaultKey(string key)
    {
        try
        {
            return entries.First((i) => i.key == key).default_key;
        }
        catch (Exception e)
        {
            return KeyCode.None;
        }
    }
}