using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSlider : MonoBehaviour
{
    public Text txt;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetText(float value)
    {
        if(value == 0)
        {
            txt.text = "All";
        }
        else txt.text = "Lv." + value;
    }
}
