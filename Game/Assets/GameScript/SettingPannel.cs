using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SettingPannel : MonoBehaviour
{
    // Start is called before the first frame update
    //    public GameObject tf_stereo;
    //    public GameObject tf_tapsound;
    [SerializeField]
    Animator ani;
//    public static bool isOpen = false;
    public Toggle fullscreen;
    void Start()
    {
        fullscreen.isOn = Screen.fullScreen;
        gameObject.SetActive(false);
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))Close();
        FMODWrapper.Refresh();
    }
    public void Open()
    {
        NoteEditor.PopupLock = false;
        ani.SetBool("Setting", true);
        NoteEditor.PopupLock = true;
        Game.isStereo=(PlayerPrefs.GetInt("stereo", 1) == 1 ? true : false);
//        tf_stereo.GetComponent<Text>().text = ""+Game.isStereo;
//        tf_tapsound.GetComponent<Text>().text = "" + Game.isTapsound;
        gameObject.SetActive(true);
        gameObject.GetComponent<Animator>().Play("SettingOpenAnime", -1, 0);
    }
    IEnumerator CloseAnimation()
    {
        gameObject.GetComponent<Animator>().Play("SettingCloseAnime", -1, 0);
        yield return new WaitForSeconds(0.2F);
        gameObject.SetActive(false);
    }
    public void Close()
    {
        ani.SetBool("Setting", false);
        NoteEditor.PopupLock = true;
        MusicPreview.pause = false;
        NoteEditor.PopupLock = false;
        StartCoroutine("CloseAnimation");
    }
    public void toggleStereo()
    {
        Game.isStereo = !Game.isStereo;
        PlayerPrefs.SetInt("stereo", Game.isStereo ? 1 : 0);
    }
    public void openCustomScene()
    {
        SceneManager.LoadScene("CustomSelect");
    }
    public void toggleFullscreen(Toggle toggle)
    {
        Screen.fullScreen = toggle.isOn;
    }
}
