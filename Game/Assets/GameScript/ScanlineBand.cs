using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.Expando;
using UnityEngine;

public class ScanlineBand : MonoBehaviour
{
    public GameObject scanline;
    public GameObject pivot;
    AudioSourceExapnd expand;
    void Start()
    {
        Refresh();
    }
    public void Refresh()
    {
        if(expand != null)
        {
            expand.Stop();
            Game.isPlaying = false;
        }
        ResizeCollider();

        expand = new AudioSourceExapnd();
    }

    void ResizeCollider()
    {
        float width = (MusicPlayer.expandedlength)*NoteEditor.zoom;
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Vector3 size = collider.size;
        Vector3 offset = collider.offset;
        offset.x = width / 2F;
        size.x = width;
        collider.size = size;
        collider.offset = offset;
    }
    void OnMouseDrag()
    {
        if (NoteEditor.PopupLock)
        {
            return;
        }
        float length = MusicPlayer.expandedlength;
        var v = Camera.main.ScreenToWorldPoint(Input.mousePosition)- transform.position;
        float time= v.x/NoteEditor.zoom;
        time = Mathf.Clamp(time, 0, length);
        Game.time = time;
        expand.time = time - Game.table.offset;
        var view = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        if (view.x>0.95F)EditorCameraScroll.scroll_pos += (view.x-0.95F)*2F;
        if (view.x < 0.05F) EditorCameraScroll.scroll_pos += (view.x - 0.05F)*2F;
    }
    public void ToStart()
    {
        Game.time = 0;
        EditorCameraScroll.scroll_pos = 0;
        expand.time = -Game.table.offset;
    }
    public void ToEnd()
    {
        Game.time = MusicPlayer.expandedlength;
        EditorCameraScroll.scroll_pos = EditorCameraScroll.max_pos;
        expand.time = Game.time - Game.table.offset;
    }

    void Update()
    {
        if (NoteEditor.PopupLock) return;
        if (expand != null)expand.Update();
        if (Input.GetKeyDown(KeyCode.Home)) ToStart();
        if (Input.GetKeyDown(KeyCode.End)) ToEnd();
        if (Input.GetKeyDown(KeyCode.PageUp)) ToJumpLeft();
        if (Input.GetKeyDown(KeyCode.PageDown)) ToJumpRight();
        scanline.transform.localPosition = Game.time * Vector3.right * NoteEditor.zoom;

        if (NoteEditor.focus == 0 && !Input.GetKey(KeyCode.LeftControl))
        {

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S))
            {
                ScanlineLeft();
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W))
            {
                ScanlineRight();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Game.isPlaying)
                {
                    ScanlinePause();
                }
                else
                {
                    ScanlinePlay();
                }
            }
            
        }
        if (Game.isPlaying)
        {
            Game.time += Time.deltaTime*Game.playSpeed;
        }
        Game.time = Mathf.Clamp(Game.time, 0, MusicPlayer.expandedlength);
    }

    private void ScanlineLeft()
    {
        SetTime(Game.time + ((Input.GetKey(KeyCode.LeftShift) ? -10F : -3F)*Time.deltaTime / NoteEditor.zoom));
    }
    public void SetTime(float time)
    {
        Game.time = time;
        Game.time = Mathf.Clamp(Game.time, 0, MusicPlayer.expandedlength);
        expand.time = Game.time - Game.table.offset;
    }

    private void ScanlineRight()
    {
        SetTime(Game.time + (Input.GetKey(KeyCode.LeftShift) ? 10F : 3F) * Time.deltaTime / NoteEditor.zoom);
    }

    private void ScanlinePause()
    {
        Game.isPlaying = false;
        expand.Stop();
    }

    private void ScanlinePlay()
    {
        Game.isPlaying = true;
        SetTime(Game.time);
        expand.Play();
    }

    private void ToJumpRight()
    {
        SetTime(Game.time + 5);
        expand.Adapt();
        EditorCameraScroll.ScrollFocusToNow();
    }

    private void ToJumpLeft()
    {
        SetTime(Game.time - 5);
        expand.Adapt();
        EditorCameraScroll.ScrollFocusToNow();
    }
}

public class AudioSourceExapnd // 0보다 작거나 길이보다 커도 재생에 오류가 없음. 단 범위를 벗어나면 소리가 안남.
{
    float _time = 0;
    public float time
    {
        get
        {
            return _time;
        }
        set
        {
            _time = value;
            Adapt();
        }
    }
    public float length = 0;
    bool isPlaying = false;
    public AudioSourceExapnd()
    {
    }
    public void Play()
    {
        isPlaying = true;
        MusicPlayer.Resume();
        Adapt();
    }
    bool isWait = false;
    public void Adapt() // 현재 어댑터 상태에 맞게 source를 변경
    {
        if (!MusicPlayer.isReady) return;
        length = MusicPlayer.expandedlength;

        if (0 <= time && time <= MusicPlayer.GetAudioData().length)
        {
            MusicPlayer.SetPosition(time);
            if (isPlaying)
            {
                MusicPlayer.Resume();
            }
            if (!isPlaying)
            {
                MusicPlayer.Pause();
            }
        }
        else 
        {
            isWait = true;
            if (MusicPlayer.isPlaying)
            {
                MusicPlayer.Pause();
            }

        }
    }
    public void Update()
    {
        if (isPlaying)
        {
            _time += Game.playSpeed*Time.deltaTime;
        }
        if (isWait)
        {
            isWait = false;
            Adapt();
        }

        if (time > MusicPlayer.GetAudioData().length-0.1)
        {
            MusicPlayer.Pause();
        }
    }

    public void Stop()
    {
        isPlaying = false;
        if(MusicPlayer.isPlaying) 
            MusicPlayer.Pause();

        Adapt();
    }

}

