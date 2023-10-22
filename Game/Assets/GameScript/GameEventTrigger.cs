using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Profiling;
using System;

public class GameEventTrigger : MonoBehaviour
{
    GameEventManager tray;
    // Start is called before the first frame update
    public EventData Search(string field, float time,bool get_next)
    {
        return tray.Search(field,time,get_next);
    }
    void Start()
    {
        tray = new GameEventManager(NoteEditor.tableNow.getEvents());
    }
    void Update()
    {
        tray.Update();
    }



}

public class GameEventTimeline
{
    public string field
    {
        get => type.field;
    }
    GameEventT type;
    List<Keyframe> keyframes;
    public class Keyframe
    {
        public float time;
        public Array data;
        public float interpole;
        public EventData e;
    }
    public GameEventTimeline(GameEventT type)
    {
        this.type = type;

    }
    public bool isUsed
    {
        get; set;
    }
    public void Init(List<EventData> events)
    {

        events = (from item in events
                  where item.isUse(type.field)
                  orderby item.time ascending
                  select item).ToList();
        if (!events.Exists((x) => x.time == 0)  && events.Count != 0)
        {
            isUsed = true;
            events.Add(new EventData());
        }
        else if (!events.Exists((x) => x.time == 0) && events.Count == 0)
        {
            isUsed = false;
            events.Add(new EventData());
        }
        else
        {
            isUsed = true;
        }
        events.Sort((x, y) => (int)Mathf.Sign(x.time - y.time));
        this.keyframes = events.Select((x)=>new Keyframe() {time = x.time,data = type.Get(x),interpole = x.interpole,e=x}).ToList();
        Debug.Assert(this.keyframes[0].time == 0);
        Debug.Assert(this.keyframes[0].data != null);

    }
    protected Keyframe nowKeyframe
    {
        get => searchKeyframe(keyframes, Game.time, false);
    }
    protected Keyframe nextKeyframe
    {
        get => searchKeyframe(keyframes, Game.time, true);
    }
    public EventData Search(float time, bool get_next)
    {
        return searchKeyframe(keyframes, time, get_next).e;
    }
    Keyframe searchKeyframe(List<Keyframe> L, float target, bool get_next = false) // 비교함수는 오름차순일 때 x<y을 반환
    {
        if (L.Count == 0) return null;
        
        int left = 0;
        int right = L.Count - 1;
        while (left != right)
        {
            int mid = (left + right + 1) / 2;
            if (target < L[mid].time) right = mid - 1;
            else left = mid;
        }
        if (!get_next)
        {
            return L[Mathf.Max(left, 0)];
        }
        else return L[Mathf.Min(left + 1, L.Count - 1)];
    }
    public void Update()
    {
        if (keyframes.Count == 0) return;

        Apply(Game.time,ref Game.state.properties);
    }

    public void Apply(float time,ref EventData ev)
    {
        var a = nowKeyframe.data;
        var b = nextKeyframe.data;
        var t = (nextKeyframe.time == nowKeyframe.time) ? 0 : (time - nowKeyframe.time) / (nextKeyframe.time - nowKeyframe.time);

        var o = type.GetTemp(a.Length);
        t = InterpolationCurve.Get(Mathf.RoundToInt(nextKeyframe.interpole)).Evaluate(t);
        type.Lerp(a, b, t, ref o);
        type.Set(ev, o);
    }
}
public class GameEventManager
{
    List<GameEventTimeline> timelines;
    public GameEventManager(List<EventData> events)
    {
        timelines = new List<GameEventTimeline>();

        foreach (var i in EventData.GetGameEventTypes())
        {
            timelines.Add(new GameEventTimeline(i));
        }
        foreach (var i in timelines)
        {
            i.Init(events);
        }
    }

    public void Update()
    {
        foreach (var i in timelines) 
            if(i.isUsed)i.Update();
    }
    public EventData GetState(float time)
    {
        var ev = new EventData();
        foreach (var i in timelines) i.Apply(time, ref ev);
        ev.time = time;
        return ev;
    }
    public EventData Search(string field,float time, bool get_next)
    {
        foreach(var i in timelines)
        {
            if (field == i.field)
            {
                return i.Search(time, get_next);
            }
        }
        return null;
    }
}