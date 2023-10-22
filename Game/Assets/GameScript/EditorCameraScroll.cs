using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EditorCameraScroll : MonoBehaviour
{

    Vector3 o;
    [SerializeField]GameObject canvas;
    [SerializeField] GameObject vertical;
    [SerializeField] GameObject setting;
    [SerializeField] GameObject workshop;
    [SerializeField] GameObject save;
    public static float scroll_pos = 0F;
    public static float max_pos 
    { 
        get => MusicPlayer.isReady ? MusicPlayer.expandedlength : 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        o = transform.position;
    }
    public void ZoomUp()
    {
        NoteEditor.zoom *= 2;
        scroll_pos *= 2;
    }
    public void ZoomDown()
    {
        NoteEditor.zoom /= 2;
        scroll_pos /= 2;
    }
    [SerializeField] Toggle toggle_follow;
    bool follow = false;
    public void ToggleFollow(bool value)
    {
        follow = value;
    }
    [SerializeField] Slider pos_slider;
    public void SetRatePos(float v)
    {
        scroll_pos = v*max_pos;
    }

    public static void ScrollFocusToNow()
    {
        scroll_pos = Game.time;
    }
    // Update is called once per frame
    void Update()
    {

        if (NoteEditor.vertical)
        {
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            ScrollFocusToNow();
        }
        if (NoteEditor.focus == 0)
        {
            if (!EventSystem.current.IsPointerOverGameObject() && !Input.GetKey(KeyCode.LeftControl))
                scroll_pos += Input.mouseScrollDelta.y/NoteEditor.zoom;
            if (follow && Game.isPlaying)
                ScrollFocusToNow();
            canvas.SetActive(!NoteEditor.vertical);
            vertical.SetActive(NoteEditor.vertical);
            setting.SetActive(false);
            workshop.SetActive(false);
            save.SetActive(true);

            MoveCameraToFocusPosition();
        }
        if (NoteEditor.focus == 1)
        {
            Vector3 pos = setting.transform.position;
            pos.z = transform.position.z;
            transform.position = pos;
            canvas.SetActive(false);
            vertical.SetActive(false);
            setting.SetActive(true);
            workshop.SetActive(false);
            save.SetActive(false);
        }

        if (NoteEditor.focus == 2)
        {
            Vector3 pos = workshop.transform.position;
            pos.z = transform.position.z;
            transform.position = pos;
            canvas.SetActive(false);
            vertical.SetActive(false);
            setting.SetActive(false);
            workshop.SetActive(true);
            save.SetActive(false);

        }
        if (NoteEditor.focus != 0 && Input.GetKeyDown(KeyCode.Escape))
        {
            SetFocus(0);
        }
        pos_slider.value = max_pos == 0 ? 0 : scroll_pos / max_pos;
    }
    
    private void MoveCameraToFocusPosition()
    {
        scroll_pos = Mathf.Clamp(scroll_pos, 0, max_pos);
        Vector3 pos = o + Vector3.right * scroll_pos * NoteEditor.zoom;
        if (NoteEditor.vertical)
        {
            pos += Camera.main.orthographicSize * Vector3.left/2F;
            pos += Camera.main.orthographicSize * Vector3.down/3F;
        }
        
        pos.z = transform.position.z;
        transform.position = pos;
    }

    public void SetFocus(int i)
    {
        NoteEditor.PopupLock = i != 0;
        NoteEditor.focus = i;
        Update();
    }
}
