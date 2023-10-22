using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventEditPanel : MonoBehaviour
{
    EventData edit;
    [SerializeField]
    LinePathEditor PathPanel;

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
    EventData prev;
    public void Open(EventData data)
    {
        NoteEditor.PopupLock = true;
        prev = data;
        edit = data.Clone();
        gameObject.SetActive(true);
        BroadcastMessage("ShowEvent", edit);
        // 데이터 로드
    }
    public void Close()
    {
        NoteEditor.PopupLock = false;

        BroadcastMessage("WriteEvent", edit);
        NoteEditor.history.Do(new ChangeGameEventCommand(prev, edit));
        NoteEditor.RefreshBPMS();
        PathPanel.Close();
        gameObject.SetActive(false);
        // 데이터 적용
    }

}
