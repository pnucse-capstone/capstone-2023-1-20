using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarMove : MonoBehaviour
{
    void Update()
    {
        Vector3 pos = Game.cursor;//Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.x = 0;
        pos.z = gameObject.transform.position.z;
        gameObject.transform.position=pos;
    }
}
