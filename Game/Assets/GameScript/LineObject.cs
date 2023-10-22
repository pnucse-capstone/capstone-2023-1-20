using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class LineObject : MonoBehaviour
{
    public LineRenderer line;
    Material mat;
    float now_rate = 0;
    [SerializeField]GameObject Center;
    SpriteRenderer center;
    Gradient base_color;

    [SerializeField] AnimationCurve curve;
    [SerializeField] GameObject textObj;
    [SerializeField] Text text;
    [SerializeField] Sprite[] image;
    [SerializeField] SpriteRenderer judgePoint;
    float idleAlpha=0.15F, highAlpha=0.5F;
    // Start is called before the first frame update
    private void Awake()
    {
        center = Center.GetComponent<SpriteRenderer>();
        base_color = GetComponentInChildren<LineRenderer>().colorGradient;
        line = GetComponentInChildren<LineRenderer>();
    }
    void Start()
    {
        mat = line.material;
        StartCoroutine(JudgePoint());

        bool useJudgepoint = PlayerPrefs.GetInt("JudgePointShow", 0) == 1;
        if (useJudgepoint)
        {
            idleAlpha = 0.16F;
        }
        else
        {
            idleAlpha = 0F;
        }
        SetJudgepointAlpha(idleAlpha);
    }

    IEnumerator JudgePoint()
    {
        float prev = Game.judgepoint_offset;
        while (true)
        {
            if(prev != Game.judgepoint_offset)
            {
                StartCoroutine(JudgePointAnimate());
            }
            prev = Game.judgepoint_offset;
            yield return null;
        }
    }
    IEnumerator JudgePointAnimate()
    {

        for (float t = 0; t < 0.5F; t += Time.deltaTime)
        {
            SetJudgepointAlpha(1-t/0.5F);
            yield return null;
        }
    }
    void SetJudgepointAlpha(float a)
    {
        var color = judgePoint.color;
        color.a = Mathf.Lerp(idleAlpha,highAlpha, a);
        judgePoint.color = color;

    }
    public void SetCenterColor(Color c)
    {
        center.material.SetColor("_Tint", c);
    }
    public void SetBaseColor(Color c)
    {
        if (c == null) return;
        now_rate = curve.Evaluate(t);
        GradientColorKey[] keys = base_color.colorKeys;
        keys[0].color = c;
        base_color.SetKeys(keys, base_color.alphaKeys);
        
    }
    public void SetWidth(float width)
    {
        line.widthMultiplier = width;
    }
    public void SetNoteScale(Vector3 scale)
    {
        judgePoint.transform.localScale  = Center.transform.localScale = scale;
    }

    internal void SetRotation(float v)
    {
        judgePoint.transform.rotation = Center.transform.rotation = Quaternion.Euler(0, 0, v);
    }
    public void SetText(string c)
    {
        text.text = c;
        StartCoroutine(TextFade());
    }
    IEnumerator TextFade()
    {
        yield return new WaitForSeconds(1F);
        float duration = 1F;
        Color color = text.color;
        for (float t = 0; t<duration; t+=Time.deltaTime)
        {
            float alpha = 1F-t/duration ;
            color.a = alpha;
            text.color = color;
            yield return null;
        }
        text.color = Color.clear;
        //        gameObject.SetActive(false);
        yield return null;
    }


    float t = 0;
    Gradient next = new Gradient();
    void Update()
    {
        now_rate = curve.Evaluate(t);
        GradientAlphaKey[] alphakeys = base_color.alphaKeys;
        for(int i=0;i<alphakeys.Length;i++)
        {
            alphakeys[i].alpha = (0.5F+0.5F*now_rate)* alphakeys[i].alpha;
        }
        Center.GetComponent<SpriteRenderer>().sprite = image[(int)Game.state.properties.noteImage[0]];
        next.SetKeys(base_color.colorKeys,alphakeys);
        line.colorGradient = next;

        t += Time.deltaTime*10;
    }
    public void SetPositions(LinePath path)
    {
        line.positionCount = path.points.Length;
        line.SetPositions(path.points.Select((x)=>x.ToVector3()).ToArray());

        transform.position = path.points[0].ToVector3();
        judgePoint.transform.position = path.Get(Game.judgepoint_offset).ToVector3();
    }

    public void SetPositions(LinePath.Point[] points)
    {
        line.positionCount = points.Length;
        line.SetPositions(points.Select((x) => x.ToVector3()).ToArray());

        transform.position = points[0].ToVector3();
        var linepath = new LinePath();
        linepath.points = points;
        judgePoint.transform.position = linepath.Get(Game.judgepoint_offset).ToVector3();
    }

    // Update is called once per frame
    public void Effect()
    {
        t = 0;
    }
}
