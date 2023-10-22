using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Steamworks;
using UnityEngine.Profiling;
using Async;
using static JudgeInfo;
using static FadeScreen;
using static UnityEngine.RectTransform;
using System.CodeDom.Compiler;

public class InputApply : MonoBehaviour
{
    //    public static Queue<float> tap_queue;
    NoteJudger judger;
    InputState input; 
    void Start()
    {
        judger = new NoteJudger();
        input = Game.state.input;
        input.Start();
        StartCoroutine(MissCheck());
    }
    private void OnDestroy()
    {
        input.Release();
    }
    public void reset()
    {
        StopAllCoroutines();
        StartCoroutine(MissCheck());
    }
    List<Note> notes = new List<Note>();
    void Update()
    {

        if (!Game.isPlaying) return;
        /* 대상 노트 추출 */
        Profiler.BeginSample("GetAllNotes");
        GamePreprocessor.FindAllNotes(x => !x.isComplete() && x.inTimeBoundary(), ref notes);
        Profiler.EndSample();
        Profiler.BeginSample("Input Update");
        input.Update();
        Profiler.EndSample();
        Profiler.BeginSample("Judge");
        judger.Judge(notes,input);
        Profiler.EndSample();
    }
    IEnumerator MissCheck()
    {
        Debug.Log("MISS CHECK 시작");
        int cnt = 0;
        foreach (var i in NoteEditor.tableNow.getAllNotes((x) => true))
        {
            cnt++;
            while (!i.isPassed() && !i.isComplete())
            {
                yield return null;
            }
            if (i.isPassed() && !i.isComplete())
            {
                i.JudgeToMiss();
            }
        }
    }
    
    

}
public class NoteJudger
{

    public void Judge(List<Note> notes,InputState input) // 주어진 입력 상태에 대해 노트들에 입력을 처리함
    {
        if (notes == null) return;
        /* 대상 노트 처리 */
        notes.Sort(NoteType.Cmp);
        if (Game.autoplay)
        {
            foreach(var i in notes)
            {
                if (i.data.time <= Game.time)
                {
                    if(i.state.length == 0)
                    {
                        NoteBegin(i, Game.time); 
                    }
                    else
                    {
                        NoteOn(i, Game.time);
                    }
                    //판정을 시작한다
                }
                if (Game.time> i.data.time+i.data.length)
                {
                    NoteComplete(i,JudgeType.perfect,Game.time);
                    //판정을 끝낸다
                }
            }
        }
        else
        {
            while (input.NextState())
            {
                Profiler.BeginSample("Soft");
                SoftBegin(notes, input);
                Profiler.EndSample();
                Profiler.BeginSample("Begin");
                CastBegin(notes, input);
                Profiler.EndSample();
                Profiler.BeginSample("On");
                CastOn(notes, input);
                Profiler.EndSample();
                Profiler.BeginSample("Complete");
                Complete(notes, input);
                Profiler.EndSample();
            }
        }
    }
    private void SoftBegin(List<Note> notes, InputState input)
    {

        foreach(var note in notes)
        {
            for (int i = 0; i < Game.lineCount; i++)
            {
                if (input.isOn(i) && ConditionBegin(note,i) && note.Type().IsSoft)
                {
                    NoteBegin(note, input.time);
                }
            }
        }
    }
    private void Complete(List<Note> notes,InputState input) // 판정 조건을 만족하면 그냥 판정처리.. 끝까지 누른 경우 대응
    {
        foreach (var note in notes)
        {
            if(note.state.length > 0)
            {
                if (note.state.length >= note.data.length)
                {
                    NoteComplete(note, note.state.judge_result, input.time);
                }
                else if (note.state.judge_result == JudgeType.miss)
                {
                    NoteComplete(note, note.state.judge_result, input.time);
                }
                else if (note.data.length <= note.state.length + Time.deltaTime*3)
                {
                    NoteComplete(note, note.state.judge_result, input.time);
                }
            }
        }
    }


    private void CompleteNotOn(List<Note> notOn,InputState input) // 판정 조건을 불만족하면 미스 처리
    {
        foreach (var note in notOn)
        {
            if (note.state.length > 0 && note.data.time+0.001F < Game.time)
            {
                if (note.GetProgressRate()<1F)
                {
                    Debug.Log("NotOn Miss:"+input.time+","+note.data.time+","+note.state.length);
                    NoteComplete(note, JudgeType.miss, input.time);
                }
                else 
                {
                    NoteComplete(note, note.state.judge_result, input.time);
                }
            }
        }
    }

