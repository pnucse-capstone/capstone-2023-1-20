using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.UI;
public class SteamStatView : MonoBehaviour
{
    Text txt;
    string[] stat_cnt = {"cnt_Child", "cnt_Polygons", "cnt_Polygons", "cnt_Lovefall", "cnt_Geohope" };
    // Start is called before the first frame update
    void Start()
    {
        txt = GetComponent<Text>();

        txt.text = format();
    }
    string format()
    {

        string value = "";
        foreach(var i in stat_cnt)
        {
            Debug.Log(SteamUserStats.GetStatInt(i));
            value += i+":"+SteamUserStats.GetStatInt(i)+'\n';
        }
        return value;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
