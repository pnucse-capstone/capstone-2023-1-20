using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Curves", menuName = "Scriptable Objects/Curves")]
class Curves : ScriptableObject
{
    public AnimationCurve None;
    public AnimationCurve Linear;
    public AnimationCurve EaseInOut;
    public AnimationCurve EaseIn;
    public AnimationCurve EaseOut;
}
public static class InterpolationCurve 
{
    static Curves curves;
    public static AnimationCurve Get(int code)
    {
        switch (code)
        {
            case 0:return curves.None;
            case 1:return curves.Linear;
            case 2:return curves.EaseInOut;
            case 3:return curves.EaseIn;
            case 4:return curves.EaseOut;
            default: throw new ArgumentException("유효하지 않은 보간 곡선 코드입니다");
        }
    }
    static InterpolationCurve()
    {
        curves = Resources.Load("Curves") as Curves;
    }
    // Start is called before the first frame update
}
