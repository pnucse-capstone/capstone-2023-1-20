using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePreprocessor : MonoBehaviour // 노트 위치 등을 미리 계산해둔다
{
    public static List<Note> usedNotes = new List<Note>();
    public static Camera mainCam;
    // Start is called before the first frame update
    public static void FindAllNotes(Func<Note,bool> predicate,ref List<Note> list)
    {
        list.Clear();
        for(int i=0;i<usedNotes.Count;i++)
        {
            var note = usedNotes[i];
            if (predicate(note))
            {
                list.Add(usedNotes[i]);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        mainCam = Camera.main;
        NoteEditor.tableNow.getAllNotes(x=>x.inTimeBoundary(),ref usedNotes);
    }

}
