using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
//using System.Windows.Media;

//소리재생을 위해 namespace를 추가해 줍니다.
public class TESTSCR : MonoBehaviour
{
    FMOD.System sys;
    FMOD.Channel ch;
    FMOD.ChannelGroup cg;
    FMOD.Sound snd;
    AudioSource audio;

    public Material mat;
    public GameObject obj1;
    public GameObject obj2;
    public GameObject obj3;
    void Start()
    {
        /*
        Debug.Log("Cam");
        uint a;
        int b;
        Application.targetFrameRate = 240;
        FMOD.Factory.System_Create(out sys);
        sys.setDSPBufferSize(64, 16);
        sys.init(512, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
        sys.createSound(Application.dataPath + "/snare.wav", FMOD.MODE.CREATESAMPLE, out snd);
        sys.createChannelGroup("ch", out cg);
        sys.playSound(snd, cg, false, out ch);*/
        StartCoroutine(e());
    }
    IEnumerator e()
    {
        var a = SceneManager.LoadSceneAsync("Epilepsy");
        while (!a.isDone)
        {
            Debug.Log(a.progress);
            yield return null;
        }
    }

    private void Update()
    {
        /*
        sys.update();
        if (Input.anyKeyDown)
        {
            Debug.Log(Time.deltaTime);
            sys.playSound(snd,cg,false,out ch);
        }
        */
    }
    void OnApplicationQuit()
    {
        sys.release();
    }
    bool hitTest(Transform obj,Vector2 A,Vector2 B)
    {
        float angle = obj.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 axis_x1 = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * obj.transform.localScale.x / 2;
        Vector2 axis_y1 = new Vector2(Mathf.Cos(angle + Mathf.PI / 2), Mathf.Sin(angle + Mathf.PI / 2)) * obj.transform.localScale.y / 2;
        Vector2 pos1 = obj.position;
        Vector2 M = (A + B ) / 2;
        Vector2 dist = M - pos1;
        Vector2 axis_x2 = (A-B) / 2;
        Vector2 axis_y2 = new Vector2(-axis_x2.y, axis_x2.x).normalized;
        bool cond1 = (axis_x1.magnitude + Mathf.Abs(Vector2.Dot(axis_x1.normalized, axis_x2)) > Mathf.Abs(Vector2.Dot(axis_x1.normalized, dist)));
        bool cond2 = (axis_y1.magnitude + Mathf.Abs(Vector2.Dot(axis_y1.normalized, axis_x2)) > Mathf.Abs(Vector2.Dot(axis_y1.normalized, dist)));
        bool cond3 = (Mathf.Abs(Vector2.Dot(axis_x1, axis_x2.normalized)) + Mathf.Abs(Vector2.Dot(axis_y1, axis_x2.normalized)) + Vector2.Dot(axis_x2.normalized, axis_x2) > Mathf.Abs(Vector2.Dot(axis_x2.normalized, dist)));
        bool cond4 = (Mathf.Abs(Vector2.Dot(axis_x1, axis_y2)) + Mathf.Abs(Vector2.Dot(axis_y1, axis_y2)) + Vector2.Dot(axis_y2, axis_x2) > Mathf.Abs(Vector2.Dot(axis_y2, dist)));

        return cond1 && cond2 && cond3 && cond4;
    }
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);
    }
}