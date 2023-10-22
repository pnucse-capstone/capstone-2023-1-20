using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class centerAlign : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector3 temp = gameObject.transform.position;
        temp.x = 0;
        gameObject.transform.position = temp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
