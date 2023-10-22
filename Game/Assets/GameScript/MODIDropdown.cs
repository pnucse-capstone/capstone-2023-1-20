using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MODIDropdown : MonoBehaviour
{
    // Start is called before the first frame update
    List<MODI> modis;
    Dropdown dropdown;
    void Start()
    {
        modis = GetModiList();
        dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        dropdown.AddOptions(modis.Select((x)=>x.name).ToList());
        dropdown.value = modis.FindIndex((x)=>x.code == Game.modi.code);
        Debug.Log((Game.modi.code, dropdown.value));
    }
    public void OnDropdownChanged(int value)
    {
        Game.modi = modis[value];
    }
    public List<MODI> GetModiList()
    {
        List<MODI> modis = new List<MODI>()
        {
            new MODINone(),
            new MODIRandom(),
//            new MODISuperRandom(),
            new MODIMirror(),
            new MODIHardJudge(),
            new MODINoGimmick(),
            new MODIOneButton(),
//            new MODIBackmask(),
            new MODI4lines(),
            new MODI4keys(),
            new MODI6keys(),
            new MODI4tunes(),
        };
        return modis;
    }
    public MODI GetMode(MODI.Mode mode)
    {
        return GetModiList().Find((x) =>x.code == mode);
    }
}
