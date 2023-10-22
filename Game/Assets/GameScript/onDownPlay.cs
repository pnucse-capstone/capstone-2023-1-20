using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onDownPlay : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            gameObject.GetComponent<Animator>().Play("", -1, 0f);
    }
}
