using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientNew : MonoBehaviour
{
    public Material mat;
    public Color default_tone;
    void Start()
    {
        mat= gameObject.GetComponent<SpriteRenderer>().material;
    }
    void Update()
    {
        setTone(default_tone);
    }
    public void setColor(Color start, Color end)
    {
        mat.SetColor("_Bot",start);
        mat.SetColor("_Top", end);
    }
    public void setTone(Color color)
    {
        mat.SetColor("_Tone", color);
    }
}
