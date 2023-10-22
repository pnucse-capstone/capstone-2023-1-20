using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Refresher : MonoBehaviour
{
    [SerializeField]
    UnityEvent handler;
    [SerializeField]
    UnityEvent hardHandler;
    // Start is called before the first frame update
    public void Refresh()
    {
        NoteEditor.snaps.CalcGrid();
        handler.Invoke();
    }   
    public void HardRefresh()
    {
        hardHandler.Invoke();
        Refresh();
    }
}
