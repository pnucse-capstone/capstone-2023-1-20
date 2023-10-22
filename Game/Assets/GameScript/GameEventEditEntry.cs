using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Newtonsoft.Json;
[RequireComponent(typeof(Toggle))]
public class GameEventEditEntry : MonoBehaviour
{
    enum DataType {Number, Color, Dropdown,None,Path,Slider}
    [SerializeField]
    DataType datatype;
    ColorPickerAdapter color_picker
    {
        get
        {
            Debug.Log(transform.parent.name);
            return transform.parent.parent.parent.GetComponentInChildren<ColorPickerAdapter>(true);
        }
    }
    LinePathEditor linePathEditor
    {
        get
        {
            return transform.parent.parent.parent.GetComponentInChildren<LinePathEditor>(true);
        }
    }
    [SerializeField] List<Selectable> fields;
    public void SetFields(List<Selectable> fields)
    {
        this.fields = fields;
//        Init();
    }
    public List<Selectable> GetFields()
    {
        return fields;
    }
    Toggle toggle {
        get => GetComponent<Toggle>();
    }
    // Start is called before the first frame update
    public string typecode;
    GameEventT type { 
        get=> EventData.GetGameEventType(typecode);
    }
    void Init()
    {
        if(datatype == DataType.Color)
        {
            foreach (var i in fields)
            {
                var bt = (Button)i;
                bt.onClick.RemoveAllListeners();
                bt.onClick.AddListener(() => {
                    Debug.Log(color_picker);
                    color_picker.OpenColorPicker((color) =>
                    {
                        Debug.Log(bt);
                        bt.image.color = color;
                    }
                    , bt.image.color);
                });
            }
        }
        toggle.onValueChanged.AddListener(SetInteractable);
    }
    void SetInteractable(bool value)
    {
        foreach (var i in fields)
        {
            i.interactable = value;
        }
    }
    private void Start()
    {
        Init();
    }
    public void ShowEvent(EventData data)
    {
        Debug.Log(typecode);
        toggle.isOn = data.isUse(typecode); 
        Array list = type.Get(data);
        if (list != null)
            Show(data, list);
    }
    public void WriteEvent(EventData select)
    {
        select.Use(typecode, toggle.isOn);
        Array list = EditState();
        type.Set(select, list);
    }
    private void Show(EventData data, Array list)
    {
        for (int i = 0; i < fields.Count; i++)
        {
            if (datatype == DataType.Color)
            {
                fields[i].image.color = (ColorAdapter)list.GetValue(i);
            }
            else if (datatype == DataType.Number)
            {
                InputField tf = (InputField)fields[i];
                tf.text = (float)list.GetValue(i) + "";
            }
            else if (datatype == DataType.Dropdown)
            {
                Dropdown dropdown = (Dropdown)fields[i];
                if (i == 0)
                {
                    dropdown.value = Mathf.RoundToInt((float)list.GetValue(i));
                }
            }
            else if (datatype == DataType.Slider)
            {
                Slider slider = (Slider)fields[i];
                slider.value = (float)list.GetValue(i);
            }
            else if (datatype == DataType.None)
            {
            }
            else if (datatype == DataType.Path)
            {
                int j = i;
                var bt = (Button)fields[i];
                bt.onClick.RemoveAllListeners();
                bt.onClick.AddListener(() =>
                {
                    linePathEditor.Open(data, j,type);
                });
            }
            else
            {
                throw new NotImplementedException();
            }
            fields[i].interactable = data.isUse(typecode);
        }
    }

    private Array EditState()
    {
        Array list;
        if (datatype == DataType.Color)
        {
            list = fields.Select((x) => (ColorAdapter)x.image.color).ToArray();
            fields.ForEach((x) => x.interactable = toggle.isOn);
        }
        else if (datatype == DataType.Number)
        {
            list = fields.Select((x) =>
            {
                InputField tf = (InputField)x;
                return float.Parse(tf.text, CultureInfo.InvariantCulture);
            }).ToArray();
        }
        else if (datatype == DataType.Dropdown)
        {
            var dropdown = (Dropdown)fields[0];
            
            list = new float[] { dropdown.value, dropdown.value, 0 };
        }
        else if (datatype == DataType.Slider)
        {
            list = fields.Select((x) =>
            {
                Slider slider = (Slider)x;
                return slider.value;
            }).ToArray();
        }
        else if (datatype == DataType.None)
        {
            list = null;
        }
        else if (datatype == DataType.Path)
        {
            list = null;
        }
        else
        {
            throw new NotImplementedException();
        }

        return list;
    }
}