    static void Filter(Func<Note, bool> predicate, List<Note> from, ref List<Note> list)
    {
        list.Clear();
        for (int i = 0; i < from.Count; i++)
        {
            var note = from[i];
            if (predicate(note))
            {
                list.Add(note);
            }
        }
    }
    List<Note> notOn = new List<Note>();

    private List<Note> CastOn(List<Note> notes, InputState input) // 중간에 조건을 벗어나는가를 체크해서 판정 처리
    {
        Filter((x) => x.state.length != 0
        && x.Type().judge_info.JudgeOn(input.time, x.data.time, x.data.length)
        , notes, ref notOn);  
        notOn.Sort(OnCmp);
        for (int i = 0; i < Game.lineCount; i++)
        {
            if (input.isOn(i))
            {
                var note = CastLine(notOn, i, ConditionOn);
                notOn.Remove(note);
                NoteOn(note, input.time);
                BottomLineEffecter.Effect(i / 5F);
            }
        }
        CompleteNotOn(notOn,input);
        return notOn;
    }

    List<Note> castBeginNotes = new List<Note>();
    private void CastBegin(List<Note> source, InputState input)
    {
        Filter((x) => true, source, ref castBeginNotes);
        castBeginNotes.Sort(BeginCmp); 
        for (int i = 0; i < Game.lineCount; i++)
        {
            if (input.isBegin(i))
            {
                var note = CastLine(castBeginNotes, i, ConditionBegin);
                castBeginNotes.Remove(note);
                NoteBegin(note, input.time);

            }
        }
    }
    public bool isInclude(Note a,Note b) //a가 b에 포함되는지
    {
        var l1 = a.data.y;
        var r1 = a.data.y +a.data.x;
        var l2 = b.data.y;
        var r2 = b.data.y + b.data.x;
        return (l2 <= l1 && r1 <= r2);
    }
    public int BeginCmp(Note x, Note y) //폭우선 정렬 
    {
        if (Cmp(x.data.time, y.data.time) != 0)
        {
            return Cmp(x.data.time, y.data.time);
        }


        if (isInclude(x, y))
        {
            return -1;
        }
        if (isInclude(y, x))
        {
            return 1;
        }

        if (Cmp(x.data.y, y.data.y) != 0)
        {
            return Cmp(x.data.y, y.data.y);
        }

        if (Cmp(x.data.x, y.data.x) != 0)
        {
            return Cmp(x.data.x, y.data.x);
        }


        if (Cmp(x.data.length, y.data.length) != 0)
        {
            return Cmp(x.data.length, y.data.length);
        }

        return Cmp(x.data.dx, y.data.dx);
    }
    public int OnCmp(Note x, Note y) //폭우선 정렬 
    {
        if (x.GetProgressRate()>=1 && y.GetProgressRate()<1)
        {
            return 1;
        }
        if (x.GetProgressRate() < 1 && y.GetProgressRate() >= 1)
        {
            return -1;
        }
        if (isInclude(x, y))
        {
            return -1;
        }
        if (isInclude(y, x))
        {
            return 1;
        }
        if (Cmp(x.data.y, y.data.y) != 0)
        {
            return Cmp(x.data.y, y.data.y);
        }
        if (Cmp(x.data.x, y.data.x) != 0)
        {
            return Cmp(x.data.x, y.data.x);
        }


        if (Cmp(x.data.time, y.data.time) != 0)
        {
            return -Cmp(x.data.time, y.data.time);
        }
        if (Cmp(x.data.length, y.data.length) != 0)
        {
            return Cmp(x.data.length, y.data.length);
        }

        return Cmp(x.data.dx, y.data.dx);
    }

