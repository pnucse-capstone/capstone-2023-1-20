using System.Collections.Generic;
using UnityEngine;
//using FMOD;
using System.Linq;
using System;
using UnityEngine.Profiling;

public class NoteRender : MonoBehaviour
{
    public GameObject noteObject;
    int amount;
    GameObject[] arr = new GameObject[1];
    GameNoteObject[] objs = new GameNoteObject[1];
    void Start()
    {
        /*표시용 오브젝트 초기설정*/
        Vector3 origin = Camera.main.transform.position;
        origin.z = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = Instantiate(noteObject, origin, Quaternion.identity);
            objs[i] = arr[i].GetComponent<GameNoteObject>();
        }

        amount = CalcInitMount();
        Debug.Log("Window Count:" + amount);
        ResizeArray(amount+30);


    }
    int CalcInitMount()
    {
        var notes = NoteEditor.tableNow.getAllNotes(x=>true);
        Debug.Log("notesCount:"+notes.Count);
        int max = 0;
        foreach(var note in notes)
        {
            float time = note.data.time;
            var inner = NoteEditor.tableNow.getAllNotes((x) => time-5< x.data.time && x.data.time <time+10);
            max = Math.Max(max, inner.Count);
        }
        return max;
    }
    private void ResizeArray(int count)
    {
        Debug.Log("Resize");
        Vector3 origin = Camera.main.transform.position;
        origin.z = 0;
        GameObject[] prev = arr;
        arr = new GameObject[count];
        objs = new GameNoteObject[count];
        for (int i = 0; i < arr.Length; i++)
        {
            if (i < prev.Length)
            {
                arr[i] = prev[i];
                objs[i] = arr[i].GetComponent<GameNoteObject>();
            }
            else
            {
                arr[i] = Instantiate(noteObject, origin, Quaternion.identity);
                objs[i] = arr[i].GetComponent<GameNoteObject>();
            }
        }
        amount = count;

    }
    bool isVisable(Note x)
    {
        return x.inTimeBoundary() 
            && (Game.time <= x.data.time+Game.judgepoint_offset || x.Type().judge_info.JudgeOn(Game.time, x.data.time, x.data.length)) 
            && !x.state.hide
            ;
    }
    void Update()
    {
        RenderNotes();
    }

    public static int Cmp(Note x, Note y)
    {
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

        if (Cmp(x.data.y, y.data.y) != 0)
        {
            return Cmp(x.data.y, y.data.y);
        }
        return Cmp(x.data.dx, y.data.dx);
    }

    private static int Cmp(float x, float y)
    {
        if (Mathf.Abs(x - y) < 0.001) return 0;
        return Math.Sign(x - y);
    }
    List<Note> notes = new List<Note>();
    private void RenderNotes()
    {
        Profiler.BeginSample("GetAllNotes");
        if (GamePreprocessor.usedNotes == null)
        {
            Debug.Log("Empty Notes");
            return;
        }
        GamePreprocessor.FindAllNotes(isVisable,ref notes);
        Profiler.EndSample();
        Profiler.BeginSample("ResizeArray");
        if (arr.Length < notes.Count)
            ResizeArray(Mathf.Max(notes.Count, amount * 2));
        Profiler.EndSample();
        Profiler.BeginSample("Sort");
        notes.Sort(Cmp);
        Profiler.EndSample();
        Profiler.BeginSample("SetNotes");
        for (int i = 0; i < amount; i++)
            objs[i].SetNote(null);
        Profiler.EndSample();
        Profiler.BeginSample("SetSortOrder");
        for (int i = 0; i < notes.Count; i++)
        {

            Note note = notes[i];
            var obj = objs[i];

            if (note.isInScreen())
            {
                obj.sortingOrder = amount - i;
                obj.SetNote(note);
            }
            else
            {
                obj.SetNote(null);
            }

        };
        Profiler.EndSample();
    }
}
public class NoteDistance
{
    static List<float> dist;
    static List<EventData> events;
    public static float searchDt(float time)
    {
        int i = search(events, time);
        return dist[i]+events[i].speed*(time-events[i].time);
    }
    static EventData searchEvent(float time)
    {
        int i = search(events, time);
        return events[i];
    }
    static int search(List<EventData> L, float target) // 비교함수는 오름차순일 때 x<y을 반환
    {
        int left = 0;
        int right = L.Count - 1;
        while (left != right)
        {
            int mid = (left + right + 1) / 2;
            if (target<L[mid].time) right = mid - 1;
            else left = mid;
        }
        return left;
    }
}