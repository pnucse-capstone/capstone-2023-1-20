using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class NoteEditor : MonoBehaviour
{
    public static string now_open_url;
    public static SnapGrid snaps;
    [SerializeField]
    NoteEditorRender view;
    public static bool vertical = true;
    public static float zoom = 1F;
    public static bool sketchmode = false;
    public static Table sketch = new Table();

    public static void SetSketch(bool value)
    {
        if (value != sketchmode)
        {
            Debug.Log("Swap:" + (sketchmode, value));
            history.Do(new SketchCommand(sketchmode, value));
        }

    }
    public static Table tableNow
    {
        get
        {
            if (sketchmode) return sketch;
            else return Game.table;
        }
    }

    public static void RefreshBPMS()
    {
        try
        {
            GameObject.Find("Refresh").GetComponent<Refresher>().Refresh();

        }
        catch (Exception e) { }
    }
    public void Start()
    {
        ResizeCollider();
        StartCoroutine(SelectDragAdd());
        StartCoroutine(SelectDragSelect());
        //        StartCoroutine(SelectDragAddRight());
        SwapMode(Mode.ADD);

        selected_notes.Clear();
        selected_events.Clear();
    }
    public enum Mode { ADD, DELETE, SELECT, NOTE, AUTOMATION }
    [SerializeField] NoteDataEditPanel noteEditPanel;
    public static bool PopupLock = false;
    public static Mode mode = Mode.ADD;
    public static int divide = 1;
    public static int focus = 0;
    public static bool isLoaded = false;

    [SerializeField] GameEventEditor EventManager;

    public static CommandHistory history = new CommandHistory();

    static bool editor_flag = true;

    public static List<Note> selected_notes = new List<Note>();
    private List<EventData> selected_events
    {
        get => GameEventEditor.selected;
    }

    void Awake()
    {
        if (editor_flag)
        {
            editor_flag = false;
            Reset();
        }
    }
    public static void Reset()//에디터를 시작상태로 만듬
    {
        Debug.Log("Editor Reset");
        history.Clear();
        snaps = new SnapGrid();
        Game.pbrffdata = new PbrffExtracter.FullEntry();
        Game.table = new Table();
        sketch = new Table();
        isLoaded = false;

        MusicPlayer.Init();
        Game.mechanim = GameMech.Create(GameMech.Type.DEFAULT);
        Game.state = new GameState();

        BrowseLoad.pbrff = new PbrffExtracter(0x00);
        now_open_url = null;
        PopupLock = false;

        snaps.CalcGrid();
        Debug.Log(snaps);
    }


    public void Refresh()
    {
        ResizeCollider();
    }

    void ResizeCollider()
    {

        float width = MusicPlayer.expandedlength * zoom;
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Vector3 size = collider.size;
        Vector3 offset = collider.offset;
        offset.x = width / 2F;
        size.x = width;
        collider.size = size;
        collider.offset = offset;
    }



    IEnumerator SelectDragAdd()
    {
        while (true)
        {

            Vector3 start, end;
            while (!(Input.GetMouseButtonDown(0) && !PopupLock && mode == Mode.ADD && isOver))//입력 전
            {
                if (isOver && mode == Mode.ADD)
                {
                    end = start = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var note = GetNoteFrom(start, end);
                    view.RenderNote(note);
                }
                yield return null;
            }

            end = start = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 드래그 시작

            while (!Input.GetMouseButtonUp(0))// 드래그 중
            {
                if (isOver)
                {
                    end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var note = GetNoteFrom(start, end);
                    view.RenderNote(note);
                }
                yield return null;
            }

            if (isOver)//드래그 종료
            {
                var note = GetNoteFrom(start, end);
                history.Do(new AddCommand(note, false));

            }
            Game.table.RemoveOverlapped();
            sketch.RemoveOverlapped();
            yield return null;
        }
    }
    IEnumerator SelectDragAddRight()
    {
        while (true)
        {

            Vector3 point;
            while (!(Input.GetMouseButtonDown(1) && !PopupLock && mode == Mode.ADD && isOver))//입력 전
            {
                yield return null;
            }
            if (isOver && mode == Mode.ADD)
            {
                point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                var note = GetNoteFrom(point, point);
                note.data.type = 1;
                note.data.x = 0;
                history.Do(new AddCommand(note, false));
                Game.table.RemoveOverlapped();
                sketch.RemoveOverlapped();
            }
            yield return null;
        }
    }
    IEnumerator SelectDragSelect()
    {
        while (true)
        {
            Vector3 start, end;

            while (!Input.GetMouseButtonDown(0) || PopupLock || mode != Mode.SELECT)//입력 전
            {
                yield return null;
            }

            RefreshBPMS();

            end = start = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 드래그 시작
            Game.table.RemoveOverlapped();
            sketch.RemoveOverlapped();
            var enote = view.GetEditorNote(start);
            var eventdata = EventManager.GetFromMousePosition();

            //드래그로 이동하기
            if ((enote != null && selected_notes.Contains(enote.note)) || (eventdata != null && selected_events.Contains(eventdata)))
            {
                while (!Input.GetMouseButtonUp(0))
                {
                    end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    yield return null;
                }
                DragSelectedMove(start, end);
            }
            else
            {
                //선택
                Rect area = new Rect(start, Vector3.zero);
                view.DrawDragRect(area);
                while (Input.GetMouseButton(0))
                {
                    end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    area = new Rect(start, end - start);
                    view.DrawDragRect(area);
                    var newselected = view.SelectNotesInArea(area);

                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        selected_notes = Union(newselected, selected_notes);
                    }
                    else if (Input.GetKey(KeyCode.LeftAlt))
                    {
                        selected_notes = Except(selected_notes, newselected);
                    }
                    else
                    {
                        selected_notes = newselected;
                    }
                    EventManager.SelectInRect(area);
                    yield return null;
                }
                area = new Rect(start, Vector3.zero);
                view.DrawDragRect(area);
            }


            yield return null;
        }
    }

    private void DragSelectedMove(Vector3 start, Vector3 end)
    {
        float distance = GetTimeFromPosition(end) - GetTimeFromPosition(start);
        float min = SelectedMinTime(); // 
        float offset = snaps.Snap(min + distance) - min;
        ChangeSelected((x) => x.time += offset, (x) => x.time += offset);
    }

    List<Note> Except(List<Note> a, List<Note> b)
    {
        var A = new HashSet<Note>(a);
        var B = new HashSet<Note>(b);
        return A.Except(B).ToList();
    }
    List<Note> Union(List<Note> a, List<Note> b)
    {
        var A = new HashSet<Note>(a);
        var B = new HashSet<Note>(b);
        return A.Union(B).ToList();
    }

    void Update()
    {
        List<Note> notes = NoteEditor.tableNow.getAllNotes((x) => true);
        view.RenderNotes(notes);

        if (PopupLock) return;
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z)) history.Undo();
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y)) history.Redo();
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D)) Duplicate();
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C)) Copy();
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V)) Paste();
        if (Input.GetKeyDown(KeyCode.DownArrow) && vertical) MoveLeftSelected();
        if (Input.GetKeyDown(KeyCode.LeftArrow) && !vertical) MoveLeftSelected();

        if (Input.GetKeyDown(KeyCode.UpArrow) && vertical) MoveRightSelected();
        if (Input.GetKeyDown(KeyCode.RightArrow) && !vertical) MoveRightSelected();

        if (Input.GetKeyDown(KeyCode.LeftArrow) && vertical) MoveUpSelected();
        if (Input.GetKeyDown(KeyCode.UpArrow) && !vertical) MoveUpSelected();

        if (Input.GetKeyDown(KeyCode.RightArrow) && vertical) MoveDownSelected();
        if (Input.GetKeyDown(KeyCode.DownArrow) && !vertical) MoveDownSelected();
        if (Input.GetKeyDown(KeyCode.Delete)) DeleteNotes();
        if (Input.GetKeyDown(KeyCode.F3)) MoveMirror();
        if (Input.GetKeyDown(KeyCode.F4)) SnapSelected();
        if (Input.GetKeyDown(KeyCode.F5)) SnapLengthSelected();
        if (Input.GetKeyDown(KeyCode.F6)) MoveRandom();
        if (Input.GetKeyDown(KeyCode.LeftBracket)) MoveLeftSelectedMs();
        if (Input.GetKeyDown(KeyCode.RightBracket)) MoveRightSelectedMs();

        //단축키
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwapMode(Mode.ADD);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwapMode(Mode.DELETE);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwapMode(Mode.SELECT);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SwapMode(Mode.NOTE);

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            foreach (var i in selected_notes) i.data.type = 1;
        }
