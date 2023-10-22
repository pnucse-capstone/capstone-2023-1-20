using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathPointInput : MonoBehaviour
{
    [SerializeField] InputField t;
    [SerializeField] InputField x;
    [SerializeField] InputField y;
    LinePath.Point prev;
    public void Set(LinePath.Point p)
    {
        t.text = p.t.ToString();
        x.text = p.x.ToString();
        y.text = p.y.ToString();
        prev = p;
    }
    public LinePath.Point Get()
    {
        float t=prev.t, x=prev.x, y=prev.y;
        float.TryParse(this.t.text, out t);
        float.TryParse(this.x.text, out x);
        float.TryParse(this.y.text, out y);
        return new LinePath.Point(t,x,y);
    }
}
