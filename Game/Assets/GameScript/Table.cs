using NAudio.Midi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
//using NAudio.Midi;
using System.Linq;
using UnityEngine;

[Serializable]
public struct ColorAdapter
{
    public float r;
    public float g;
    public float b;
    public ColorAdapter(float r, float g, float b)
    {
        this.r = r;
        this.g = g;
        this.b = b;
    }
    public static implicit operator Color(ColorAdapter c)
    {
        return new Color(c.r, c.g, c.b);
    }
    public static implicit operator ColorAdapter(Color c)
    {
        return new ColorAdapter() { r = c.r, g = c.g, b = c.b };
    }
    public override string ToString()
    {
        return $"[R:{r},G:{g},B:{b}]";
    }
}
[Serializable]
public class Table
{
    const int recent_version = 5;
    public int version = recent_version;
    [JsonProperty]
    [SerializeField]
    NotesWindow NoteList = new NotesWindow();
    [JsonProperty]
    [SerializeField]
    List<EventData> EventList = new List<EventData>();

    public void ApplySpecialMode(Action<EventData> firstEvent)
    {
        for (int i = 0; i < EventList.Count; i++)
        {
            foreach (var entry in EventList[i].use.ToArray())
            {
                switch (entry.Key)
                {
                    case "bgColor":
                    case "bgColor2":
                    case "multiplyNoteColor":
                    case "additiveNoteColor":
                    case "lineColorM":
                    case "lineColorA":
                    case "invert":
                    case "invertLong":
                    case "skip":
                    case "speed":
                    case "speed2":
                    case "bpm":
                        break;
                    default:
                        EventList[i].Use(entry.Key, false);
                        break;
                }
            }
        }
        if (EventList.Count != 0)
        {
            if (EventList[0].time == 0)
                firstEvent(EventList[0]);
            else
            {
                var e = new EventData();
                firstEvent(e);
                EventList.Add(e);
            };
        }
    }
    string _title = "";
    public string title
    {
        get => _title == "" ? "Unknown" : _title;
        set => _title = value;
    }
    string _composer = "";
    public string composer
    {
        get => _composer == "" ? "Unknown" : _composer;
        set => _composer = value;
    }

    string _leveler = "";
    public string leveler
    {
        get => _leveler == "" ? "Unknown" : _leveler;
        set => _leveler = value;
    }
    public int level = 1;
    string _description = "";
    public string description
    {
        get => _description == "" ? "Unknown" : _description;
        set => _description = value;
    }
    public bool use_preview_time = false;
    public float preview_time = 0;
    public float offset = 0;
    public float start_bpm = 120;
    public void SetTimeSignature(TimeSignature signature)
    {
        timeSignatureNumerator = signature.numerator;
    }
    public float timeSignatureNumerator = 4; // 타임시그니쳐
    public float timeSignatureMultiplier
    {
        get
        {
            return timeSignatureNumerator / 4F;
        }
    } // 타임시그니쳐

    public float penalty = 1F;
    public int format_code;
    public bool use_script;
    public string composer_link = "";
    public string difficulty = "normal";
    public float length_offset = 0;
    public void CopyTo(Table dest)
    {

        foreach (var i in typeof(Table).GetProperties())
        {
            try
            {
                i.SetValue(dest, i.GetValue(this));
            }
            catch { }
        }
        foreach (var i in typeof(Table).GetFields())
        {
            i.SetValue(dest, i.GetValue(this));
        }
        dest.NoteList = NoteList;
        dest.EventList = EventList;
    }
    public Table()
    {
        NoteList = new NotesWindow();
        EventList = new List<EventData>();
        //        EventList.Add(new EventData() { speed = 1 });
    }

    #region rounding
    float adaptBPMTime(float time, float bpm, float offset, int beats)
    {
        float sperbeat = 60F / bpm / beats;
        time += sperbeat / 2;
        float off_t = time - offset;
        return sperbeat * ((int)(off_t / sperbeat)) + offset;
    }
    public void adaptNoteBPM(float bpm, float offset, int beats)
    {
        foreach (var i in NoteList)
        {
            i.data.time = adaptBPMTime(i.data.time, bpm, offset, beats);
        }
    }
    public void adaptEventBPM(float bpm, float offset, int beats)
    {
        foreach (var i in EventList)
        {
            i.time = adaptBPMTime(i.time, bpm, offset, beats);
        }
    }
    #endregion

