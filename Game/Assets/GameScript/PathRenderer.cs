using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRenderer : MonoBehaviour
{
    /*
    // Start is called before the first frame update
    public Color color;
    LineRenderer line; 
    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
    }
    void DrawPath()
    {        
        var L = NotePosition.path.getNodesNear(Game.table.searchNote(Game.table.searchEvent(Game.time).time).data.time);
        line.positionCount = System.Math.Max(L.Length-1,0);
        line.SetPositions(L);
    }
    public void showLine(float alpha)
    {
        if (alpha <= 0)
        {
            line.enabled = false;
            return;
        }
        DrawPath();
        line.enabled = true;
        Color c = color;
        c.a = alpha;
        line.startColor = c;
        line.endColor = c;
    }*/
}
