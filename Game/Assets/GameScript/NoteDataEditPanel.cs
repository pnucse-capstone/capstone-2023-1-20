using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;

public class NoteDataEditPanel : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }
    List<Note> notes
    {
        get=>_notes;
        set{Debug.Log(value.Count); _notes = value; }
    }
    List<Note> _notes;

    List<NoteData> next;
    [SerializeField]
    KeySoundPanel keysoundPanel;
    [SerializeField]
    Button color, colorStart, colorEnd;
    public void OpenKeySoundPanel()
    {
        keysoundPanel.Open(next);
    }
    public void Open(List<Note> notes)
    {
        Debug.Log("Open!!!");
        if (notes == null || notes.Count == 0) return;
        this.notes = notes.ToList();
        NoteEditor.PopupLock = true;
        next = notes.Select(x=>new NoteData(x.data)).ToList();
        
        gameObject.SetActive(true);
        BroadcastMessage("ShowEvent", notes);
        Debug.Log(notes.Count);
        // 데이터 로드
    }
    public void SetColorActive(bool value)
    {
        if(notes.Count == 1)
        {
            color.interactable = !value;
            colorEnd.interactable = !value;
            colorStart.interactable = !value;
        }
        else
        {
            colorStart.interactable = colorEnd.interactable = color.interactable = false;
        }
    }
    public void Close()
    {
        Debug.Log(notes.Count);
        NoteEditor.PopupLock = false;

        BroadcastMessage("WriteEvent", next);

        foreach (var i in next)
        {
            i.x = (int)Mathf.Clamp(i.x, 0, 5 - i.y);
        }

        var prev = notes.Select(x => new NoteData(x.data)).ToList();
        NoteEditor.history.Do(new ChangeNoteCommand(prev, next));

        NoteEditor.RefreshBPMS();
        gameObject.SetActive(false);
        // 데이터 적용
    }

}
