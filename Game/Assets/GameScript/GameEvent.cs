using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[Serializable]
public class EventData
{
    public static List<string> Type { get => types.Select((x) => x.field).ToList(); }
    static List<GameEventT> types;
    static EventData()
    {
        types = new List<GameEventT>();
        var arr = (from i in Assembly.Load("Assembly-CSharp").GetTypes()
                   where i.GetCustomAttributes().Any((x) => x is UseGameEvent)
                   select i).ToArray();
        foreach (var i in arr)
        {
            types.Add(Activator.CreateInstance(i) as GameEventT);
        }
    }
    public static GameEventT GetGameEventType(string eventType)
    {
        return types.Find((x) => x.field == eventType);
    }
    public static List<GameEventT> GetGameEventTypes()
    {
        return types;
    }

    public Dictionary<string, bool> use;
    public float time = 0;

    public float speed = 1;
    public float speed2 = 1;
    public float width = 1;
    public float[] widths = new float[16] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
    public float bpm = 120;
    public float[] position = new float[Game.lineCount];
    public ColorAdapter bgColor = Color.black;
    public ColorAdapter multiplyNoteColor = Color.white;
    public ColorAdapter additiveNoteColor = Color.black;

    public ColorAdapter lineColorM = Color.white;
    public ColorAdapter lineColorA = Color.black;

    public ColorAdapter[] lineColors = new ColorAdapter[Game.lineCount];
    public float[] positionY = new float[Game.lineCount];
    public float[] rotation = new float[Game.lineCount];
    public float rotationCam = 0;
    public float zoomCam = 5F;

    public float invert = 0;
    public float invertLong = 0;
    public float skip = 0;
    public ColorAdapter bgColor2 = Color.black;

    public float[] positionCam = new float[2] { 0, 4 };
    public float interpole = 0;
    public float[] noteWidth = new float[16] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
    public float[] noteHeight = new float[16] { 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F };
    public float[] notePosX = new float[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public float[] lineWidth = new float[16] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
    public float[] lineCenterWidth = new float[16] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
    public float[] lineCenterHeight = new float[16] { 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F, 0.2F };
    public float[] lineLength = new float[16] { 2F, 2F, 2F, 2F, 2F, 2F, 2F, 2F, 2F, 2F, 2F, 2F, 2F, 2F, 2F, 2F };

    public void SetLineCount(int cnt)
    {
        Resize(ref noteWidth, cnt, 1);
        Resize(ref noteHeight, cnt, 0.2F);
        Resize(ref notePosX, cnt, 0);
        Resize(ref lineWidth, cnt, 1);
        Resize(ref lineCenterWidth, cnt, 1);
        Resize(ref lineCenterHeight, cnt, 0.2F);
        Resize(ref lineLength, cnt, 0.2F);
        Resize(ref lineColors, cnt, new ColorAdapter(1, 1, 1));
        Resize(ref linePath, cnt, LinePath.Default);
        Resize(ref notePath, cnt, LinePath.Default);
    }
    void Resize<T>(ref T[] arr, int cnt, T defaultValue)
    {
        if (arr == null)
        {
            arr = new T[] { };
        }
        int prev_cnt = arr.Length;
        Array.Resize(ref arr, cnt);
        if (prev_cnt < cnt)
        {
            for (int i = prev_cnt; i < cnt; i++)
            {
                arr[i] = defaultValue;
            }
        }
    }

    public LinePath[] linePath;
    public float[] noteImage = new float[] { 0, 0, 0, };
    public float[] noteColorset = new float[] { 0, 0, 0, };

    public LinePath[] notePath;
    public EventData()
    {
        SetLineCount(Game.lineCount);
        for (int i = 0; i < Game.lineCount; i++)
        {
            lineColors[i] = Color.white;
        }
        use = new Dictionary<string, bool>();
        foreach (var i in Type)
        {
            use.Add(i, false);
        }
        use["interpole"] = true;
    }
    public EventData Clone()
    {
        return JsonConvert.DeserializeObject<EventData>(JsonConvert.SerializeObject(this));
    }
    public void Set(EventData data)
    {
        System.Type t = typeof(EventData);
        var fields = t.GetFields();
        foreach (var pi in fields)
        {
            pi.SetValue(this, pi.GetValue(data));
        }
    }
    public void Use(string type, bool value)
    {
        Resize();
        use[type] = value;
    }
    public bool isUse(string type)
    {
        Resize();
        return use[type];
    }
    void Resize()
    {
        foreach (var i in Type)
        {
            if (!use.ContainsKey(i))
            {
                use.Add(i, false);
            }
        }
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    internal void Reset()
    {
        Set(new EventData());
    }
}

public class GameState
{
    public InputState input;
    public InputMap inputmap;
    public EventData properties = new EventData();
    public TimeDistance timedistance;


    public Vector3 LineScale(int input_line)
    {
        return Game.mechanim.lineType.LineCenterScale(Game.state, input_line);
    }
    public GameState()
    {
        inputmap = new InputMap();
        inputmap.Reset();
        bool mode = PlayerPrefs.GetInt("asyncInput", 1) == 1;
        if (mode)
        {
            input = new InputStateLagacy(inputmap);
        }
        else
        {
            input = new InputStateLagacy(inputmap);
        }
    }

}

