using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventGraphic : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spr;
    [SerializeField] Color init;
    Color color;
    [SerializeField] Transform gear;
    public EventData data;
    
    // Start is called before the first frame update
    void Start()
    {
        color = init;
    }
    void OnMouseOver()
    {
        spr.color = Color.white;
    }
    void OnMouseExit()
    {
        spr.color = color;
    }
    public bool isInRect(Rect rect)
    {
        return rect.Overlaps(new Rect(gear.position,Vector2.zero), true);
    }
    public void SetVisible(bool value)
    {
        gameObject.SetActive(value);
    }
    public void SetEvent(EventData data)
    {
        this.data = data;

        transform.localPosition = Vector3.right * data.time * NoteEditor.zoom + Vector3.back;
        gameObject.SetActive(true);
    }

    public void SetColor(Color color)
    {
        this.color = color;
        spr.color = color;
    }
    public void ResetColor()
    {
        spr.color = init;
    }
}