    #region apply
    public void Apply(Action<Note> filter)
    {
        foreach (var i in NoteList)
        {
            filter(i);
        }
        NoteList.Sort(NoteType.Cmp);
        postLoadTableHandler();
    }
    public void Apply(Action<EventData> filter)
    {
        foreach (var i in EventList)
        {
            filter(i);
        }
        EventList.Sort(cmp);
    }
    #endregion

    public void RemoveOverlapped()
    {
        RemoveOverlappedNotes();
        RemoveOverlappedEvents();
    }

    private void RemoveOverlappedNotes()
    {

        Debug.Log("RemoveOverlaped");
        var L = getAllNotes((x) => true);
        L.Sort(NoteType.Cmp);
        List<Note> filtered = new List<Note>();
        for (int i = 0; i < L.Count - 1; i++)
        {
            if (!isEquals(L[i], L[i + 1]))
                filtered.Add(L[i]);
        };
        if (L.Count >= 1)
            filtered.Add(L[L.Count - 1]);

        NoteList.Clear();
        foreach (var i in filtered)
        {
            NoteList.Add(i);
        };
    }
    private void RemoveOverlappedEvents()
    {
        var L = getEvents((x) => true);
        L.Sort(cmp);
        List<EventData> L2 = new List<EventData>();
        for (int i = 0; i < L.Count - 1; i++)
        {
            if (L[i].time != L[i + 1].time) L2.Add(L[i]);
        };
        if (L.Count >= 1) L2.Add(L[L.Count - 1]);
        EventList = L2;

    }
    public void setNoteWindowMode(Func<Note, bool> f)
    {
        NoteList.setNoteWindowMode(f);
    }

    #region edit and info

    public void Reset()
    {
        NoteList.reset();
        end_checker = 0;
    }
    public Note getNote(int index)
    {
        return NoteList[index];
    }
    public List<Note> getNearNotes(Func<Note, bool> condition)
    {
        List<Note> ret = new List<Note>();
        List<Note> L = NoteList.getNearList(condition);
        foreach (Note i in L)
        {
            if (condition(i)) ret.Add(i);
        }
        return ret;
    }
    public List<Note> getAllNotes(Func<Note, bool> condition)
    {
        List<Note> temp = new List<Note>();
        foreach (Note i in NoteList)
        {
            if (condition(i)) temp.Add(i);
        }
        return temp;
    }
    public void getAllNotes(Func<Note, bool> condition, ref List<Note> notes)
    {
        notes.Clear();
        foreach (Note i in NoteList)
        {
            if (condition(i)) notes.Add(i);
        }
    }
    public List<Note> getNotesByIndex(int s_index, int e_index)
    {
        var temp = new List<Note>();
        for (int i = s_index; i <= e_index; i++)
        {
            temp.Add(NoteList[i]);
        }
        return temp;
    }
    public List<EventData> getEvents()
    {
        var temp = new List<EventData>();
        foreach (EventData i in EventList)
        {
            temp.Add(i);
        }
        return temp;
    }
    public List<EventData> getEvents(Func<EventData, bool> condition)
    {
        var temp = new List<EventData>();
        foreach (EventData i in EventList)
        {
            if (condition(i)) temp.Add(i);
        }
        return temp;
    }

