using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class FloatingMotion : MonoBehaviour
{
    Vector3 o_pos;
    public float size = 0.1F;
    float t = 0F;
    // Start is called before the first frame update
    void Start()
    {
        o_pos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        t =Time.time;
        gameObject.transform.position= new Vector3(o_pos.x, o_pos.y + size* (float)Math.Sin(t*3), -1);
    }
}
