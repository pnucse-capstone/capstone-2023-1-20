using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FPSSlider : MonoBehaviour
{
    public Text text;
    public Dropdown Reflex;
    public Slider fpsSlider;
    public Dropdown Vsync;
    void Start()
    {
        Refresh();
    }

    int GetReflex()
    {
        return PlayerPrefs.GetInt("Reflex", 0);
    }

    void Refresh()
    {
        int fps = GetFps();
        text.text = fps + "";
        if (fps == -1) text.text = "∞";

        Vsync.value = GetVsync();
        fpsSlider.value = fpsToCode(GetFps());
        Reflex.value = GetReflex();

        //        Debug.Log((GetVsync(),GetReflex()));
        //        Vsync.interactable = GetReflex()==0;
        fpsSlider.interactable = GetVsync() == 0;// && GetReflex() == 0;

        DisplaySetting.Apply();

    }
    public void SetReflex(int value)
    {
        /*
        if (value != 0 )
        {
            SetVsync(0);
            SetFpsByValue(-1);
        }*/
        Refresh();
    }

    public void SetVsync(int value)
    {
        if (value != 0)
        {
            SetFpsByValue(-1);
        }
        PlayerPrefs.SetInt("VSync",value);
        Refresh();
    }
    public int GetVsync()
    {
        return PlayerPrefs.GetInt("VSync", 1);
    }
    private int GetFps()
    {
        return PlayerPrefs.GetInt("fps", -1);
    }
    public void SetFpsByValue(int fps)
    {
        SetFpsByCode(fpsToCode(fps));
    }
    public void SetFpsByCode(float code)
    {
        SetFpsByCode((int)code);
    }
    public void SetFpsByCode(int code)
    {
        int fps = codeToFps(code);
        PlayerPrefs.SetInt("fps", fps);
        Refresh();
    }
    static int codeToFps(int frame_code)
    {
        switch (frame_code)
        {
            case 8: return -1;
            case 0: return 30;
            case 1: return 60;
            case 2: return 120;
            case 3: return 144;
            case 4: return 240;
            case 5: return 300;
            case 6: return 360;
            case 7: return 400;
            default: return -1;
        }
    }
    static int fpsToCode(int fps)
    {
        switch (fps)
        {
            case -1:return 8;
            case 30: return 0; 
            case 60: return 1; 
            case 120: return 2;
            case 144: return 3;
            case 240: return 4;
            case 300: return 5;
            case 360: return 6;
            case 400: return 7;
            default: return 8;
        }
    }
}
