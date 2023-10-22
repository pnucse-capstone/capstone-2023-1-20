using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RolloverButton : MonoBehaviour, ISelectHandler
{
    [SerializeField]
    FMODSFX sfx;
    Animator ani;
    void Start()
    {
        ani = GetComponent<Animator>();
    }
    
    public void SetOn()
    {
        if (NoteEditor.PopupLock) return;
        ani.SetTrigger("Over");
        sfx.Play();
    }
    public void SetOff()
    {
        if (NoteEditor.PopupLock) return;
        ani.SetTrigger("Off");
    }
    public void SetUp()
    {
        if (NoteEditor.PopupLock) return;
    }
    public void SetDown()
    {
        if (NoteEditor.PopupLock) return;
        ani.SetTrigger("Down");
    }
    public void OnMouseEnter()
    {
        if (NoteEditor.PopupLock) return;
        GetComponentInParent<MainKeyboardSelect>().Deselect();
        sfx.Play();
    }
    public void OnMouseDown()
    {
        if (NoteEditor.PopupLock) return;
    }
    public void OnMouseUp()
    {
        if (NoteEditor.PopupLock) return;
        SetUp();
    }
    public void OnMouseExit()
    {
        if (NoteEditor.PopupLock) return;
        GetComponentInParent<MainKeyboardSelect>().Deselect();
    }

    public void OnSelect(BaseEventData eventData)
    {
        GetComponent<Button>().OnDeselect(eventData);
    }

    public void ResetButton()
    {
        ani.ResetTrigger("Off");
        ani.ResetTrigger("Over");
        ani.SetTrigger("Normal");
        ani.ResetTrigger("Down");
    }
}
