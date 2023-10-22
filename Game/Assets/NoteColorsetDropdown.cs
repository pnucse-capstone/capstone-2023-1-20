using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NoteColorsetDropdown : MonoBehaviour
{
    
    Dropdown dropdown;
    List<NoteColorMap> colorsets;

    void Awake()
    {
        colorsets = Resources.LoadAll<NoteColorMap>("ColorSet").ToList();
//        GameInit.colors = colorsets.Find((x)=>x.presetName == PlayerPrefs.GetString("ColorSet", "Default"));
        colorsets = colorsets.FindAll(x => !string.IsNullOrEmpty(x.presetName)).ToList();
    }

    // Start is called before the first frame update
    //�⺻�����δ� �״��, ��� ��� �������ִ¹������..
    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        dropdown.AddOptions(colorsets.Select(x => x.presetName).ToList());
        dropdown.value = dropdown.options.FindIndex(x => x.text == PlayerPrefs.GetString("ColorSet", "Default"));
        dropdown.onValueChanged.AddListener(Change);
    }

    public void Change(int value)
    {
        NoteColorMap color = colorsets.Find((x)=>x.presetName == dropdown.options[value].text);
        Debug.Log("ColorSet:"+color.presetName);
        GameInit.colors = color;
        PlayerPrefs.SetString("ColorSet", color.presetName);
    }

}
