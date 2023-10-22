using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onRightPlay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
            gameObject.GetComponent<Animator>().Play("", -1, 0f);
    }
}
