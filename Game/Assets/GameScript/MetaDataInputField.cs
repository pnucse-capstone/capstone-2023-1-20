using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
public class MetaDataInputField : MonoBehaviour
{
    enum DataType {String, Integer, Float }
    [HideInInspector]
    public string type;
    // Start is called before the first frame update
    void Start()
    {
        var input = GetComponent<InputField>();
        input.onEndEdit.AddListener((x)=>EndEditCallback(x));
        Refresh();
    }
    public void EndEditCallback(string x)
    {
        DirtyChecker.MakeDirty();
        var field = typeof(Table).GetField(type);
        var prop = typeof(Table).GetProperty(type);
        if (field != null)
        {
            if(field.FieldType == typeof(float))
            {
                float value = float.Parse(x, CultureInfo.InvariantCulture);
                field.SetValue(Game.table, value);
            }
            else if (field.FieldType == typeof(int))
            {
                int value = int.Parse(x);
                field.SetValue(Game.table, value);
            }
            else if (field.FieldType == typeof(string))
            {
                field.SetValue(Game.table, x);
            }
        }
        if (prop != null)
        {
            if (prop.PropertyType == typeof(float))
            {
                float value = float.Parse(x, CultureInfo.InvariantCulture);
                prop.SetValue(Game.table, value);
            }
            else if (prop.PropertyType == typeof(int))
            {
                int value = int.Parse(x);
                prop.SetValue(Game.table, value);
            }
            else if (prop.PropertyType == typeof(string))
            {
                prop.SetValue(Game.table, x);
            }
        }
        Game.table.InvalidTableCorrection();
    }
    void Refresh()
    {
        var input = GetComponent<InputField>();
        FieldInfo field = typeof(Table).GetField(type);
        PropertyInfo prop = typeof(Table).GetProperty(type);
        if (field != null && Game.table != null)
            input.text = field.GetValue(Game.table).ToString(); 
        if (prop != null && Game.table != null) 
            input.text = prop.GetValue(Game.table).ToString();
    }
}
