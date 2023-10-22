using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VerticalButton : MonoBehaviour
{
    [SerializeField]
    Text text;

    void Start()
    {
    }
    private void Update()
    {
        Show();

    }
    public void ChangeMode()
    {
        if (NoteEditor.vertical)
        {
            NoteEditor.vertical = false;
        }
        else
        {
            NoteEditor.vertical = true;
        }
    }
    void Show()
    {
        if (NoteEditor.vertical)
        {
            text.text = "VER.";
        }
        else
        {
            text.text = "HORIZ.";
        }

    }
}
