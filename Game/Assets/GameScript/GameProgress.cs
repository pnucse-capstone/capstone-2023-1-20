using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using Steamworks.Data;

public class GameProgress : MonoBehaviour
{
    [SerializeField]
    Text rate;
    [SerializeField]
    UnityEngine.Color color;
    // Start is called before the first frame update
    void Start()
    {
        Refresh();
    }

    // Update is called once per frame
    void Refresh()
    {
        /*
        var a = Camera.main.gameObject.GetComponent<SelectCameraMove>().progressRate(Hardmode.isHardmode);
        rate.text = string.Format("{0:F2}", a) + "%";
        if (Hardmode.isHardmode)
        { 
            rate.color = color; 
        }
        else
        {
            rate.color = UnityEngine.Color.white;
        }
        */
    }
}
