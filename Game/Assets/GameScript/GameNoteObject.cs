using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using static UnityEngine.GraphicsBuffer;

public class GameNoteObject : MonoBehaviour
{
    [SerializeField] SpriteRenderer head;
    [SerializeField] SpriteRenderer headCover;
    [SerializeField] SpriteRenderer Outline;
    [SerializeField] NoteLineRenderer line;
//    [SerializeField] NoteLineRenderer lineOutline;
    // Start is called before the first frame update
    void Start()
    {
        SetVisible(false);
    }
    public void SetNote(Note note)
    {
        if(note == null || (note.isComplete() && note.state.judge_result != JudgeType.miss))
        {
            SetVisible(false);
        }
        else
        {
            Profiler.BeginSample("Visable");
            SetVisible(true);
            Profiler.EndSample();

            Profiler.BeginSample("Head");
            SetNoteHead(note);
            Profiler.EndSample();

            Profiler.BeginSample("Line");
            SetNoteLine(note);
            Profiler.EndSample();
        }
    }

    public int sortingOrder
    {
        get => head.sortingOrder;
        set 
        { 
            head.sortingOrder = value;
            Outline.sortingOrder = value-1;
            line.sortingOrder = value;
            //lineOutline.sortingOrder = value - 1;
        }
    }
    private void SetNoteLine(Note note)
    {
        if (note.data.length > 0)
        {

            line.SetVisible(true); 
            //lineOutline.SetVisible(true);
            var notePath = note.NotePath();
            line.material.SetFloat("_Start", Mathf.Max(0, Game.time-note.data.time)/ note.data.length);
            float l = CalcLength(notePath);
            line.material.SetFloat("_Length", l);

            line.RenderLinePath(notePath, note.LongNoteWidth(), note);
            //lineOutline.RenderLinePath(notePath, note.LongNoteWidth() + 0.1F);

            if (note.isComplete())
            {
                line.material.SetColor("_ColorStart", note.LongNoteColor(0)*0.5F);
                line.material.SetColor("_ColorEnd", note.LongNoteColor(1) * 0.5F);
            }
            else
            {
                line.material.SetColor("_ColorStart", note.LongNoteColor(0));
                line.material.SetColor("_ColorEnd", note.LongNoteColor(1));
            }
            line.material.SetFloat("_Thick", 0.6F / note.LongNoteWidth());
            //            line.material.SetFloat("_Invert", Mathf.Lerp(note.data.invertLong, 1 - note.data.invertLong, Game.state.properties.invertLong));
        }
        else
        {
            line.SetVisible(false);
            //lineOutline.SetVisible(false);
        }
    }

    private float CalcLength(LinePath.Point[] notePath)
    {
        float L = 0;
        for(int i = 0; i < notePath.Length-1; i++)
        {
            L += (notePath[i].ToVector3() - notePath[i+1].ToVector3()).magnitude;
        }
        return L;
    }

    [SerializeField]
    Sprite[] image;
    private void SetNoteHead(Note note)
    {
        /*¸Ó¸® ·»´õ¸µ*/
        Profiler.BeginSample("Scale");
        head.size = note.Scale();
        Outline.size = head.size;
        Profiler.EndSample();

        Profiler.BeginSample("Image");
        head.sprite = note.NoteImage();
        Outline.sprite = head.sprite;
        Profiler.EndSample();

        Profiler.BeginSample("Position");
        transform.position = note.Position(note.state.length);
        Profiler.EndSample();

        Profiler.BeginSample("Rotation");
        transform.rotation = note.Rotation();
        Profiler.EndSample();

        Profiler.BeginSample("Color");
        head.color = note.NoteColor();
        Outline.color = head.color;

        head.material.SetColor("_Tint",head.color);
        Profiler.EndSample();

        if (note.data.x == 0)
        {
            head.material.SetFloat("_Outline", 0.07F);
            head.size = new Vector2(head.size.x, head.size.y+ 0.08F);
        }
        else if (note.data.x <= 2)
        {
            head.material.SetFloat("_Outline", 0.06F);
            head.size = new Vector2(head.size.x , head.size.y + 0.08F);
        }
        else
        {
            head.material.SetFloat("_Outline", 0.05F);
            head.size = new Vector2(head.size.x, head.size.y + 0.08F);
        }
        if (note.isComplete())
        {
            head.color = note.NoteColor()*0.5F;
            head.material.SetColor("_Tint", note.NoteColor() * 0.5F);
        }
        if (Game.reduced)
        {
            head.material.SetFloat("_Invert", 0);
        }
        else
        {
            head.material.SetFloat("_Invert", Mathf.Lerp(note.data.invert, 1 - note.data.invert, Game.state.properties.invert));
        }

        Outline.size = head.size;
        headCover.size = head.size;
        headCover.color = head.color;

    }

    private void SetVisible(bool v)
    {
        line.SetVisible(v);
        //lineOutline.SetVisible(v);
        head.enabled = v;
        Outline.enabled = v;
        headCover.enabled= v;
    }
}
