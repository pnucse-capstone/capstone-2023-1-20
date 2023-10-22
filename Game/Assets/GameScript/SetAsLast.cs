using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAsLast : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.SetAsLastSibling();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.SetAsLastSibling();
    }
}
