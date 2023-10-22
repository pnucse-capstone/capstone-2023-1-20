using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;

public class DrawSpectrum : MonoBehaviour
{
    public GameObject sprite;
    public GameObject prefab;
    int barCount = (1 << 9);

    GameObject[] objs;
    [SerializeField]Toggle toggle;

    SpectrumWindow wind;
    static bool value = true;
    // Start is called before the first frame update
    void Start()
    {
        objs = new GameObject[barCount];
        toggle.isOn = value;
        for(int i=0;i<barCount; i++)
        {
            objs[i] = Instantiate(prefab,gameObject.transform) as GameObject;
        }
        Refresh();
        OnLoad();
    }
    public void SetEnable(bool value)
    {
        DrawSpectrum.value = value;
        gameObject.SetActive(value);
    }
    public void OnLoad()
    {
        if (MusicPlayer.GetAudioData() == null) return;
        Debug.Log("Spectrum ONLOAD:" + MusicPlayer.GetAudioData().GetPCM().length);
        wind = new SpectrumWindow(MusicPlayer.GetAudioData().GetPCM());
    }
    public void Refresh()
    {
        if (!gameObject.activeSelf) return;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (!NoteEditor.isLoaded)
        {
            return;
        }

        float dx = Camera.main.orthographicSize*4/ barCount;

        for (int i = 0; i<barCount;i++) // i번째 막대기를 그린다
        {
            float x = (dx*(i-barCount/4) +EditorCameraScroll.scroll_pos*NoteEditor.zoom); 

            // barCount/4를 빼는 이유는 보여주는 부분의 1/4 정도를 왼쪽으로 평행이동시키기 위함. 카메라의 orthogonal size에 해당
            float t1 = (x)/ NoteEditor.zoom - Game.table.offset;
            float t2 = (x+dx) / NoteEditor.zoom - Game.table.offset;
            Set(objs[i], dx , x, wind.GetMax(t1,t2), wind.GetMin(t1,t2));
        }
    }
    void Set(GameObject obj, float dt, float t, float max, float min)
    {
        obj.transform.localScale = new Vector3(dt, 3*max-3*min);
        obj.transform.localPosition = new Vector3(t, 3*min);
    }
    class SpectrumWindow
    {
        PCMSamples pcm;
        float max;
        float min;
        public SpectrumWindow(PCMSamples pcm) 
        {
            Debug.Log("Spectrum Window");
            this.pcm= pcm;
            max = pcm.Samples.Max();
            min = pcm.Samples.Min();
        }
        public int TimeToIndex(float time)
        {
            return (int)(time * pcm.SampleRate * pcm.Channels);
        }
        public float IndexToTime(int i)
        {
            return 1F*i/ pcm.SampleRate / pcm.Channels;
        }
        public float Max()
        {
            return max;
        }
        public float Min()
        {
            return min;
        }
        public float GetMax(float x1, float x2)
        {
            int k1 = TimeToIndex(x1);
            int k2 = TimeToIndex(x2);
            float localMax = min;
            if (k1 < 0 || k2 >= pcm.Samples.Count()) { return 0; };
            int step = Mathf.Max(Mathf.RoundToInt(16 / NoteEditor.zoom),1);
            for (int i=k1;i < k2; i+=step)
            {
                localMax = Mathf.Max(localMax, pcm.Samples[i]);
            }
            return localMax;
        }
        public float GetMin(float x1, float x2)
        {
            int k1 = TimeToIndex(x1);
            int k2 = TimeToIndex(x2);
            float localMin = max;
            if (k1 < 0 || k2 >= pcm.Samples.Count()) { return 0; };
            int step = Mathf.Max(Mathf.RoundToInt(16 / NoteEditor.zoom), 1);
            for (int i = k1; i < k2; i+=step)
            {
                localMin = Mathf.Min(localMin, pcm.Samples[i]);
            }
            return localMin;

        }
    }
}
