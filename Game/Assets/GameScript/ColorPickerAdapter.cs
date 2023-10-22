using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(FlexibleColorPicker))]

public class ColorPickerAdapter : MonoBehaviour
{
    Action<Color> color_picker_use;
    // Start is called before the first frame update
    FlexibleColorPicker color_picker;
    void Start()
    {
        color_picker = GetComponent<FlexibleColorPicker>();
        
        gameObject.SetActive(false);
    }

    public void OpenColorPicker(Action<Color> callback,Color now)
    {
        color_picker.color = now;
        gameObject.SetActive(true);
        transform.position = Input.mousePosition;
        color_picker_use = callback;        
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && color_picker_use != null)
        {
            var m_PointerEventData = new PointerEventData(EventSystem.current);
            m_PointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(m_PointerEventData, results);
            var cast = results.Count == 0 ? "" : results[0].gameObject.tag;
            Debug.Log(cast);
            if (cast != "ColorPicker")
            {
                gameObject.SetActive(false);
                Debug.Log(color_picker.color);
                color_picker_use(color_picker.color);
                color_picker_use = null;
            }
        }
    }
}
