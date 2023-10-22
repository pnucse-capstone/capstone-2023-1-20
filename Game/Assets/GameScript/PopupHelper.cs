using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupHelper : MonoBehaviour
{
    // Start is called before the first frame update
    public PopUp popup;
    public PopUp exit;
    // Update is called once per frame
    void Update()
    {
        if (exit == null) return;
        if (Input.GetKeyDown(KeyCode.Escape) && !NoteEditor.PopupLock)
        {
            exit.Open();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && NoteEditor.PopupLock)
        {
            exit.Close();
        }
    }
}
