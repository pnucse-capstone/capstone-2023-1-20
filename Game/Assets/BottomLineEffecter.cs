using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomLineEffecter : MonoBehaviour
{
    // Start is called before the first frame update
    static float[] values=  new float[256];
    [SerializeField]
    Material material;
    Color idleColor;
    Color attackColor = Color.white;
    static float attack = 0;
    void Start()
    {
        material = gameObject.GetComponent<Renderer>().material;
        material.SetInt("_BarAmount", 256);
        idleColor = Color.black;
//        idleColor  = Color.HSVToRGB(0.5F, (float)Math.Sin(Game.time), 1F);
        idleColor.a = 0.5F;
        
    }
    public static void Attack()
    {
        attack = 1F;
    }
    public static void Effect(float t)
    {

        float center = 216*t+20;
        for (int i=0;i<values.Length; i++)
        {
            values[i] += 50*Time.deltaTime*Mathf.Exp(-(i - center)* (i - center)/200);
            values[i] = Mathf.Clamp(values[i], 0.1F, 1);
        }
    }
    // Update is called once per frame
    void Update()
    {
        //DAMP
        for (int i = 1; i < values.Length-1; i++)
        {
            values[i] = Mathf.Max(values[i] -2*Time.deltaTime, 0.1F);
        }
        attack = Mathf.Max(0, attack-Time.deltaTime);
        material.SetFloatArray("_Value", values);

        var nowColor = Color.Lerp(idleColor, attackColor, attack);
        material.SetColor("_Color", nowColor);

    }
}
