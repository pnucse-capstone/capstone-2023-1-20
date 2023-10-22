using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NoteDataEditEntry : MonoBehaviour
{
    ColorPickerAdapter color_picker
    {
        get
        {
            Debug.Log(transform.parent.name);
            return transform.parent.parent.parent.GetComponentInChildren<ColorPickerAdapter>(true);
        }
    }
    enum DataType { Number, Dropdown, None,
        Toggle,Color,Slider,Integer
    }
    [SerializeField]
    DataType datatype;
    [SerializeField] Selectable InputUI;
    // Start is called before the first frame update
    public string typecode;
    FieldInfo field => typeof(NoteData).GetField(typecode);
    public bool dirty = false;
    bool multi = false;
    public void ShowEvent(List<Note> data)
    {
        if(data.Count == 1)
        {
            var value = field.GetValue(data[0].data);
            if (value != null)
            {
                Show(value);
            }
            multi = false;
        }
        else
        {
            ShowDefault();
            multi = true;
        }
        dirty = false;
    }
    public void BeginEditForNoteColor()
    {
        if(multi)BeginEdit();
    }
    void ShowDefault()
    {
        InputUI.interactable = false;


        if (datatype == DataType.Number)
        {
            ((InputField)InputUI).text = "";
        }
        else if (datatype == DataType.Dropdown)
        {
            ((Dropdown)InputUI).value = 0;
        }
        else if (datatype == DataType.Toggle)
        {
            ((Toggle)InputUI).isOn = false;
        }
        else if (datatype == DataType.Color)
        {
            ((Button)InputUI).image.color = Color.white;
        }
        else if (datatype == DataType.Slider)
        {
            ((Slider)InputUI).value = 0F;
        }

        else if (datatype == DataType.Integer)
        {
            ((InputField)InputUI).text = "";
        }
        else
        {
            throw new NotImplementedException();
        }
    }
    void Start()
    {

        if (datatype == DataType.Color)
        {
            var bt = (Button)InputUI;
            bt.onClick.RemoveAllListeners();
            bt.onClick.AddListener(() => {
                Debug.Log(color_picker);
                color_picker.OpenColorPicker((color) =>
                {
                    Debug.Log(bt);
                    bt.image.color = color;
                }
                , bt.image.color);
                dirty = true;
            });
        }
    }
    private void Show(object value)
    {
        if (datatype == DataType.Number)
        {
            ((InputField)InputUI).text = value.ToString();
        }
        else if (datatype == DataType.Dropdown)
        {
            ((Dropdown)InputUI).value = (int)value;
        }
        else if (datatype == DataType.Toggle)
        {
            ((Toggle)InputUI).isOn = (bool)value;
        }
        else if (datatype == DataType.Color)
        {
            ((Button)InputUI).image.color = (ColorAdapter)value;
        }
        else if (datatype == DataType.Slider)
        {
            ((Slider)InputUI).value = (float)value;
        }
        else if (datatype == DataType.Integer)
        {
            ((InputField)InputUI).text = value.ToString();
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public void WriteEvent(List<NoteData> select)
    {
        if (dirty)
        {
            foreach(var i in select)
            {
                object value = UIValue();
                field.SetValue(i, value);
            }
        }
    }
    public object UIValue()
    {
        
        if (datatype == DataType.Number)
        {
            return float.Parse(((InputField)InputUI).text);
        }
        else if (datatype == DataType.Dropdown)
        {
            return ((Dropdown)InputUI).value;
        }
        else if (datatype == DataType.Toggle)
        {
            return ((Toggle)InputUI).isOn;
        }
        else if (datatype == DataType.Color)
        {
            return (ColorAdapter)((Button)InputUI).image.color;
        }
        else if (datatype == DataType.Slider)
        {
            return (float)((Slider)InputUI).value;
        }
        else if (datatype == DataType.Integer)
        {
            return int.Parse(((InputField)InputUI).text);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public void BeginEdit()
    {
        Debug.Log("down");
        dirty = true;
        
        InputUI.interactable = true;
    }
}
