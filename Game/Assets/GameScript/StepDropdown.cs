using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class StepDropdown : MonoBehaviour
{
    Dropdown drop;
    // Start is called before the first frame update
    static int recent = 1;

    int[] set = new int[] { 1,2,3,4,6,8,12,16,24,32};
    void Start()
    {
        drop = GetComponent<Dropdown>();
        drop.value = recent;
        drop.options.Clear();
        drop.AddOptions(set.Select(x=>$"1/{x}").ToList());
    }
    public void Lock()
    {
        NoteEditor.PopupLock = true;
    }
    public void Release()
    {
        NoteEditor.PopupLock = false;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
        }
        for (KeyCode i = KeyCode.Keypad0;i<=KeyCode.Keypad9;i++)
        if (Input.GetKeyDown(i))
        {
            SetStep((int)i-(int)KeyCode.Keypad0);
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Equals))
        {
            SetStep(drop.value + 1);
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus))
        {
            SetStep(drop.value - 1);
        }
    }
    public void SetStep(int code)
    {
        recent = code;
        NoteEditor.divide = set[code];
        drop.value = code;
    }

}
