using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class LineObjectRender : MonoBehaviour
{
    List<GameObject> objs;
    List<LineObject> lines;
    // Start is called before the first frame update
    void Start()
    {
        objs = new List<GameObject>();
        lines = new List<LineObject>();
        GameObject line_prefab = Resources.Load("Prefab/LinePrefab/DefaultLineObject") as GameObject;
        int cnt = Game.lineCount;
        Debug.Log("Input Line Cnt:"+cnt);
        for (int i = 0; i < cnt; i++)
        {
            var new_line = Instantiate(line_prefab, gameObject.transform);
            new_line.GetComponent<LineObject>().SetText(Game.state.inputmap.getInputString(i));
            lines.Add(new_line.GetComponent<LineObject>());
            objs.Add(new_line);
        }
        Refresh();

    }
    List<LinePath.Point> points = new List<LinePath.Point>();
    Dictionary<int, LinePath.Point[]> pool = new Dictionary<int, LinePath.Point[]>();
    void Refresh()
    {
        int cnt = Game.lineCount;
        for (int i = 0; i < cnt; i++)
        {
            objs[i].transform.position = Vector3.zero;//Game.state.LinePosition(i);
            lines[i].SetNoteScale(Game.state.LineScale(i));
            var linepath = Game.mechanim.lineType.GetLinePath(Game.state, i);
            linepath.Slice(0F, Game.state.properties.lineLength[i], ref points);

            LinePath.Point[] arr;
            if (pool.ContainsKey(points.Count))
            {
                arr = pool[points.Count];
            }
            else
            {
                arr = new LinePath.Point[points.Count];
                pool.Add(points.Count,arr);
            }

            for (int k = 0; k < points.Count; k++)
            {
                arr[k] = points[k];
            }
            Profiler.BeginSample("Get Path");
            lines[i].SetPositions(arr);
            Profiler.EndSample();
            lines[i].SetWidth(Game.state.LineScale(i).z);
            lines[i].SetRotation(Game.state.properties.rotation[i]);
            if (!Game.reduced)
            {
                lines[i].SetBaseColor(Game.state.properties.lineColors[i]);
                lines[i].SetCenterColor(Game.state.properties.lineColors[i] + Color.white * 0.1F);
            }
            else
            {
                lines[i].SetBaseColor(new Color(0.2F,0.2F,0.2F));
                lines[i].SetCenterColor(new Color(0.2F, 0.2F, 0.2F) + Color.white * 0.1F);

            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        Refresh();
        int cnt = Game.lineCount;
        for (int i = 0; i < cnt; i++)
        {
            if (Game.state.input.isOn(i))
            {
                lines[i].Effect();

            }
        }
    }
}
