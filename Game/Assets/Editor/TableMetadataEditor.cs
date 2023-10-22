using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(MetaDataInputField))]
public class TableMetadataEditor : Editor 
{
    public int select;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var obj = (MetaDataInputField)target;
        var fields = typeof(Table).GetFields();
        var props = typeof(Table).GetProperties();
        var names = fields.Select((x) => x.Name).Concat(props.Select((x)=>x.Name)).ToList();
        var select = names.FindIndex((x)=>x == obj.type);
        select = EditorGUILayout.Popup(select, names.ToArray());
        
        obj.type = names[select];
    }

}