#endif
        //        if (Input.GetKeyDown(KeyCode.Insert)) InsertEvent();


        if (view.GetEditorNote(Input.mousePosition) == null && Mode.NOTE == mode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                selected_notes.Clear();
                selected_events.Clear();
            }
        }


    }

    KeyCode[] keys = { KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Semicolon };
    Note[] automation_state = { null, null, null, null, null, null };
    private void AutomationProcess()
    {
        for (int i = 0; i < 6; i++)
        {
            KeyCode key = keys[i];
            if (Input.GetKeyDown(key) && automation_state[i] == null)
            {
                var note = automation_state[i] = new Note(new NoteData());
                note.data.time = snaps.SnapRound(Game.time);
                note.data.y = i;
            }
            if (Input.GetKeyUp(key) && automation_state[i] != null)
            {
                var note = automation_state[i];
                note.data.length = snaps.SnapRound(Game.time) - note.data.time;
                automation_state[i] = null;
                history.Do(new AddCommand(note, false));
            }
        }

    }

    private static List<NoteData> clipboardNote = new List<NoteData>();
    private static List<EventData> clipboardEvent = new List<EventData>();
    private void Paste()
    {

        Debug.Log("PASTE:" + clipboardNote.Count);
        SwapMode(Mode.SELECT);
        var new_selected_notes = new List<Note>();
        var new_selected_events = new List<EventData>();
        foreach (var i in clipboardNote)
        {
            new_selected_notes.Add(new Note(new NoteData(i)));
        }
        foreach (var i in clipboardEvent)
        {
            new_selected_events.Add(i.Clone());
        }
        float now = snaps.Snap(Game.time);
        foreach (var i in new_selected_notes)
        {
            i.data.time = now + i.data.time;
        }
        foreach (var i in new_selected_events)
        {
            i.time = now + i.time;
        }
        //        selected_notes = new_selected_notes;
        EventManager.Select(new_selected_events);
        history.Do(new ComplexCommand(new AddCommand(new_selected_notes, true), new AddGameEventCommand(new_selected_events)));
    }
    bool isOver = false;

    void OnMouseEnter()
    {
        isOver = true;
    }
    private void OnMouseExit()
    {
        isOver = false;
    }

    private void Copy()
    {
        clipboardNote.Clear();
        clipboardEvent.Clear();
        float minv = float.PositiveInfinity;
        foreach (var i in selected_notes)
        {
            var data = new NoteData(i.data);
            minv = Mathf.Min(minv, data.time);
            clipboardNote.Add(data);
        }
        foreach (var i in selected_events)
        {
            var data = i.Clone();
            minv = Mathf.Min(minv, data.time);
            clipboardEvent.Add(data);
        }
        minv = snaps.Snap(minv);
        foreach (var i in clipboardNote)
        {
            i.time = i.time - minv;
        }
        foreach (var i in clipboardEvent)
        {
            i.time = i.time - minv;
        }
    }


    private void MoveMirror()
    {
        var miny = selected_notes.Min((x) => x.data.y);
        var maxy = selected_notes.Max((x) => x.data.y + x.data.x);
        ChangeSelected((data) =>
        {
            data.y = ((maxy - (data.y - miny) - data.x));
        });
    }


    private void MoveRandom()
    {
        //새로운 복제로 바꾼다
        var after = new List<NoteData>();
        var before = selected_notes.Select(x => new NoteData(x.data)).ToList();
        after = selected_notes.Select((x) => new NoteData(x.data)).ToList();
        after = new RandomizerDefault(0, 6).Convert(after);
        // after는 랜더마이징된 노트
        var dc = new ChangeNoteCommand(before, after);
        history.Do(dc);
    }
    private void SnapSelected()
    {
        ChangeSelected((data) =>
        {
            data.time = snaps.SnapRound(data.time);
        }, (e) => e.time = snaps.SnapRound(e.time));

    }
    private void SnapLengthSelected()
    {
        ChangeSelected((data) =>
        {
            data.length = snaps.SnapRound(data.time + data.length) - data.time;
        });
    }
    private void MoveUpSelected()
    {
        ChangeSelected((data) => { data.y = (data.y + Game.lineCount - 1) % Game.lineCount; });
    }

    private void MoveDownSelected()
    {
        ChangeSelected((data) => { data.y = (data.y + Game.lineCount + 1) % Game.lineCount; });
    }

    private void MoveLeftSelectedMs()
    {
        ChangeSelected((data) => { data.time -= 0.001F; });
    }

    private void MoveRightSelectedMs()
    {
        ChangeSelected((data) => { data.time += 0.001F; });
    }
    private void MoveRightSelected()
    {
        float prev = SelectedMinTime();
        if (Input.GetKey(KeyCode.LeftControl))
        {
            float next = prev;
            for (int i = 0; i < Game.table.timeSignatureNumerator * divide; i++)
                next = snaps.SnapNextBeat(next);
            float offset = next - prev;
            ChangeSelected(
                (data) =>
                {
                    data.time += offset;
                },
                (data) =>
                {
                    data.time += offset;
                }
                );
        }
        else
        {
            float next = snaps.SnapNextBeat(prev);
            float offset = next - prev;
            ChangeSelected(
                (data) =>
                {
                    data.time += offset;
                },
                (data) =>
                {
                    data.time += offset;
                }
                );
        }
    }
    private void MoveLeftSelected()
    {
        float prev = SelectedMinTime();
        if (Input.GetKey(KeyCode.LeftControl))
        {
            float next = prev;
            for (int i = 0; i < Game.table.timeSignatureNumerator * divide; i++)
                next = snaps.SnapPrevBeat(next);
            float offset = next - prev;
            ChangeSelected(
                (data) =>
                {
                    data.time += offset;
                },
                (data) =>
                {
                    data.time += offset;
                }
                );
        }
        else
        {
            float next = snaps.SnapPrevBeat(prev);
            float offset = next - prev;
            ChangeSelected(
                (data) =>
                {
                    data.time += offset;
                },
                (data) =>
                {
                    data.time += offset;
                }
                );
        }
    }

    private float SelectedMinTime()
    {
        float a = float.MaxValue, b = float.MaxValue;
        if (selected_notes.Count != 0)
        {
            a = selected_notes.Min((x) => x.data.time);
        }
        if (selected_events.Count != 0)
        {
            b = selected_events.Min((x) => x.time);
        }
        return Mathf.Min(a, b);
    }

    private void ChangeSelected(Action<NoteData> modify)
    {
        ChangeSelected(modify, (x) => { });
    }
    private void ChangeSelected(Action<NoteData> modifyNote, Action<EventData> modifyEvent)
    {
        var next_data = new List<NoteData>();
        var next_event = new List<EventData>();
        foreach (var i in selected_notes)
        {
            var data = new NoteData(i.data);
            modifyNote(data);
            next_data.Add(data);
        }
        foreach (var i in selected_events)
        {
            var data = i.Clone();
            modifyEvent(data);
            next_event.Add(data);
        }
        var prev_data = selected_notes.Select(x => new NoteData(x.data)).ToList();
        var nc = new ChangeNoteCommand(prev_data, next_data);
        var ec = new ChangeGameEventCommand(selected_events, next_event);
        history.Do(new ComplexCommand(nc, ec));
        if (isInvalid(selected_notes, selected_events)) history.Cancel();
    }
    private bool isInvalid(List<Note> notes, List<EventData> events)
    {
        bool time_invalid_note = selected_notes.Exists((i) => !(i.data.time >= 0 && i.data.time <= MusicPlayer.expandedlength));
        bool line_invalid_note = selected_notes.Exists((i) => !(i.data.y >= 0 && i.data.y + i.data.x < Game.lineCount));

        bool time_invalid_event = selected_events.Exists((i) => !(i.time >= 0 && i.time <= MusicPlayer.expandedlength));
        return time_invalid_note || line_invalid_note || time_invalid_event;
    }

    private void ShowSelectedMode()
    {
    }



    private void DeleteNotes()
    {
        ComplexCommand cmd = new ComplexCommand();
        cmd.Push(new DeleteCommand(selected_notes));
        cmd.Push(new DeleteGameEventCommand(selected_events));
        history.Do(cmd);
        selected_notes.Clear();
        selected_events.Clear();
    }

    private void Duplicate()
    {
        var new_selected_notes = new List<Note>();
        var new_selected_events = new List<EventData>();
        float minv = float.PositiveInfinity;
        float maxv = 0;
        foreach (var i in selected_notes)
        {
            var data = new NoteData(i.data);
            minv = Mathf.Min(minv, data.time);
            maxv = Mathf.Max(maxv, data.time);
            var note = new Note(data);
            new_selected_notes.Add(note);
        }
        foreach (var i in selected_events)
        {
            var data = i.Clone();
            minv = Mathf.Min(minv, data.time);
            maxv = Mathf.Max(maxv, data.time);
            new_selected_events.Add(data);
        }
        foreach (var i in new_selected_notes)
        {
            i.data.time = i.data.time + maxv - minv;
        }
        foreach (var i in new_selected_events)
        {
            i.time = i.time + (maxv - minv);
        }
        selected_notes = new_selected_notes;
        EventManager.Select(new_selected_events);
        history.Do(new ComplexCommand(new AddCommand(new_selected_notes, true), new AddGameEventCommand(new_selected_events)));
    }


    public static void SwapMode(int to)
    {
        SwapMode((Mode)to);
    }
    public static void SwapMode(Mode to)
    {
        mode = to;
        if (to != Mode.SELECT && to != Mode.NOTE)
        {
            selected_notes.Clear();
        }
    }

    private Note GetNoteFrom(Vector3 start, Vector3 end)
    {
        Vector3 pivot = transform.position;

        int a = Mathf.FloorToInt((pivot - start).y / (6F / Game.lineCount));
        int b = Mathf.FloorToInt((pivot - end).y / (6F / Game.lineCount));
        int c = Mathf.FloorToInt((pivot - end).y / (6F / Game.lineCount));
        float start_time = GetTimeFromPosition(start);
        float end_time = GetTimeFromPosition(end); // 노트 끝지점
        var note = new Note(new NoteData());
        note.data.time = Mathf.Clamp(start_time, 0, MusicPlayer.expandedlength);
        note.data.length = Mathf.Max(0, end_time - start_time);
        note.data.x = Mathf.Abs(b - a);
        note.data.y = c + Mathf.Min(0, a - b);
        note.data.color = Utility.DefaultColor(note.data);
        note.data.lineColorStart = Utility.DefaultLineColorStart(note.data);
        note.data.lineColorEnd = Utility.DefaultLineColorEnd(note.data);
        return note;
    }
    private float GetTimeFromPosition(Vector3 position)
    {
        return snaps.Snap((position - transform.position).x / zoom);// 노트 끝지점

    }

    private void OnMouseOver()
    {
        if (PopupLock) return;
        isOver = true;
        Vector3 mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //마우스를 위치를 찾음
        EditorNote select = view.GetEditorNote(mouse_pos);
        if (select != null) select.SetColor(Color.white);
        if (mode == Mode.DELETE)
        {
            /*면적이 가장 작은걸 select하여 삭제*/
            if (select != null)
            {
                select.SetColor(Color.red);
                if (Input.GetMouseButtonDown(0))
                {
                    history.Do(new DeleteCommand(select.note));
                }
            }
        }

        if (mode == Mode.NOTE)
        {

            if (select != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (selected_notes.Contains(select.note))
                    {
                        noteEditPanel.Open(selected_notes);
                    }
                    else
                    {
                        var list = new List<Note>();
                        list.Add(select.note);
                        noteEditPanel.Open(list);
                    }
                    selected_notes.Clear();
                }
            }
            //표시되는 노트의 범위를 확인
            //두가지를 이용해서 마우스가 닿은 경우 오브젝트를 빨갛게 변경
        }

    }


    //    public Canvas canvas;
    public void SetStep(int divide)
    {
        NoteEditor.divide = divide;
    }

    public void SetStartBPM(string value)
    {
        float bpm = float.Parse(value, CultureInfo.InvariantCulture);
        Game.table.start_bpm = bpm;
        sketch.start_bpm = bpm;
        RefreshBPMS();
    }
}
public class TimeDistance
{
    public enum Type { NONE, LINEAR }
    public float DistancePerBeat = 1F;
    public float TimeSignatureMultiplier = 1F;
    List<(float, float, Type)> bpms;
    List<(float, float, Type)> psum;
    float start_bpm = 120;
    public TimeDistance(float start_bpm)
    {
        this.start_bpm = start_bpm;
        bpms = new List<(float, float, Type)>();
        psum = new List<(float, float, Type)>();
        Refresh();
    }
    public override string ToString()
    {
        string t = "";
        foreach (var i in bpms)
        {
            t += $"[{i.Item1},{i.Item2}],";
        }
        return t;
    }
    public void SetStartBPM(float bpm)
    {
        start_bpm = bpm;
        CalcPsum();
    }
    void Refresh()
    {
        if (!bpms.Exists((x) => x.Item1 == 0))
        {
            bpms.Add((0, start_bpm, Type.NONE));
        }
        CalcPsum();
    }
    public void SetBPMs(List<(float, float, Type)> list, float start_bpm)
    {
        Debug.Log("Set BPMS");
        this.start_bpm = start_bpm;
        bpms = new List<(float, float, Type)>();
        foreach (var i in list)
        {
            bpms.Add((i.Item1, i.Item2, i.Item3));
        }
        Refresh();
    }
    void CalcPsum()
    {
        bpms.Sort();
        psum = new List<(float, float, Type)>();
        if (bpms.Count == 0) return;
        (float, float, Type) prev = bpms[0];
        (float, float, Type) psum_now = (0, 0, Type.NONE);
        psum.Add(psum_now);
        foreach (var i in bpms)
        {
            psum_now.Item2 += Calc(prev, i, i.Item1); // 직전 시간 bpm * 시간 차이
            psum_now.Item1 = i.Item1;
            psum.Add(psum_now);
            prev = i;
        };
    }
    public float TimeToDistance(float time) // bpms의 값이 음수인 경우 또한 고려함
    {
        //        time /= TimeSignatureMultiplier;
        var _psum = search(psum, time).Item2;
        var l = search(bpms, time, false);
        var r = search(bpms, time, true);
        float dist = (_psum + Calc(l, r, time)) * DistancePerBeat;
        return dist;
    }
    float Calc((float, float, Type) a, (float, float, Type) b, float time)
    {
        float _time = a.Item1;
        float bpm = a.Item2;
        float bpmR = b.Item2;
        float dist = bpm * (time - _time) / 60F;
        if (b.Item3 == Type.LINEAR)
        {
            float t = Mathf.InverseLerp(a.Item1, b.Item1, time);
            float bpmT = Mathf.Lerp(bpm, bpmR, t);
            dist += (bpmT - bpm) * (time - _time) / 2F / 60F;//삼각형 차이
        }
        return dist;
    }
    public float DistanceToTime(float distance)// bpms의 값이 음수인 경우는 고려 안함
    {
        distance /= DistancePerBeat;
        var L = from item in psum orderby item.Item2 ascending select (item.Item2, item.Item1, item.Item3);
        // 굳이 정렬할 필요는 없는데 이진탐색이라..

        (float, float, Type) left = search(L.ToList(), distance);
        //distance 보다 작은 가장 큰 Psum을 찾는다

        float time = left.Item2;
        var right = search(bpms, time);
        float bpm = search(bpms, time).Item2;
        //그 Psum의 time과 일치하는 bpm을 찾는다 
        return (time + Calc2(left, right, distance));
        //(distance-Psum)/bpm + time을 반환한다
    }
    float Calc2((float, float, Type) a, (float, float, Type) b, float distance)
    {
        return (distance - a.Item1) / (b.Item2 / 60 * 1);
    }
    (float, float, Type) search(List<(float, float, Type)> L, float target, bool get_next = false) // 비교함수는 오름차순일 때 x<y을 반환
    {
        int left = 0;
        int right = L.Count - 1;
        while (left != right)
        {
            int mid = (left + right + 1) / 2;
            if (target < L[mid].Item1) right = mid - 1;
            else left = mid;
        }
        if (!get_next)
        {
            return L[Math.Max(left, 0)];
        }
        else return L[Math.Min(left + 1, L.Count - 1)];
    }
    public float Snap(float time, float divide)
    {
        divide *= Game.table.timeSignatureNumerator;
        var pair = search(bpms, time);
        float bpm = pair.Item2;
        float secperbeat = 60 / bpm;
        float step = Game.table.timeSignatureNumerator * (secperbeat) / divide;
        //Round(time,step);
        return time - (time - pair.Item1) % step;
    }

