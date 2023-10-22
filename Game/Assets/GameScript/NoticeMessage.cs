using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeMessage : MonoBehaviour
{

    static GameObject singleton;
    // Start is called before the first frame update
    void Start()
    {
        if(singleton == null) singleton = Instantiate(Resources.Load("Prefab/NoticeMessage") as GameObject);
        singleton.SetActive(false);
    }
    public static void Notify(string str)
    {
        singleton.SetActive(true);
        singleton.GetComponentInChildren<Text>().text = str;
        singleton.GetComponentInChildren<Animator>().Play("", -1, 0);
    }
}
