using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NoteEditorRender : MonoBehaviour
{
    const int init_amount = 64;
    EditorNote[] enotes = new EditorNote[init_amount];
    public GameObject noteObject;
    public List<Sprite> noteImage = new List<Sprite>();
    public GameObject selectArea;

    EditorNote cursor;
    // Start is called before the first frame update
    int index = 0;
    void Start()
    {
        cursor = Instantiate(noteObject, transform).GetComponent<EditorNote>();
        cursor.SetNote(new Note(new NoteData()));
        cursor.SetVisible(false);
        cursor.SetSortOrder(enotes.Length + 1);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        /*노트 이미지 불러옴*/
        noteImage.Add(Resources.Load("noteX", typeof(Sprite)) as Sprite); //NORMAL

        if (Game.table == null)
            Game.table = new Table();

        /*표시용 오브젝트 초기설정*/
        for (int i = 0; i < enotes.Length; i++)
        {
            enotes[i] = Instantiate(noteObject, transform).GetComponent<EditorNote>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        index = 0;
        foreach (var i in enotes) i.SetVisible(false);
        selectArea.SetActive(false);
    }

    public void DrawDragRect(Rect area)
    {
        selectArea.SetActive(true);
        selectArea.transform.position = area.position;
        selectArea.transform.localScale = area.size;

    }
    public List<Note> SelectNotesInArea(Rect area)
    {
        var temp = new List<Note>();
        for (int i = 0; i < enotes.Length; i++)
        {
            if (enotes[i].GetArea().Overlaps(area, true) && enotes[i].isVisible())
            {
                temp.Add(enotes[i].note);
            }
        }
        return temp;
    }

    public void RenderNote(Note note)
    {
        if (enotes.Length <= index)
        {
            ResizeArray(Math.Max(enotes.Length * 2, index + 2));
        }
        EditorNote temp = enotes[index];
        temp.SetSortOrder(enotes.Length - index);
        temp.SetLineColor(MapLineColor(note));
        temp.SetNote(note);
        temp.SetVisible(true);

        index++;
    }
    public void RenderNotes(List<Note> notes)
    {
        if (enotes.Length < notes.Count)
        {
            ResizeArray(Math.Max(enotes.Length * 2, notes.Count + 1));
        }
        notes.Sort(NoteType.Cmp);
        for (int i = 0; i < enotes.Length; i++)
        {
//            enotes[i].SetVisible(false);
        };
        for (int i = 0; i < notes.Count; i++)
        {
            RenderNote(notes[i]);
        };
        ColoringNotes();
    }
    Color MapLineColor(Note note)
    {
        float hue = note.data.x * (1 / Game.lineCount);
        if ((note.data.y % 2) <= 0.5F) hue += 1 / 12F;
        Color c = Color.HSVToRGB(hue, 0.5F, 0.5F + note.data.length / 16F);
        c.a = 0.2F;
        return c;
    }

    private void ResizeArray(int count)
    {
        Debug.Log("ResizeArray:" + (count, enotes.Length));
        EditorNote[] prev = enotes;
        enotes = new EditorNote[count];
        cursor.SetSortOrder(count + 1);
        for (int i = 0; i < enotes.Length; i++)
        {
            if (i < prev.Length)
            {
                enotes[i] = prev[i];
            }
            else
            {
                enotes[i] = Instantiate(noteObject, transform).GetComponent<EditorNote>();
                enotes[i].SetVisible(false);
            }
        }

    }

    private void ColoringNotes()
    {

        foreach (var e in enotes)
        {
            if (e.isVisible())
            {

                e.ResetColor();
                
                if (NoteEditor.mode == NoteEditor.Mode.SELECT && NoteEditor.selected_notes.Contains(e.note))
                {
                    e.SetColor(Color.green);
                }
                if (NoteEditor.mode == NoteEditor.Mode.NOTE && NoteEditor.selected_notes.Contains(e.note))
                {

                    e.SetColor(Color.blue);
                }
            }
        }
        cursor.ResetColor();
        var select = GetEditorNote(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (select != null)
        {
            if (NoteEditor.mode == NoteEditor.Mode.DELETE) select.SetColor(Color.red);
            if (NoteEditor.mode == NoteEditor.Mode.NOTE)
            {
                if (!NoteEditor.selected_notes.Contains(select.note))
                {
                    foreach (var e in enotes)
                    {
                        e.ResetColor();
                    }
                    select.SetColor(Color.blue);

                }
            }
        }
    }

    public EditorNote GetEditorNote(Vector3 pos)
    {
        float min = float.MaxValue;
        EditorNote select = null;
        foreach (var e in enotes)
        {
            if (e.isVisible())
            {
                var note_area = e.GetArea();
                if (note_area.Overlaps(new Rect(pos.x, pos.y, 0.01F, 0.01F), true))
                {
                    float area = Math.Abs(note_area.width * note_area.height);
                    if (min > area)
                    {
                        min = area;
                        select = e;
                    }
                }
            }
        }
        return select;
    }


}
