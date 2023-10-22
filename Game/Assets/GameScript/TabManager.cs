using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    EventData target;
    static TabManager prev;
    public GameObject view;

    ColorPickerAdapter color_picker
    {
        get
        {
            return transform.parent.parent.parent.GetComponentInChildren<ColorPickerAdapter>(true);
        }
    }
    LinePathEditor linePathEditor
    {
        get
        {
            return transform.parent.parent.parent.GetComponentInChildren<LinePathEditor>(true);
        }
    }

    public void Select()
    {
        Debug.Log(name);
        if (prev != null)
        {
            prev.BroadcastMessage("WriteEvent", target);
            prev.SetTab(false);
        }
        SetTab(true);
        BroadcastMessage("ShowEvent", target);
        prev = this;

        color_picker.gameObject.SetActive(false);
        linePathEditor.gameObject.SetActive(false);
    }
    public void SetTab(bool active)
    {
        GetComponent<Button>().interactable = !active;
        view.SetActive(active);
    }
    public void ShowEvent(EventData edit)
    {
        target = edit;
    }
    // Start is called before the first frame update

}
