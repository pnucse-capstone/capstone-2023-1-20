using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainSelect : MonoBehaviour
{
    public void RunEditor()
    {
        SceneManager.LoadScene(SceneNames.EDITOR);
        NoteEditor.Reset();
    }

}
