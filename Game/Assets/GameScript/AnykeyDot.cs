using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnykeyDot : MonoBehaviour
{
    [SerializeField]
    GameObject dot;
    void Start()
    {
        dot.SetActive(false);
    }

    // Update is called once per frame
    public void Set(bool value)
    {
        if (value)
        {
            GetComponent<Animator>().Play("", -1, 0);
        }
        dot.SetActive(value);
    }
}
