using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JudgementDetail 
{
    static List<(float time,float offset)> lists = new List<(float time,float offset)> ();
    public static void Reset()
    {
        lists.Clear();
    }
    public static void Add(float time, float offset)
    {
        lists.Add((time, offset));
    }
    public static void GetList()
    {

    }
}