    public float SnapRound(float time, float divide)
    {
        divide *= Game.table.timeSignatureNumerator;
        var pair = search(bpms, time);
        float bpm = pair.Item2;
        float secperbeat = 60 / bpm;
        float step = Game.table.timeSignatureNumerator * (secperbeat) / divide;

        float t1 = time - (time - pair.Item1) % step;

        pair = search(bpms, time);
        bpm = pair.Item2;
        secperbeat = 60 / bpm;
        step = Game.table.timeSignatureNumerator * (secperbeat) / divide;

        float t2 = time - (time - pair.Item1) % step + step;
        if (Mathf.Abs(t1 - time) > Mathf.Abs(t2 - time))
        {
            return t2;
        }
        else
        {
            return t1;
        }
    }
    public float SnapDistance(float distance, float divide)
    {
        float time = Snap(DistanceToTime(distance), divide);
        return TimeToDistance(time);
    }

}

public interface EditorCommand
{
    void Undo();
    void Redo();
    void Do();
}
public enum Cmd { ADD, DEL }

public class SketchCommand : EditorCommand
{
    bool prev = false;
    bool next = false;
    public SketchCommand(bool prev, bool next)
    {
        this.prev = prev;
        this.next = next;
    }
    public void Undo()
    {
        Debug.Log("undo:" + prev);
        NoteEditor.sketchmode = prev;
        NoteEditor.RefreshBPMS();
    }
    public void Redo()
    {
        Debug.Log("redo:" + next);
        NoteEditor.sketchmode = next;
        NoteEditor.RefreshBPMS();
    }
    public void Do()
    {
        Redo();
    }
}

