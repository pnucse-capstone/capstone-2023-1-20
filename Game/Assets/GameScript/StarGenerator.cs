using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarGenerator : MonoBehaviour
{
    public int cnt=20;
    public GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }
    void Generate()
    {
        for(int i=0;i<cnt; i++)
        {
            var temp=Instantiate(prefab,gameObject.transform);
            temp.GetComponent<randomStart>().reposit();
        }
    }
}
