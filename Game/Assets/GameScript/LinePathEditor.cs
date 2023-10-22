using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

[Serializable]
public struct LinePath
{
    [Serializable]
    public struct Point
    {
        public float t;
        public float x;
        public float y;
        public Point(float t, float x, float y)
        {
            this.t = t;
            this.x = x;
            this.y = y;
        }
        public void Log()
        {
            Debug.Log((t, x, y));
        }

        public static Point Lerp(Point a, Point b, float t)
        {
            var x = Mathf.Lerp(a.x, b.x, t);
            var y = Mathf.Lerp(a.y, b.y, t);
            return new Point(Mathf.Lerp(a.t,b.t,t),x,y);
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y,-1);
        }
    }
    public static LinePath Default => new LinePath(new Point[] { new Point(0,0,0) , new Point(1,0,10) });
    public Point[] points;
    public LinePath(Point[] points)
    {
        this.points = (Point[])points.Clone();
        Array.Sort(points, cmp);
    }
    private int cmp(Point x, Point y)
    {
        return Math.Sign(x.t - y.t);
    }

    public Point[] GetPoints()
    {
        return points;
    }

    Point search(float t, bool get_next = false) // 비교함수는 오름차순일 때 x<y을 반환
    {
        int left = 0;
        int right = points.Length - 1;
        while (left != right)
        {
            int mid = (left + right + 1) / 2;
            if (t < points[mid].t) right = mid - 1;
            else left = mid;
        }
        if (!get_next)
        {
            return points[Math.Max(left, 0)];
        }
        else return points[Math.Min(left + 1, points.Length - 1)];
    }
    static List<Point> list = new List<Point>();
    public LinePath Slice(float start, float end)
    {
        Profiler.BeginSample("Slice");
        list.Clear();
        LinePath path = new LinePath();
        list.Add(Get(start));
        list.Add(Get(end));
        foreach (var i in points)
        {
            if(i.t>start && i.t<end) list.Add(i);
        }
        list.Sort(cmp);
        path.points = list.ToArray();
        Profiler.EndSample();
        return path;
    }
    public void Slice(float start, float end,ref List<Point> list)
    {
        list.Clear();
        list.Add(Get(start));
        foreach (var i in points)
        {
            if (i.t > start && i.t < end) list.Add(i);
        }
        list.Add(Get(end));
    }
    public Point Get(float distance)
    {
        Point a = search(distance,false);
        Point b = search(distance, true);
        //float t = (b.t - a.t) == 0? 0:(distance-a.t)/(b.t-a.t);
        float t = Mathf.InverseLerp(a.t, b.t, distance);

        return Point.Lerp(a,b,t);
    }
    public float maxTime
    {
        get => points[points.Length - 1].t;
    }
    public float minTime
    {
        get => points[0].t;
    }
    public static LinePath Lerp(LinePath a, LinePath b, float t)
    {
        var r = new LinePath();
        var ts = new SortedSet<float>();
        foreach (var p in a.points)
        {
            ts.Add(p.t);
        }
        foreach (var p in b.points)
        {
            ts.Add(p.t);
        }
        r.points = new Point[ts.Count];
        int index = 0;
        foreach (var t_now in ts)
        {
            var anow = a.Get(t_now);
            var bnow = b.Get(t_now);
            r.points[index] = Point.Lerp(anow, bnow, t);
            index++;
        }
        return r;
    }
}
public class LinePathEditor : MonoBehaviour
{
    [SerializeField]
    Transform content;
    [SerializeField]
    GameObject entry;

    List<GameObject> list = new List<GameObject>();
    public void Add()
    {
        var temp = Instantiate(entry,content);
        list.Add(temp);
    }
    public void Remove()
    {
        if (list.Count>2)
        {
            var temp = list[list.Count - 1];
            list.Remove(temp);
            DestroyImmediate(temp);
        }
    }
    public void Resize(int count)
    {
        if(count>=2)
        while(list.Count != count)
        {
            if (list.Count > count) Remove();
            if (list.Count < count) Add();
        }
    }
    int nowline = -1;
    EventData nowevent;
    GameEventT etype;
    public void Open(EventData e,int i,GameEventT etype)
    {
        gameObject.SetActive(true);
        transform.position = Input.mousePosition;
        nowevent = e;
        nowline = i;
        this.etype = etype;
        var paths = (LinePath[])etype.Get(e);
        Resize(paths[nowline].points.Length);
        var points = paths[nowline].points;
        for(int p = 0; p < points.Length; p++)
        {
            list[p].GetComponent<PathPointInput>().Set(points[p]);
        }
    }
    public void Close()
    {
        if (nowevent == null) return;
        var points = new List<LinePath.Point>();
        foreach(var entry in list)
        {
            var point = entry.GetComponent<PathPointInput>().Get();
            points.Add(point);
        }
        var path = new LinePath(points.ToArray());
//        nowevent.linePath[nowline] = path;
        etype.Get(nowevent).SetValue(path, nowline);
        gameObject.SetActive(false);
    }
    void Update()
    {
        
    }

}
