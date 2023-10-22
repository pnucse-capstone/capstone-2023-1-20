using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RandomTimeStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Animator>().Play("", -1, Random.Range(0F, 1F));
    }

    // Update is called once per frame
    void Update()
    {
    }
}