public class AddCommand : EditorCommand
{
    List<NoteData> target;
    bool nested = false;
    public AddCommand(Note note, bool nested)
    {
        target = new List<NoteData>();
        target.Add(new NoteData(note.data));
        this.nested = nested;
    }
    public AddCommand(List<Note> notes, bool nested)
    {
        target = new List<NoteData>();
        foreach (var i in notes)
        {
            target.Add(new NoteData(i.data));
        }
        this.nested = nested;
    }
    public void Undo()
    {
        foreach (var i in target)
        {
            NoteEditor.tableNow.deleteNote(i);
        }
    }
    public void Redo()
    {
        NoteEditor.selected_notes.Clear();
        foreach (var i in target)
        {
            var note = new Note(new NoteData(i));
            NoteEditor.tableNow.addNote(note, nested);
            NoteEditor.selected_notes.Add(note);
        }
    }
    public void Do()
    {
        Redo();
    }
}

public class DeleteCommand : EditorCommand
{
    List<NoteData> target;
    public DeleteCommand(Note note)
    {
        target = new List<NoteData>();
        target.Add(new NoteData(note.data));
    }
    public DeleteCommand(List<Note> notes)
    {
        target = new List<NoteData>();
        foreach (var i in notes)
        {
            target.Add(new NoteData(i.data));
        }
    }
    public void Undo()
    {
        NoteEditor.selected_notes.Clear();
        foreach (var i in target)
        {
            var note = new Note(i);
            NoteEditor.tableNow.addNote(note, true);
            NoteEditor.selected_notes.Add(note);
        }
    }
    public void Redo()
    {
        foreach (var i in target)
        {
            NoteEditor.tableNow.deleteNote(i);
        }
    }
    public void Do()
    {
        Redo();
    }
}

