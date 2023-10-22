using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineEffecter : MonoBehaviour
{
    /*
    public GameObject line_prefab;
    List<GameObject> lines_obj;
    List<Animator> lines;
    // Start is called before the first frame update
    void Start()
    {
        lines_obj = new List<GameObject>();
        lines = new List<Animator>();
        for (int i=0;i<6;i++)
        {
            Debug.Log("i" + i);
            lines_obj.Add(Instantiate(line_prefab));
            lines.Add(lines_obj[i].GetComponent<Animator>());
//            lines[i].transform.position = NotePosition.getlinePos(i);
  //          lines[i].transform.localScale = new Vector3(1,NotePosition.line_width/2);
    //        lines[i].Play("", -1, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0;i<6; i++)
        {
            if (InputApply.input[i] == InputState.BEGIN || InputApply.input[i] == InputState.ON)
            {
                lines[i].Play("", -1, 0);
            }
        }
    }*/
}
