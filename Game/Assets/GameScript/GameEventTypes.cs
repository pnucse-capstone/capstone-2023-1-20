using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Profiling;

public abstract class GameEventT // 리플렉션과 비슷하게 string으로 각 이벤트 별로 get set을 구현함. 다만 미리 캐싱하든 해서 빠르게...
{

    public abstract string field { get; }
    private Array temp;
    public Array GetTemp(int Count)
    {
        if(temp == null || temp.Length != Count)
        {
            temp = CreateTemp(Count);
        }
        return temp;
    }
    FieldInfo fieldInfo;
    public readonly int Count = 1;
    abstract public Array CreateTemp(int Length);
    public GameEventT()
    {
        var defaultEvent = new EventData();
        fieldInfo = typeof(EventData).GetField(field);
    }
    virtual public void Set(EventData data, Array values) 
    {
        if (values == null || values.Length == 0)
        {
            
        }
        else if (values.Length == 1)
        {
            fieldInfo.SetValue(data, values.GetValue(0));
        }
        else
        {
            fieldInfo.SetValue(data, values);
        }
    }
    virtual public Array Get(EventData data)
    {
        if (fieldInfo == null) return null;
        bool isArray = fieldInfo.FieldType.IsArray;
        if (isArray)
        {
            var arr=(Array)fieldInfo.GetValue(data);
            return arr;
        }
        else
        {
            Array list = Array.CreateInstance(fieldInfo.FieldType, 1);
            list.SetValue(fieldInfo.GetValue(data), 0);
            return list;
        }
    }
    abstract public void Lerp(Array a, Array b, float t, ref Array output);

}
public class UseGameEvent : Attribute
{

}
abstract public class GameEventFloat : GameEventT
{
    float[] temp;
    public override Array CreateTemp(int Length)
    {
        return new float[Length];
    }
    public override void Lerp(Array a, Array b, float t, ref Array output)
    {
        var now = (float[])a;
        var next = (float[])b;
        var o = (float[])output;
        var length = Mathf.Min(a.Length, b.Length);
        Debug.Assert(length <= output.Length);
        for(int i=0;i<length; i++)
        {
            o[i] = Mathf.Lerp(now[i],next[i],t);
        }
    }

}
abstract public class GameEventDropdown : GameEventT
{
    float[] temp;
    public override Array CreateTemp(int Length)
    {
        return new float[Length];
    }
    public override void Lerp(Array a, Array b, float t, ref Array output)
    {
        var now = (float[])a;
        var next = (float[])b;
        var o = (float[])output;
        var length = Mathf.Min(a.Length, b.Length);

        Debug.Assert(length <= output.Length);
        Debug.Assert(now[0] == now[1] && now[2]==0);
        Debug.Assert(next[0] == next[1] && next[2] == 0); // 정규화됨


        o[0] = now[0];
        o[1] = next[0];
        o[2] = t;
    }

}
abstract public class GameEventPath : GameEventT
{
    LinePath[] temp;
    public override Array CreateTemp(int Length)
    {
        return new LinePath[Length];
    }
    public override void Lerp(Array a, Array b, float t, ref Array output)
    {
        var now = (LinePath[])a;
        var next = (LinePath[])b;
        var o = (LinePath[])output;
        int length = Mathf.Min(a.Length, b.Length);
        Debug.Assert(a.Length <= b.Length && length <= output.Length);
        for (int i = 0; i < length; i++)
        {
            o[i] = LinePath.Lerp(now[i], next[i], t);
        }
    }
}
abstract public class GameEventColor :GameEventT
{
    ColorAdapter[] temp;
    public override Array CreateTemp(int Length)
    {
        return new ColorAdapter[Length];
    }
    public override void Lerp(Array a, Array b, float t, ref Array output)
    {
        int length = Mathf.Min(a.Length, b.Length);
        Debug.Assert(length <= b.Length && length <= output.Length);
        if (a is Color[])
        {
            var now = (Color[])a;
            var next = (Color[])b;
            var o = (Color[])output;
            for (int i = 0; i < length; i++)
            {
                o[i] = Color.Lerp(now[i], next[i], t);
            }
        }
        else if (a is ColorAdapter[])
        {
            var now = (ColorAdapter[])a;
            var next = (ColorAdapter[])b;
            var o = (ColorAdapter[])output;
            for (int i = 0; i < length; i++)
            {
                o[i] = Color.Lerp(now[i], next[i], t);
            }
        }
        else throw new NotImplementedException();
    }
}
[UseGameEvent]
public class SpeedGameEvent : GameEventFloat
{
    public SpeedGameEvent() { }

    public override string field => "speed";

}
[UseGameEvent]
public class Speed2GameEvent : GameEventFloat
{
    public Speed2GameEvent() { }
    public override string field => "speed2";


}


[UseGameEvent]
public class EditorBPMGameEvent : GameEventFloat
{
    public EditorBPMGameEvent() { }
    public override string field => "bpm";
}