public class ChangeNoteCommand : EditorCommand
{
    List<NoteData> nexts;
    List<NoteData> prevs;
    public ChangeNoteCommand(List<NoteData> notes, List<NoteData> next)
    {
        prevs = new List<NoteData>();
        nexts = new List<NoteData>();

        foreach (var i in notes)
        {
            prevs.Add(new NoteData(i));
        }
        foreach (var i in next)
        {
            nexts.Add(new NoteData(i));
        }
    }
    public void Undo()
    {
        foreach (var i in nexts)
        {
            NoteEditor.tableNow.deleteNote(i);
        }
        NoteEditor.selected_notes.Clear();
        foreach (var i in prevs)
        {
            var note = new Note(i);
            NoteEditor.selected_notes.Add(note);
            NoteEditor.tableNow.addNote(note, true);
        }
    }
    public void Redo()
    {
        foreach (var i in prevs)
        {
            NoteEditor.tableNow.deleteNote(i);
        }
        NoteEditor.selected_notes.Clear();
        foreach (var i in nexts)
        {
            var note = new Note(i);
            NoteEditor.selected_notes.Add(note);
            NoteEditor.tableNow.addNote(note, true);
        }
    }
    public void Do()
    {
        Redo();
    }
}

public class SelectCommand : EditorCommand
{
    List<Note> prevNotes;
    List<Note> nextNotes;
    List<EventData> prevEvents;
    List<EventData> nextEvents;
    public SelectCommand(List<Note> prevNotes, List<Note> nextNotes, List<EventData> prevEvents, List<EventData> nextEvents)
    {
        this.prevNotes = prevNotes;
        this.nextNotes = nextNotes;
        this.prevEvents = prevEvents;
        this.nextEvents = nextEvents;
    }
    public SelectCommand(List<Note> prevNotes, List<Note> nextNotes)
    {
        this.prevNotes = prevNotes;
        this.nextNotes = nextNotes;
        this.prevEvents = GameEventEditor.selected;
        this.nextEvents = GameEventEditor.selected;
    }
    public void Do()
    {
        NoteEditor.selected_notes = nextNotes;
        GameEventEditor.selected = nextEvents;
    }

