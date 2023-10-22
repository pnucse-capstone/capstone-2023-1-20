using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SketchModeLabel : MonoBehaviour
{

    [SerializeField]
    GameObject obj;
    // Update is called once per frame
    void Update()
    {
        obj.SetActive(NoteEditor.sketchmode);
    }
}
