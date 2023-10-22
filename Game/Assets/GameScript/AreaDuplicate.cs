using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AreaDuplicate : MonoBehaviour
{
    /*
    public GameObject image_left;
    public GameObject image_right;
    float left_bound;
    float right_bound;
    List<Note> nl;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
    }
    public void UpsideDown()
    {
        nl = Game.table.getAllNotes((x) => (x.data.time >= left_bound && x.data.time <= right_bound));
        nl.Sort(cmp);
        foreach (Note i in nl)
        {
            i.data.y = -i.data.y;
        }
    }
    public void duplicate()
    {
        nl = Game.table.getAllNotes((x) => (x.data.time >= left_bound && x.data.time <= right_bound));
        nl.Sort(cmp);
        float bef = nl[0].data.time;
        float now = EditorController.time_round(Game.time);
        foreach (Note i in nl)
        {
            NoteData temp = new NoteData(i.data);
            temp.time -= bef;
            temp.time += now;
            Game.table.addNote(new Note(temp));
        }
    }
    private int cmp(Note x, Note y)
    {
        return Math.Sign(x.data.time - y.data.time);
    }
    */
}
