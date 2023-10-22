using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetaDataDifficultyDropdown : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }
    public void Select(int value)
    {
        Game.table.difficulty = GetComponent<Dropdown>().options[value].text;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void Refresh()
    {
        var index = GetComponent<Dropdown>().options.FindIndex((x)=>Game.table.difficulty==x.text);
        GetComponent<Dropdown>().value = index;
    }
}