    public void Redo()
    {
        Do();
    }

    public void Undo()
    {
        NoteEditor.selected_notes = prevNotes;
        GameEventEditor.selected = prevEvents;
    }
}
public class ComplexCommand : EditorCommand
{
    List<EditorCommand> cmds = new List<EditorCommand>();
    public ComplexCommand()
    {
        cmds = new List<EditorCommand>();
    }
    public ComplexCommand(EditorCommand A)
    {
        cmds = new List<EditorCommand>();
        cmds.Add(A);
    }
    public ComplexCommand(EditorCommand A, EditorCommand B)
    {
        cmds = new List<EditorCommand>();
        cmds.Add(A);
        cmds.Add(B);
    }

    public ComplexCommand(EditorCommand A, EditorCommand B, EditorCommand C)
    {
        cmds = new List<EditorCommand>();
        cmds.Add(A);
        cmds.Add(B);
        cmds.Add(C);
    }
    public void Push(EditorCommand cmd)
    {
        cmds.Add(cmd);
    }
    public void Undo()
    {
        foreach (var i in cmds)
        {
            i.Undo();
        }
    }
    public void Redo()
    {
        foreach (var i in cmds)
        {
            i.Redo();
        }
    }
    public void Do()
    {
        Redo();
    }
}
public class SnapGrid
{
    public override string ToString()
    {
        return "Snaps:" + JsonConvert.SerializeObject(grid.ToArray());
    }
    public enum GridType { Node, Thick, Thin };
    public struct Grid
    {
        public float time;
        public GridType GridType;
    }
    List<Grid> grid = new List<Grid>();
    List<EventData> bpms = new List<EventData>();
    public void CalcGrid()
    {
        grid.Clear();
        bpms.Clear();
        int divide = NoteEditor.divide;
        bpms = NoteEditor.tableNow.getEvents((x) => x.isUse("bpm"));

        if (!bpms.Exists((x) => x.time == 0))
        {
            bpms.Add(new EventData() { bpm = Game.table.start_bpm, time = 0 });
            bpms.Sort((x, y) => Math.Sign(x.time - y.time));
        }
        bpms.Add(new EventData() { bpm = Game.table.start_bpm, time = MusicPlayer.expandedlength });

        for (int i = 0; i < bpms.Count - 1; i++)
        {
            double left = bpms[i].time; //시작점
            double right = bpms[i + 1].time; // 끝점
            double step = 60.0 / bpms[i].bpm / NoteEditor.divide;
            int cnt = 0;
            for (double t = left; t < right; t += step)
            {
                int n = Mathf.RoundToInt(Game.table.timeSignatureNumerator) * divide;
                if (cnt % n == 0)
                {
                    grid.Add(new Grid() { time = (float)t, GridType = GridType.Node });
                }
                else if (cnt % divide == 0)
                {
                    grid.Add(new Grid() { time = (float)t, GridType = GridType.Thick });
                }
                else
                {
                    grid.Add(new Grid() { time = (float)t, GridType = GridType.Thin });
                }
                cnt++;
            }
        }
        Debug.Log("CalcGrid:" + this);

        // 그리드를 계산한다
    }
    public float SnapCeil(float time)
    {
        return grid.FirstOrDefault(x => x.time >= time).time;
    }
    public float SnapFloor(float time)
    {
        time = Mathf.Max(time, 0);
        return grid.Last(x => x.time <= time).time;
    }
    public float Snap(float time)
    {
        return SnapFloor(time);
    }
    public float SnapRound(float time)
    {
        if (Mathf.Abs(time - SnapCeil(time)) < Mathf.Abs(time - SnapFloor(time)))
        {
            return SnapCeil(time);
        }
        else
        {
            return SnapFloor(time);
        }
    }

