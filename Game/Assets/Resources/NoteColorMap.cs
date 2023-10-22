using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NoteColorPreset", menuName = "Scriptable Objects/Color Preset", order = 1)]
public class NoteColorMap : ScriptableObject
{
    public string presetName;
    [SerializeField]
    C[] colors;
    [SerializeField]
    C[] longColorStart;
    [SerializeField]
    C[] longColorEnd;
    [SerializeField]
    public float multiplier = 1F;
    public NoteColorMap() 
    {
        colors = new C[6];
        longColorStart = new C[6];
        longColorEnd = new C[6];

        for (int w=0;w<6; w++)
        {
            colors[w] = new C();
            colors[w].colors = new Color[6-w];
            longColorStart[w] = new C();
            longColorStart[w].colors = new Color[6 - w];
            longColorEnd[w] = new C();
            longColorEnd[w].colors = new Color[6 - w];
            for (int i=0;i<6-w; i++)
            {
                var data = new NoteData();
                data.x = w;
                data.y = i;
                colors[w].colors[i] = Utility.DefaultColor(data);
                longColorStart[w].colors[i] = Utility.DefaultLineColorStart(data);
                longColorEnd[w].colors[i] = Utility.DefaultLineColorEnd(data);
            }
        }

    }

    public Color Get(int width,int index)
    {
        return colors[width].colors[index];
    }

    public Color GetLongEnd(int width,int index)
    {
        return longColorEnd[width].colors[index];
    }
    public Color GetLongStart(int width,int index)
    {
        return longColorStart[width].colors[index];
    }

    [Serializable]
    class C
    {
        public Color[] colors;
    }
}
