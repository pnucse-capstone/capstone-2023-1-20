using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameEventEditEntryTween : MonoBehaviour
{
    [SerializeField]
    Dropdown dropdown;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ShowEvent(EventData data)
    {
        dropdown.value = Mathf.RoundToInt(data.interpole);
    }
    public void WriteEvent(EventData select)
    {
        select.Use("interpole", true);
        select.interpole = dropdown.value;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
