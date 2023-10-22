using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetShader : MonoBehaviour
{
    public float t= 0 ;
    Material mat;
    // Start is called before the first frame update
    void Start()
    {

        mat = GetComponent<Image>().material;
    }

    // Update is called once per frame
    void Update()
    {
        mat.SetFloat("_t", t);
    }
}
