using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleShortcut : MonoBehaviour
{
    [SerializeField] KeyCode key;
    // Update is called once per frame
    bool vertical = false;
    [SerializeField]
    bool isSketch = false;
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            var toggle = GetComponent<Toggle>();
            toggle.isOn = !toggle.isOn;
        }
        if(vertical != NoteEditor.vertical)
        {
            var toggle = GetComponent<Toggle>();
            toggle.onValueChanged.Invoke(toggle.isOn);
        }
        vertical = NoteEditor.vertical;
        if (isSketch)
        {
            var toggle = GetComponent<Toggle>();
            toggle.isOn = NoteEditor.sketchmode;
        }

    }
}
