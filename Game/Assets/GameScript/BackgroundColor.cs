using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundColor : MonoBehaviour
{
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }
    [SerializeField]
    Color[] Colors;
    // Update is called once per frame
    void Update()
    {

        /*
        if (Hardmode.isHardmode)
        {
            mat.SetColor("_Color", Colors[2]);
        }
        else
        {
            mat.SetColor("_Color", Colors[0]);
        }
        */
    }
}
