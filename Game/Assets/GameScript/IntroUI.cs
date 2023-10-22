using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class IntroUI : MonoBehaviour
{
    public GameObject pannel;
    public Animator CameraAni;
    public Animator ButtonsAni;
    public static AsyncOperation selectScene;
    public GameObject Logo;
    public Animator LogoAni;
    public static bool firstLoad=true;
    [SerializeField]
    FadeScreen fade;
    void Start()
    {
        Game.playSpeed = 1F;
        Game.speed_multiplier = PlayerPrefs.GetFloat("NoteSpeed", 3F);
        Game.modi = new MODINone();
        NoteEditor.PopupLock = true;
        if (!firstLoad) Logo.SetActive(false);
        PlayerPrefs.SetString("modes", "");
        Application.targetFrameRate = PlayerPrefs.GetInt("fps", 60);
        StartCoroutine(Slide());
    }
    IEnumerator Slide()
    {
        yield return new WaitForSeconds(0.01F);
        
        fade.FadeIn2(() => { });
        if (firstLoad)
        {
            Debug.Log("First Load");
            yield return new WaitForSeconds(4F);
        }
        else
        {
            LogoAni.Play("Appear",0,0.5F);
        }
        NoteEditor.PopupLock = false;
        while (true)
        {
            if (Input.anyKeyDown)
            {
                CameraAni.SetTrigger("Slide");
                ButtonsAni.SetBool("Open", true);
                firstLoad = false;
            }
            yield return null;
        }
    }
    void Update()
    {
        //ani.SetBool("Open", true);

        //            StartCoroutine(ChangeScene());
    }
}
