using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeButton : MonoBehaviour
{
    [SerializeField]
    NoteEditor.Mode mode;
    
    // Start is called before the first frame update
    void Start()
    {
        SetHighlight(false);
        GetComponent<Button>().onClick.AddListener(() =>NoteEditor.SwapMode(mode));
    }

    void SetHighlight(bool value)
    {
        if (value)
        {
            var selected_color = new Color(0.5F, 0.5F, 0.5F, 0.5F);
            GetComponent<Image>().color = selected_color;
        }
        else
        {
            var idle_color = new Color(0, 0, 0, 0.5F);
            GetComponent<Image>().color = idle_color;
        }
    }
    // Update is called once per frame
    void Update()
    {
        SetHighlight(NoteEditor.mode == mode);
    }
}