[UseGameEvent]
public class BgcolorGameEvent : GameEventColor
{
    public BgcolorGameEvent() { }
    public override string field => "bgColor";
}
[UseGameEvent]
public class Bgcolor2GameEvent : GameEventColor
{
    public Bgcolor2GameEvent() { }
    public override string field => "bgColor2";
}
[UseGameEvent]
public class MultiplyNoteColorGameEvent : GameEventColor
{
    public MultiplyNoteColorGameEvent() { }
    public override string field => "multiplyNoteColor";
}
[UseGameEvent]
public class AdditiveNoteColorGameEvent : GameEventColor
{
    public AdditiveNoteColorGameEvent() { }
    public override string field => "additiveNoteColor";
}
[UseGameEvent]
public class WidthGameEvent : GameEventFloat
{
    public WidthGameEvent() { }
    public override string field => "width";
}
[UseGameEvent]
public class WidthsGameEvent : GameEventFloat
{
    public WidthsGameEvent() { }
    public override string field => "widths";
}
[UseGameEvent]
public class PositionGameEvent : GameEventFloat
{
    public PositionGameEvent() { }
    public override string field => "position";
}
[UseGameEvent]
public class LineColorGameEvent : GameEventColor
{
    public LineColorGameEvent() { }
    public override string field => "lineColors";

}
[UseGameEvent]
public class PositionYGameEvent : GameEventFloat
{
    public PositionYGameEvent() { }
    public override string field => "positionY";

}

[UseGameEvent]
public class RotationGameEvent : GameEventFloat
{
    public RotationGameEvent() { }
    public override string field => "rotation";
}

[UseGameEvent]
public class InterpolationCurveGameEvent : GameEventFloat
{
    public InterpolationCurveGameEvent() { }
    public override string field => "interpole";
}
[UseGameEvent]
public class NoteImageGameEvent : GameEventDropdown
{
    public NoteImageGameEvent() { }
    public override string field => "noteImage";
}
[UseGameEvent]
public class PositionCamGameEvent: GameEventFloat
{
    public PositionCamGameEvent() { }
    public override string field => "positionCam";
}
[UseGameEvent]
public class RotationCamGameEvent : GameEventFloat
{
    public RotationCamGameEvent() { }
    public override string field => "rotationCam";
}

[UseGameEvent]
public class ZoomCamGameEvent : GameEventFloat
{
    public ZoomCamGameEvent() { }
    public override string field => "zoomCam";
}

[UseGameEvent]
public class NoteWidthGameEvent : GameEventFloat
{
    public NoteWidthGameEvent() { }
    public override string field => "noteWidth";
}
[UseGameEvent]
public class NoteHeightGameEvent : GameEventFloat
{
    public NoteHeightGameEvent() { }
    public override string field => "noteHeight";
}
[UseGameEvent]
public class NotePosXGameEvent : GameEventFloat
{
    public NotePosXGameEvent() { }
    public override string field => "notePosX";
}
[UseGameEvent]
public class LineWidthGameEvent : GameEventFloat
{
    public LineWidthGameEvent() { }
    public override string field => "lineWidth";
}
[UseGameEvent]
public class LineCenterHeightGameEvent : GameEventFloat
{
    public LineCenterHeightGameEvent() { }
    public override string field => "lineCenterHeight";
}
[UseGameEvent]
public class LineCenterWidthGameEvent : GameEventFloat
{
    public LineCenterWidthGameEvent() { }
    public override string field => "lineCenterWidth";
}
[UseGameEvent]
public class LineLengthGameEvent : GameEventFloat
{
    public LineLengthGameEvent() { }
    public override string field => "lineLength";
}

[UseGameEvent]
public class PathTGameEvent : GameEventPath
{
    public PathTGameEvent() { }
    public override string field => "linePath";
}
[UseGameEvent]
public class NotePathGameEvent : GameEventPath
{
    public NotePathGameEvent() { }
    public override string field => "notePath";
}

[UseGameEvent]
public class LineColorMGameEvent : GameEventColor
{
    public LineColorMGameEvent() { }
    public override string field => "lineColorM";
}
[UseGameEvent]
public class LineColorAGameEvent : GameEventColor
{
    public LineColorAGameEvent() { }
    public override string field => "lineColorA";
}
[UseGameEvent]
public class InvertEvent : GameEventFloat
{
    public InvertEvent() { }
    public override string field => "invert";
}
[UseGameEvent]
public class invertLongEvent : GameEventFloat
{
    public invertLongEvent() { }
    public override string field => "invertLong";
}
[UseGameEvent]
public class SkipIntervalEvent : GameEventFloat
{
    public SkipIntervalEvent() { }
    public override string field => "skip";
}

[UseGameEvent]
public class NoteColorsetEvent : GameEventDropdown
{
    public NoteColorsetEvent() { }
    public override string field => "noteColorset";
}