    public void deleteEvent(EventData target)
    {
        EventList.Remove(target);
        EventList.Sort(cmp);
    }
    public void addEvent(EventData target)
    {
        EventList.Add(target);
        EventList.Sort(cmp);
        postLoadTableHandler();
    }
    public EventData searchEvent(float target, bool get_next = false) // 비교함수는 오름차순일 때 x<y을 반환
    {
        List<EventData> L = EventList;
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
            return L[Math.Max(left, 0)];
        }
        else return L[Math.Min(left + 1, L.Count - 1)];
    }
    public Note searchNote(float target, bool get_next = false) // 비교함수는 오름차순일 때 x<y을 반환
    {

        List<Note> L = NoteList;
        int left = 0;
        int right = L.Count - 1;
        if (L.Count == 0) return null;
        while (left != right)
        {
            int mid = (left + right + 1) / 2;
            if (target < L[mid].data.time) right = mid - 1;
            else left = mid;
        }
        if (!get_next)
        {
            return L[Math.Max(left, 0)];
        }
        else return L[Math.Min(left + 1, L.Count - 1)];
    }
    int end_checker = 0;
    public bool isEnd()
    {
        if (NoteList.Count == 0) return false;
        if (end_checker == NoteList.Count) return true;
        var note = NoteList[end_checker];
        if (note.isComplete() == true && note.data.length + note.data.time < Game.time) end_checker++;
        return false;
    }
    public void addNote(Note note, bool nesting = false)
    {
        if (!NoteList.Exists((x) => isEquals(note, x)) || nesting)
        {
            NoteList.Add(note);
            NoteList.Sort(NoteType.Cmp);
        }
        postLoadTableHandler();
    }
    public void setNotes(List<Note> notes)
    {
        NoteList.Clear();
        notes.ForEach((x) => NoteList.Add(x));
        NoteList.Sort(NoteType.Cmp);
        postLoadTableHandler();
    }
    public void setEvents(List<EventData> events)
    {
        EventList.Clear();
        events.ForEach((x) => EventList.Add(x));
        EventList.Sort(cmp);
    }
    private bool isEquals(Note A, Note B)
    {
        return NoteType.Cmp(A, B) == 0;
    }

    public int numNote()
    {
        return NoteList.Count;
    }
    public void deleteNote(Note note)
    {
        NoteList.Remove(note);
    }
    public void deleteNote(NoteData data)
    {
        var note = NoteList.First(x => NoteType.Cmp(x.data, data) == 0);
        NoteList.Remove(note);
    }
    #endregion

    #region IO
    public void save(string path)
    {
        File.WriteAllText(path, ToJson());
    }

    public string ToJson()
    {
        return TableJsonLoader.ToJSON(this);
    }
    public void loadWithJson(string json_text, bool apply_offset = false)
    {
        FromJson(json_text, apply_offset);
    }
    public void loadWithPath(string path, bool apply_offset = false) // 파일 위치로 로드
    {
        if (File.Exists(path))
        {
            File.ReadAllText(path);
            string json_text = File.ReadAllText(path);
            FromJson(json_text, apply_offset);
        }
        else
        {
            Debug.LogWarning("파일이 경로에 없습니다");
        };
    }

    public void loadWithMIDI(string path)
    {
        Reset();
        var mf = new MidiFile(path);
        double msperbeat = 500000 / 1000.0; //단위에 주의
        double secondpertick = msperbeat / 1000.0 / mf.DeltaTicksPerQuarterNote;
        List<(long, double)> secondperticks = new List<(long, double)>();
        for (int n = 0; n < mf.Tracks; n++)
        {
            Debug.Log("Track no:" + n);
            var timeSignature = mf.Events[0].OfType<TimeSignatureEvent>().FirstOrDefault();
            foreach (var midiEvent in mf.Events[n])
            {
                if (midiEvent.CommandCode == MidiCommandCode.MetaEvent)
                {
                    var meta = (MetaEvent)midiEvent;
                    if (meta.MetaEventType == MetaEventType.SetTempo)
                    {
                        var tempo = (TempoEvent)meta;
                        msperbeat = tempo.MicrosecondsPerQuarterNote / 1000.0;
                        secondpertick = msperbeat / 1000.0 / mf.DeltaTicksPerQuarterNote;
                        secondperticks.Add((tempo.AbsoluteTime, secondpertick));
                    };
                }
            }
        }
        var copy = secondperticks.ToList();
        for (int n = 0; n < mf.Tracks; n++)
        {
            int i = 0;
            double time = 0;
            double prevnotetick = 0;
            secondperticks = copy.ToList();
            foreach (var midiEvent in mf.Events[n])
            {
                if (midiEvent.CommandCode == MidiCommandCode.NoteOn && !MidiEvent.IsNoteOff(midiEvent))
                {
                    double dtick = midiEvent.AbsoluteTime - prevnotetick;
                    while ((i < secondperticks.Count - 1) && secondperticks[i + 1].Item1 < midiEvent.AbsoluteTime)
                    {
                        time += (secondperticks[i + 1].Item1 - secondperticks[i].Item1) * secondperticks[i].Item2;
                        i++;
                    }
                    time += (midiEvent.AbsoluteTime - secondperticks[i].Item1) * secondperticks[i].Item2;
                    if (secondperticks[i].Item2 < 0 || midiEvent.AbsoluteTime - secondperticks[i].Item1 < 0)
                    {
                        Debug.Log("");
                    }
                    secondperticks[i] = (midiEvent.AbsoluteTime, secondperticks[i].Item2);

                    var note = new Note(new NoteData((float)time, 0));

                    NoteOnEvent ev = (NoteOnEvent)midiEvent;
                    int vv = ev.Velocity;
                    int nn = ev.NoteNumber;
                    note.data.color = Utility.DefaultColor(note.data);
                    note.data.lineColorStart = Utility.DefaultLineColorStart(note.data);
                    note.data.lineColorEnd = Utility.DefaultLineColorEnd(note.data);

                    note.data.y = n % 6;
                    NoteList.Add(note);
                    prevnotetick = midiEvent.AbsoluteTime;
                }
            }
        }
        NoteList.Sort(NoteType.Cmp);
        RemoveOverlappedNotes();

    }

    public void FromJson(string json, bool apply_offset)
    {
        var obj = JObject.Parse(json);
        int version = obj.Value<int>("version");
        Table data = TableJsonLoader.GetTable(version, json);
        data.CopyTo(this);
        NoteList.Sort(NoteType.Cmp);
        if (apply_offset) ApplyNoteOffset();
        InvalidTableCorrection();
        postLoadTableHandler();
    }
    #endregion
    public void InvalidTableCorrection()
    {
        foreach (var i in EventList)
        {
            if (i.isUse("bgColor") && !i.isUse("bgColor2"))
            {
                i.bgColor2 = i.bgColor;
            }
            i.Use("bgColor2", i.isUse("bgColor"));
        }
        //        if (music_volume < 0 || music_volume > 1) music_volume = 1;
    }
    void ApplyNoteOffset()
    {
        foreach (NoteData i in NoteList.Select((x) => x.data))
        {
            i.time += PlayerPrefs.GetFloat("sync", 0.1F);
        }
        foreach (EventData i in EventList)
        {
            i.time += PlayerPrefs.GetFloat("sync", 0.1F);
        }
        EventList[0].time = 0;


    }
    void postLoadTableHandler()
    {
        Debug.Log("NoteList");
        for (int i = 0; i < NoteList.Count; i++)
        {
            NoteList[i].data.index = i;
        }
    }
    // 비정상적인 데이터 수정
    private int cmp(EventData x, EventData y)
    {
        return Math.Sign(x.time - y.time);
    }
    public int numEvents()
    {
        return EventList.Count;
    }


    public static Table CreateFromJson(string json)
    {
        var table = new Table();
        table.FromJson(json, false);
        return table;
    }
}
[Serializable]
public class BPMNode
{
    public float time = 0;
    public float bpm = 120;
    public BPMNode(float time, float bpm)
    {
        this.time = time;
        this.bpm = bpm;
    }
}
[Serializable]
public class NotesWindow : List<Note>
{
    int left = 0;
    int right = 0;
    public NotesWindow() : base()
    {
        isInRange = isNoteInScreen;
    }
    public void reset()
    {
        foreach (Note i in this)
        {
            i.Reset();
        }
        left = 0;
        right = 0;
    }
    public List<Note> getNearList(Func<Note, bool> cond)
    {
        List<Note> L = new List<Note>();
        adjustRange();
        if (this.Count == 0) return L;
        for (int i = left; i <= right; i++)
        {
            if (cond(this[i]) && isNear(this[i]))
            {
                L.Add(this[i]);
            }
        }
        return L;
    }
    void adjustRange()
    {
        int now_index = left;
        while (now_index < Count - 1)
        {
            Note now = this[now_index];
            if (Game.time < now.data.time) break;
            if (isInRange(now)) break;
            now_index++;
        };
        while (now_index > 0)
        {
            now_index--;
            Note now = this[now_index];
            if (!isInRange(now)) break;
        };
        left = now_index;

        now_index = Mathf.Clamp(right, 0, Count - 1);
        while (now_index > 0)
        {
            Note now = this[now_index];
            if (Game.time > now.data.time) break;
            if (isInRange(now)) break;
            now_index--;
        };
        while (now_index < Count - 1)
        {
            now_index++;
            Note now = this[now_index];
            if (!isInRange(now)) break;
        };
        right = now_index;
    }
    Func<Note, bool> isInRange;
    public void setNoteWindowMode(Func<Note, bool> f)
    {
        isInRange = f;
    }
    public static bool isNear(Note now)
    {
        return (now.data.time - Game.time) < 10F && (Game.time - now.data.time) < 2F;
    }
    public static bool isNoteInScreen(Note now)
    {
        return now.isInScreen();
    }
}

[Serializable]
public class TableMetaData
{
    public string title;
    public string composer;
    public string leveler;
    public int level;
    public string difficulty;
    public string composer_link;
    public string description;
    public string hash; // 내부 참조용. 실제 검증은 서버단에서함
    public float volume; // 내부 참조용. 실제 검증은 서버단에서함
    public void Load(Table table)
    {
        title = table.title;
        composer = table.composer;
        leveler = table.leveler;
        level = table.level;
        difficulty = table.difficulty;
        composer_link = table.composer_link;
        description = table.description;
        //        volume = table.music_volume;
    }
}