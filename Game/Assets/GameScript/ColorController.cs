using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorController : MonoBehaviour
{
    [SerializeField]
    ColorPickerAdapter color_picker;    // Start is called before the first frame update
    public void OnClick()
    {
        var list = GetComponentInParent<GameEventEditEntry>().GetFields();
        color_picker.OpenColorPicker((c)=> {
            foreach (var i in list) i.image.color = c;
        }, Color.white);
    }
}
