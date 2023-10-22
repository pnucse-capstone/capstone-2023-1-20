using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEditor;
using System.Reflection;
using Newtonsoft.Json;

public enum NoteStep { BEGIN, ON , END};
[Serializable]
public class Note
{
    FMOD.Channel SoundChannel;
    public int note_type_code = 0;
    public NoteData data;
    public NoteState state = new NoteState();

    public Vector3 Position(float offset = 0 ) 
        // 노트로 부터 offset 지점의 좌표를 반환. 롱노트 위의 한 점을 반환한다 생각하면 됨.
    {
        return Type().Position(Game.state,this,offset);
    }
    public LinePath.Point[] NotePath()
    {
        return Type().LongNotePositions(Game.state,this);
    }
    public float GetProgressRate()
    {

        float p = state.length;
        float l = data.length - 0.15F;
        
        if (l<= 0)
        {
            return state.length == 0 ? 0 : 1;
        }
        return p / l;
    }
    public bool isComplete()
    {
        return state.judge;
    }
    public void Reset()
    {
        state.reset();
    }
    public bool inTimeBoundary()
    {
        return Type().InTimeBoundary(this);
    }
    public bool isInScreen()
    {
        // 코헨 서더랜드 알고리즘 변형
        Vector3 pos1 = GamePreprocessor.mainCam.WorldToViewportPoint(Position());
        Vector3 pos2 = GamePreprocessor.mainCam.WorldToViewportPoint(Position(data.length));
        int code1 = 
            (pos1.x < 0 ? 1 : 0) |
            (pos1.x > 1 ? 2 : 0) |
            (pos1.y < 0 ? 4 : 0) |
            (pos1.y > 1 ? 8 : 0);
        int code2 =
            (pos2.x < 0 ? 1 : 0) |
            (pos2.x > 1 ? 2 : 0) |
            (pos2.y < 0 ? 4 : 0) |
            (pos2.y > 1 ? 8 : 0);
        return code1 == 0 || code2 == 0 || (code1 & code2) == 0; 
    }
    public NoteType Type()
    {
        
        return Game.mechanim.noteType(data.type);
    }

    public void Ignore()
    {
        state.judge = true;
    }

    public Note(NoteData data)
    {
        this.data = data; 
    }
    public bool isPassed()
    {
        bool isPassed = Type().judge_info.JudgeBegin(Game.time,data.time) == JudgeType.OUT && data.time<Game.time;
        return isPassed;
    }
    public void JudgeToMiss()
    {
        if (state.judge_result == JudgeType.NONE)
        {
            state.judge_result = JudgeType.miss;
            state.judge = true;
            ScoreBoard.Write(state.judge_result);
            ScoreEffect.Effect(state.judge_result);
        }
    }
    public Quaternion Rotation()
    {
        return Type().Rotation(Game.state, this);
    }


    public Color NoteColor()
    {
        return Type().NoteColor(Game.state, this);
    }
    public Vector3 Scale()
    {
        return Type().Scale(Game.state, this);
    }
    
    public float LongNoteWidth()
    {
        return Type().LongNoteWidth(Game.state, this);
    }
    public Color LongNoteColor(int code)
    {
        return Type().LongNoteColor(Game.state, this, code);
    }

    public void SoundStart()
    {
        SoundChannel = KeySoundPallete.FMODPlay(data.key);
    }

    public void SoundEnd()
    {
        SoundInfo info = KeySoundPallete.GetSoundInfo(data.key);
        if ((info != null))
        {
            SoundChannel.stop();
        }
    }
    public Sprite NoteImage()
    {
        return StreamingImageAssets.GetNoteImage(Type().imageCode());
    }
    internal bool Condition(int input_line)
    {
        return Type().Condition(Game.state, this, input_line);
    }
}
[Serializable]
public class NoteData
{
    public int index;

    public float time = 0;
    public float length =0;

    public int type = 0;

    public float vx= 1;
    public float y= 0;
    public float dy = 0;
    
    public int dx = 0;
    public int x = 0;

    public int key = 0; //키음. 0일때 기본음 재생

    public int z = 0;

    public float posx = 0;
    public float posy = 0;
    public float scalex = 1;
    public float scaley = 1;

    public float invert = 0;
    public float invertLong = 0;
    public ColorAdapter color;

    public ColorAdapter lineColorStart;
    public ColorAdapter lineColorEnd;

    public bool useDefaultColor = true;
    public NoteData(NoteData data)
    {
        var fields = typeof(NoteData).GetFields();
        foreach(var i in fields)
        {
            i.SetValue(this,i.GetValue(data));
        }
    }

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive || type.IsEnum || type.Equals(typeof(string));
    }

    public bool Equals(NoteData data)
    {
        var a = JsonConvert.SerializeObject(data);
        var b = JsonConvert.SerializeObject(this);
        return a == b;
    }
    public NoteData() { }
    public NoteData(float time = 0, uint length = 0)
    {
        this.time=time;
        this.length = length;
        vx = 1;
    }
};
public class NoteState
{
    public bool judge = false;
    public float length = 0F;

    public JudgeType judge_result = JudgeType.NONE;

    public bool hide;
    public void cancel()
    {
        if(length >0 && !judge)
        {
            ScoreBoard.Erase(judge_result);
        }
        reset();
    }
    public void reset()
    {
        judge = false;
        length = 0;
        judge_result = JudgeType.NONE;

        hide = false;
    }
}