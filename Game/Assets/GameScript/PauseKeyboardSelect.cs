using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseKeyboardSelect : MonoBehaviour
{

    int select = -1;
    [SerializeField] GameObject[] buttons;
    GameObject selected;
    // Update is called once per frame
    void OnEnable()
    {
        select = -1;
    }
    void Update()
    {
        if (PauseUI.isFade) return;
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (select == -1) select = 0;
            else select = (select + 1) % buttons.Length;
            Select(select);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (select == -1) select = 0;
            else select = (select +buttons.Length - 1) % buttons.Length;
            Select(select);
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            selected.GetComponent<Animator>().SetBool("Select",true);
            selected.GetComponent<Button>().onClick.Invoke();
        }

    }
    void Select(int s)
    {
        foreach (var i in buttons)
        {
            i.GetComponent<Animator>().SetBool("Select", false);
            i.GetComponent<Button>().image.color = new Color(0.3F,0.3F,0.5F,0.7F);
        }
        buttons[s].GetComponent<Animator>().SetBool("Select", true);
        selected = buttons[s];
        GetComponent<FMODSFX>().Play();
    }
}
