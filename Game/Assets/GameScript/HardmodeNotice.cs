using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HardmodeNotice : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }
    bool visible = false;
    // Update is called once per frame
    /*
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.H) && !visible)
        {
            if(Hardmode.isHardmode)
            {
                Hardmode.isHardmode = false;
            }
            else
            {
                GetComponent<Animation>().Play("NoticePopup");
                visible = true;
                Hardmode.isHardmode = true;
            }
        }
        else if (Input.anyKeyDown && visible)
        {
            GetComponent<Animation>().Play("NoticePopdown");
            visible = false;
        }
    }*/
}
public class Hardmode
{
    private static bool hardmode = false;
    /*
    public static bool isHardmode 
    {
        get
        {
            return hardmode;
        }
        set
        {
            if (hardmode != value) 
            {
                hardmode = value;
                NoteTypeInfo.init(hardmode);
                foreach (var i in GameObject.FindGameObjectsWithTag("Refresh"))
                {
                    i.SendMessage("Refresh");
                }
            };
        }
    }
    */
    
}