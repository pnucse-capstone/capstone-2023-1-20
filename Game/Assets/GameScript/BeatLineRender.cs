using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatLineRender : MonoBehaviour
{
    GameObject[] arr = new GameObject[32];
    [SerializeField]
    GameObject line_prefab;
    void Start()
    {
        //아몰랑 구현안해
        /*표시용 오브젝트 초기설정*/
        Vector3 origin = Camera.main.transform.position;
        origin.z = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = Instantiate(line_prefab, origin, Quaternion.identity);
        }
    }
    // Update is called once per frame
    void Update()
    {
        RenderLines();
    }

    private void RenderLines()
    {
        for(int i =0;i < arr.Length; i++)
        {
            arr[i].transform.position = Vector3.up * i;
        }
    }
}