    private static int Cmp(float x, float y)
    {
        if (Mathf.Abs(x - y) < 0.001) return 0;
        return Math.Sign(x - y);
    }
    void NoteBegin(Note note,float time)
    {
        if (note == null) return;
        JudgeType judge = note.Type().judge_info.JudgeBegin(time, note.data.time);
        if (judge != JudgeType.OUT)
        {
            note.state.judge_result = judge; // 머리 판정을 기록
            note.state.length = Mathf.Max(time - note.data.time, 0.0001F);
            BottomLineEffecter.Attack();

            //머리 이펙트, 단노트는 한번만 해야하므로...
            if (note.data.length != 0F)
            {
                ScoreBoard.Write(judge);
                ScoreEffect.Effect(judge);
                note.SoundStart();
                EffectRender.makeNoteEffect(note, judge, NoteStep.BEGIN);
                ScoreEffect.ShowEarlyLate(time - note.data.time);

            }
            else
            {
                note.SoundStart();
                EffectRender.makeNoteEffect(note, judge, NoteStep.BEGIN);
                ScoreEffect.ShowEarlyLate(time - note.data.time );
                NoteComplete(note,judge,time);
            }

        }
    }
    void NoteOn(Note note,float time)
    {
        if (note == null) return;
         if (note.state.length != 0)
        {
            EffectRender.makeNoteEffect(note, note.state.judge_result, NoteStep.ON);
            note.state.length = Mathf.Max(time - note.data.time, 0.0001F);
        }
    }
    Note CastLine(List<Note> notes,int line, Func<Note,int,bool> predicate)
    {
        foreach(var i in notes)
        {
//            if(i.GetProgressRate()>1)
//            Debug.Log((i.data.length, i.state.length));
        }
        if (notes.Exists((x)=>predicate(x, line))){
            var hit = notes.First((x) => predicate(x, line));
            return hit;
        }
        return null;
    }
    bool ConditionBegin(Note note,int line)
    {
        bool cond = note.Type().Condition(Game.state, note, line);
        bool start = note.state.length == 0;
        bool valid = cond && start;
        return valid;
    }
    bool ConditionOn(Note note, int line)
    {
        bool cond = note.Type().Condition(Game.state, note, line);
        return cond;
    }


    void NoteComplete(Note note, JudgeType judgeType, float time) // 현재 상태를 기반으로 판정을 적용하여 처리를 완료함
    {
        if (note == null) return;
        if (note.state.judge) return;
        note.state.judge_result = judgeType;
        if (note.state.judge_result == JudgeType.miss && note.data.length !=0)
        {
            note.SoundEnd();
        }
        note.state.judge = true;
        ScoreBoard.Write(note.state.judge_result);
        EffectRender.makeNoteEffect(note, note.state.judge_result, NoteStep.END);
        ScoreEffect.Effect(note.state.judge_result);
    }

}

public enum JudgeType { perfect, good, ok, miss, Count, OUT, NONE };
public class JudgeInfo // 각 판정에 대한 시간 판정 범위에 값을 저장.
{
    public enum JudgeTime { on, passed, yet, Count }
    List<JudgeDt> dts;
    List<JudgeType> types;
    public JudgeInfo()
    {
        dts = new List<JudgeDt>();
        types = new List<JudgeType>();
    }
    float front_bound = 0;
    float back_bound = 0;
    public void Add(JudgeType type, JudgeDt dt)
    {
        if (dt.front < front_bound) front_bound = dt.front;
        if (dt.back > back_bound) back_bound = dt.back;
        dts.Add(dt);
        types.Add(type);
        dts[(int)type] = dt;
    }
    public List<JudgeType> getJudgeTypes()
    {
        return types;
    }
    public JudgeType JudgeBegin(float now, float note_time)
    {
        for (int i = 0; i < dts.Count; i++)
        {
            if (dts[i].Check(now, note_time)) return types[i];
        }
        return JudgeType.OUT;
    }
    public bool JudgeOn(float now, float note_time, float note_length)
    {
        float left = note_time + front_bound;
        float right = note_time + note_length;
        return left <= now && now <= right;
    }
}
public struct JudgeDt
{
    public float front; //항상 음수
    public float back; // 항상 양수
    public JudgeDt(float front, float back)
    {
        this.front = front;
        this.back = back;
    }
    public static JudgeDt operator /(JudgeDt a, float b)
    {
        if (b == 0)
        {
            throw new DivideByZeroException();
        }
        return new JudgeDt(a.front / b, a.back / b);
    }
    public bool Check(float now, float note_time)
    {
        return (now - note_time) > front*Game.playSpeed && (now - note_time) < back * Game.playSpeed;
    }

}