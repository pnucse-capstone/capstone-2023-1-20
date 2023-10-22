using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorNote : MonoBehaviour
{
    [SerializeField]
    LineRenderer line;
    [SerializeField]
    SpriteRenderer head;
    public Note note;
    // Start is called before the first frame update
    void Start()
    {
        SetVisible(false);
        line.enabled = false;
    }
    public bool isVisible()
    {
        return gameObject.activeSelf;
    }
    public Rect GetArea()
    {
        var size = line.GetPosition(1) - line.GetPosition(0) + Vector3.down * line.widthMultiplier;
        size.x = Mathf.Max(size.x, 0.5F);
        return new Rect(transform.position, size);
    }
    public void SetNote(Note note)
    {
        this.note = note;
        transform.localPosition = Position(note, 0);        
        transform.localScale = Scale(note);

        if (note.data.length >= 0)
        {
            line.enabled = true;
            line.positionCount = 2;
            Vector3 A = (Vector3.down * (note.data.x + 1)) *0.5F*6F/Game.lineCount;
            Vector3 B = Position(note, note.data.length)- Position(note, 0);
            line.SetPosition(0, transform.position + A);
            line.SetPosition(1, transform.position + A + B);
            line.widthMultiplier = Scale(note).y;
        }
        else
        {
            line.enabled = false;
        }
    }
    public void SetColor(Color color)
    {
        head.color = color;
    }
    public void SetVisible(bool value)
    {
        gameObject.SetActive(value);
    }
    public void SetSortOrder(int value)
    {
        head.sortingOrder = value;
        line.sortingOrder = value;
    }
    public void SetLineColor(Color color)
    {
        line.startColor = line.endColor = color;
    }
    public void ResetColor()
    {
        SetColor(NoteColor(note));
    }
    Vector3 Position(Note note, float offset)
    {
        Vector3 p = Vector3.right * (note.data.time + offset)*NoteEditor.zoom + Vector3.down * note.data.y*6F/Game.lineCount;
        p.z = 0;
        return p;
    }
    Vector3 Scale(Note note)
    {
        return new Vector3(0.10F, (1 + note.data.x)*6F/Game.lineCount) - Vector3.up * 0.06F;
    }
    Color NoteColor(Note note)
    {
        return note.NoteColor();
    }
}
