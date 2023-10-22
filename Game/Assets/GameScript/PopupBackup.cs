using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupBackup : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(AutoBackup.Exists() && !NoteEditor.isLoaded);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
