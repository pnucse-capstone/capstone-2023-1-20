using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleFieldInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Refresh()
    {
        GetComponent<Toggle>().isOn = Game.table.use_script;
    }
}
