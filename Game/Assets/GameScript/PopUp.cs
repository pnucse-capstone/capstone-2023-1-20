using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PopUp : MonoBehaviour
{
    public Text msg;
    public Text loadingText;
    public GameObject loading_bar;
    private void Start()
    {
    }
    public void Open()
    {
        NoteEditor.PopupLock = true;
        gameObject.SetActive(true);
    }
    public void Close()
    {
        NoteEditor.PopupLock = false;
        gameObject.SetActive(false);
    }
    public void SetMessage(string message)
    {
        msg.text = message;
    }
    public void SetLoading(float value)
    {
        loadingText.text = string.Format("{0:F2}%",value*100F);
    }

    public void backToMain()
    {
        KeySoundPallete.Release();
        SceneManager.LoadScene(SceneNames.INTRO);
        AutoBackup.Archive(); 
    }
    public void Quit()
    {
        Application.Quit();
    }
}
