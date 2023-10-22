using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class showMusicTitle : MonoBehaviour
{
    [SerializeField] Text txt;
    // Start is called before the first frame update
    void Start()
    {
        txt.text = Game.table.composer +" - "+ Game.table.title;
    }
}
