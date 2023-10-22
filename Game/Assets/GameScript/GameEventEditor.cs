using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameEventEditor : MonoBehaviour
{
    public GameObject prefab;
    public List<GameEventGraphic> lines;
    [SerializeField] EventEditPanel panel;
    [SerializeField] GameObject cursor;

    public static List<EventData> selected = new List<EventData>();
    // Start is called before the first frame update
    public void Select(List<EventData> events)
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            selected = Union(events.ToList(),selected);
        }
        else if (Input.GetKey(KeyCode.LeftAlt))
        {
            selected = Union(events.ToList(), selected);
        }
        else
        {
            selected = events.ToList();
        }
    }
    void Start()
    {
        lines = new List<GameEventGraphic>(1);
    }
    public void Refresh()
    {
        ResizeCollider();
    }

    void OnMouseEnter()
    {
        cursor.SetActive(true);    
    }
    float cursor_distance= 0F;
    private void OnMouseOver()
    {
        //        cursor.SetActive(NoteEditor.mode != NoteEditor.Mode.SELECT);
        float distance = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).x/NoteEditor.zoom; // 
        distance = NoteEditor.snaps.Snap(distance); // 시간으로 바꿔서 스냅 후
        cursor.transform.localPosition = distance*Vector3.right*NoteEditor.zoom; // 다시 거리로 바꿔서 대입
        cursor_distance = distance;  // 사실 시간임
        
    }
    public List<EventData> Union(List<EventData> a, List<EventData> b)
    {
        HashSet<EventData> A = new HashSet<EventData>(a);
        HashSet<EventData> B = new HashSet<EventData>(b);
        return A.Union(B).ToList();
    }
    public List<EventData> Except(List<EventData> a, List<EventData> b)
    {
        HashSet<EventData> A = new HashSet<EventData>(a);
        HashSet<EventData> B = new HashSet<EventData>(b);
        return A.Except(B).ToList();
    }

    void OnMouseExit()
    {
        cursor.SetActive(false);
    }
    public List<EventData> SelectInRect(Rect rect)
    {
        var newSelected = new List<EventData>();
        foreach(var i in lines)
        {
            if (i.isInRect(rect))
            {
                newSelected.Add(i.data);
            }
        }
        Select(newSelected);
        return newSelected;
    }
    // Update is called once per frame
    Vector3 PrevMousePosition;
    public EventData GetFromMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        var target = lines.Find((x) => x != null && hit.collider !=null && x.gameObject == hit.collider.gameObject);
        if (target == null) return null;
        return target.data;
    }
    void Update()
    {
        if (NoteEditor.PopupLock) return;
        var list = NoteEditor.tableNow.getEvents();
        RenderEvents(list);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit.collider != null)
        {
            GameEventGraphic target = lines.Find((x) => x.gameObject == hit.collider.gameObject);
            if (target != null) target.SetColor(Color.white);
            if (Input.GetMouseButtonDown(0))
            {
                PrevMousePosition = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0))
            {

                if (hit.collider != null && target != null && (Input.mousePosition - PrevMousePosition).magnitude < 10)
                {
                    panel.Open(target.data);
                }
                if (cursor.activeSelf && (Input.mousePosition-PrevMousePosition).magnitude<10)
                {
                    var time = cursor_distance;
                    var man = new GameEventManager(NoteEditor.tableNow.getEvents());
                    var temp = man.GetState(time);

                    if (!list.Exists((x) => x.time == temp.time) && selected.Count == 0)
                    {
                        NoteEditor.history.Do(new AddGameEventCommand(temp));
                    }
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (target != null) target.SetColor(Color.white);
                if (hit.collider != null && target != null)
                {
                    NoteEditor.history.Do(new DeleteGameEventCommand(target.data));
                    if (target.data.isUse("bpm"))
                    {
//                        GameObject.Find("Refresh").GetComponent<Refresher>().HardRefresh();
                        NoteEditor.RefreshBPMS();
                    }
                }
            }
        }

    }

    private void RenderEvents(List<EventData> list)
    {
        if (list.Count > lines.Count) Resize(Mathf.Max(list.Count, lines.Count * 2));

        foreach (var i in lines)
        {
            i.SetVisible(false);
        }

        for (int i = 0; i < list.Count; i++)
        {
            lines[i].SetEvent(list[i]);
            if (selected.Contains(lines[i].data))
            {
                lines[i].SetColor(Color.green);
            }
            else
            {
                lines[i].ResetColor();
            }
        }
    }

    void ResizeCollider()
    {

        float width = MusicPlayer.expandedlength*NoteEditor.zoom;
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Vector3 size = collider.size;
        Vector3 offset = collider.offset;
        offset.x = width / 2F;
        size.x = width;
        collider.size = size;
        collider.offset = offset;
    }
    void Resize(int count)
    {
        while (count>lines.Count)
        {
            lines.Add((Instantiate(prefab,transform) as GameObject).GetComponent<GameEventGraphic>());
        }
    }

}
public class AddGameEventCommand:EditorCommand
{
    List<EventData> target;
    public AddGameEventCommand(EventData temp)
    {
        target = new List<EventData>();
        target.Add(temp);
    }
    public AddGameEventCommand(List<EventData> temp)
    {
        target = new List<EventData>();
        foreach (var i in temp)
        {
            target.Add(i);
        }

    }
    public void Undo()
    {
        foreach(var i in target)
        {
            NoteEditor.tableNow.deleteEvent(i);
        }

        NoteEditor.RefreshBPMS();
    }
    public void Redo()
    {
        foreach (var i in target)
        {
            NoteEditor.tableNow.addEvent(i);
        }

        NoteEditor.RefreshBPMS();
    }
    public void Do()
    {
        Redo();
    }
}
public class DeleteGameEventCommand : EditorCommand
{
    List<EventData> target;
    public DeleteGameEventCommand(EventData temp)
    {
        target = new List<EventData>();
        target.Add(temp);
    }
    public DeleteGameEventCommand(List<EventData> temp)
    {
        target = new List<EventData>();
        foreach (var i in temp)
        {
            target.Add(i);
        }

    }
    public void Undo()
    {
        foreach (var i in target)
        {
            NoteEditor.tableNow.addEvent(i);
        }

        NoteEditor.RefreshBPMS();
    }
    public void Redo()
    {
        foreach (var i in target)
        {
            NoteEditor.tableNow.deleteEvent(i);
        }

        NoteEditor.RefreshBPMS();
    }
    public void Do()
    {
        Redo();
    }
}

public class ChangeGameEventCommand : EditorCommand
{
    List<EventData> prev;
    List<EventData> next;
    List<EventData> target;

    public ChangeGameEventCommand(List<EventData> prev, List<EventData> next)
    {
        this.prev = new List<EventData>();
        this.next = new List<EventData>();
        target = new List<EventData>();
        foreach(var i in prev)
        {
            target.Add(i);
            this.prev.Add(i.Clone());
        }
        foreach (var i in next)
        {
            this.next.Add(i.Clone());
        }
    }

    public ChangeGameEventCommand(EventData prev,EventData next)
    {
        this.prev = new List<EventData>();
        this.next = new List<EventData>();
        target = new List<EventData>();
        target.Add(prev);
        this.prev.Add(prev.Clone());
        this.next.Add(next.Clone());
    }
    public void Undo()
    {
        for(int i=0;i<target.Count; i++)
        {
            target[i].Set(prev[i]);
        }
        GameEventEditor.selected = target;

        NoteEditor.RefreshBPMS();

    }
    public void Redo()
    {
        for (int i = 0; i < target.Count; i++)
        {
            target[i].Set(next[i]);
        }
        NoteEditor.RefreshBPMS();

    }
    public void Do()
    {
        Redo();
    }
}

