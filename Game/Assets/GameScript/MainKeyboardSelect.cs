using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainKeyboardSelect : MonoBehaviour
{
    int select = -1;
    GameObject selected;
    [SerializeField]GameObject[] buttons;
    // Update is called once per frame
    void Update()
    {
        if (NoteEditor.PopupLock) return;
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (select == - 1)select = 0;
            else select = (select + 1) % 3;
            Select(select);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (select == -1) select = 0;
            else select = (select+3 - 1) % 3;
            Select(select);
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            selected.GetComponent<RolloverButton>().SetDown();
            selected.GetComponent<Button>().onClick.Invoke();
            Deselect();
        }

    }
    void Select(int s)
    {
        buttons[s].GetComponent<RolloverButton>().ResetButton();
        for (int i= 0;i<buttons.Length;i++)
        {
            if (i == s)
            {
                buttons[s].GetComponent<RolloverButton>().SetOn();
                selected = buttons[s];
            }
            else
            {
                buttons[i].GetComponent<RolloverButton>().SetOff();
            }
        }
    }
    public void Select(GameObject button)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == button)
            {
                Select(i);                
            }
        }
    }
    public void Deselect()
    {
        foreach(var i in buttons)
        {
            i.GetComponent<RolloverButton>().ResetButton();
            selected = null;
            select = -1;
        }
    }
}
