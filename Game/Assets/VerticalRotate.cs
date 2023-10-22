using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (NoteEditor.vertical)
        {
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }

    }
}
