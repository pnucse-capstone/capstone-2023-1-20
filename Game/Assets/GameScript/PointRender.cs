using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointRender : MonoBehaviour
{
    [SerializeField]
    GameObject point;
    GameObject[] points;
    // Start is called before the first frame update
    void Start()
    {
        points = new GameObject[16];
        for(int i=0;i<16; i++)
        {
            points[i] = Instantiate(point) as GameObject;
            points[i].SetActive(false);
        }
    }
    float bpm = 140;
    // Update is called once per frame
    void Update()
    {
        float spb = 60 / bpm;
        for(int i= 0; i<16; i++)
        {
            float time = Game.time-Game.time%spb;
            float time_point = time + i* spb;
//            Vector3 pos = NotePosition.defaultFunction(time_point);
 //s           points[i].transform.position = pos;
        }

    }
}
