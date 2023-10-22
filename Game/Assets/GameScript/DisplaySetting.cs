
using NVIDIA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DisplaySetting : MonoBehaviour
{
    [SerializeField]
    PostProcessLayer post;
    public static int FPS => PlayerPrefs.GetInt("fps", 60);

    // Start is called before the first frame update
    void Start()
    { 
        var r = PlayerPrefs.GetInt("Reflex", 0);
        GetComponent<Reflex>().isLowLatencyMode = r!= 0;
        GetComponent<Reflex>().isLowLatencyBoost = r == 2;
        if (r ==0)
        {
            GetComponent<Reflex>().enabled = false;
            GetComponent<Reflex>().isIgnored = true;
        }
        SetBloom();
        Application.targetFrameRate = FPS;
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("VSync", 0);
    }
    void SetBloom()
    {
        if (Game.reduced)
        {
            post.enabled = false;

        }
        else
        {
            post.enabled = PlayerPrefs.GetInt("Bloom", 0) == 1;
        }

    }
    public static void Apply()
    {

        Application.targetFrameRate = FPS;
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("VSync", 0);
    }
    // Update is called once per frame
}
