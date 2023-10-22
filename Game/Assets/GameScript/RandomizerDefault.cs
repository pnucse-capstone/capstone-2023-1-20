using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public interface Randomizer
{
    public void Convert(Table table);
}

public class RandomizerDefault:Randomizer
{
    int from, to;
    public RandomizerDefault(int from=0, int to=6) 
    { 
        this.from = from;
        this.to = to;
    }
    public void Convert(Table table)
    {
        List<NoteData> notes = table.getAllNotes((x)=>true).Select(x=>x.data).ToList();
        List<NoteData> after = Convert(notes);
        table.setNotes(after.Select(x=>new Note(x)).ToList());
    }
    public List<NoteData> Convert(List<Note> notes)
    {
        List<NoteData> data = notes.Select(x => x.data).ToList();
        List<NoteData> after = Convert(data);
        return after;
    }
    public List<NoteData> Convert(List<NoteData> notes)
    {
        var temp = notes.ToArray();
        temp.Reverse();
        Stack<NoteData> stack = new Stack<NoteData>(temp);
        
        Stack<NoteData> ret= new Stack<NoteData>();
        while (stack.Count != 0)
        {
            var note = stack.Pop();
            List<int> r = GetPossibleLines(note);
            // ����ϴ� ���� ����
            while (r.Count != 0)
            {
                //������ ������ ��� ������ġ���� Ȯ���ؼ� ������ �ƴϸ� �߰���.
                int i = UnityEngine.Random.Range(0, r.Count);
                int line = r[i];
                r.RemoveAt(i);
                note.y = line;
                //                Debug.Log("Moori" + JsonConvert.SerializeObject(r)+","+i);

                if (!Moori(note, ret))
                {
                    ret.Push(note);
                    break;
                }
                if (r.Count == 0 && note.length != 0)
                {
                    note.length = 0;
                    r = GetPossibleLines(note);
                }
            }

        }

        //        Debug.Log("Moori Ret" + JsonConvert.SerializeObject(ret) );
        return ret.ToList();
    }

    private List<int> GetPossibleLines(NoteData note)
    {
        var r = new List<int>();
        for (int i = 0 + from; i < to - note.x; i++)
        {
            r.Add(i);
        }

        return r;
    }

    bool Moori(NoteData note,Stack<NoteData> others)
    {
        int cnt = 0;
        float prevtime = 0;
        foreach (var i in others)
        {
//            Debug.Log("Moori" + (note.time,note.y,note.x)+(i.time,i.y, i.x));
            if (!isSimmilar(prevtime , i.time))
            {
                cnt = 0;
            }
            if (isOnNote(note, i.time) &&
                isXOverlapped(note, i))
            {
                cnt++;
                 if (cnt > note.x-1)
                {
                    return true;
                }
            }
            prevtime = i.time;
        }
        foreach (var i in others)
        {
            if (Eva(note, i))
            {
                return true;
            }
        }
        return false;
    }

    private bool isOnNote(NoteData note, float time)// time�� ��Ʈ �ð� ������ ��ġ����
    {
        return isIn(note.time, note.time + note.length * 1.05F, time);
    }

    private bool isXOverlapped(NoteData note, NoteData i)// x������ ��ġ���� üũ
    {
        return (
                isIn(note.y, note.y + note.x, i.y)
                ||
                isIn(note.y, note.y + note.x, i.y + i.x)
                );
    }
    private bool isXInclude(NoteData note, NoteData i)// note�� i�� x������ �����ϴ��� üũ
    {
        return (
                isIn(note.y, note.y + note.x, i.y)
                &&
                isIn(note.y, note.y + note.x, i.y + i.x)
                );
    }

    bool Eva(NoteData a, NoteData b) // ��Ʈ 1:1 ����� �˼� �ִ� ������ġ
    {
        if (isSimmilar(a.time, b.time) && a.x == b.x && a.y == b.y) return true;
        if (isSimmilar(a.time, b.time) && !isXInclude(a, b) && !isXInclude(b, a) && isXOverlapped(a, b))
        {
            return true;
        }
        if (a.x == 0 &&  b.x == 0 && a.y == b.y && Mathf.Abs(a.time - b.time) < 0.2F) return true;
        return false;
    }
    bool isSimmilar(float a, float b)
    {
        return Mathf.Abs(a - b) < 0.001F;
    }
    bool isIn(float a,float b,float o)
    {
        return a <= o && o <= b;
    }
}