    internal float SnapNextBeat(float time)
    {
        time = SnapRound(time);
        return (float)grid.First(x => x.time > time + 0.01).time;
    }

    internal float SnapPrevBeat(float time)
    {
        time = SnapRound(time);
        return (float)grid.Last(x => x.time < time - 0.01).time;
    }

    internal List<Grid> GetGrids()
    {
        return grid.ToList();
    }
}
public class CommandHistory
{
    public Stack<EditorCommand> undo_commands = new Stack<EditorCommand>();
    public Stack<EditorCommand> redo_commands = new Stack<EditorCommand>();

    public void Do(EditorCommand command)
    {
        Debug.Log("Do");
        DirtyChecker.MakeDirty();
        command.Do();
        undo_commands.Push(command);
        redo_commands.Clear();
    }

    public void Undo()
    {
        Debug.Log("Uo");

        DirtyChecker.MakeDirty();
        if (undo_commands.Count > 0)
        {
            var elem = undo_commands.Pop();
            elem.Undo();
            redo_commands.Push(elem);
            Debug.Log("undo:" + (undo_commands.Count, redo_commands.Count));
        }
    }
    public void Redo()
    {
        DirtyChecker.MakeDirty();
        if (redo_commands.Count > 0)
        {
            var elem = redo_commands.Pop();
            elem.Redo();
            undo_commands.Push(elem);
            Debug.Log("redo:" + (undo_commands.Count, redo_commands.Count));
        }
    }
    public void Cancel() // 뭔가 잘못된 명령을 했을때 롤백하는 용도
    {
        Undo();
        redo_commands.Clear();
    }

    public void Clear()
    {
        undo_commands.Clear();
        redo_commands.Clear();
    }
}
