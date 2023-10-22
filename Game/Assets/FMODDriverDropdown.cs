using FMOD;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class FMODDriverDropdown : MonoBehaviour
{
    Dropdown dropdown;
    // Start is called before the first frame update
    bool start = false;
    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        Reload();
    }
    public void Reload()
    {

        var sys = FMODWrapper.GetSystem();
        sys.getDriver(out int now);
        UnityEngine.Debug.Log(now);
        int num;
        sys.getNumDrivers(out num);
        List<string> drivers = new List<string>();
        for (int i = 0; i < num; i++)
        {
            string driverName = "";
            Guid guid = new Guid();
            
            SPEAKERMODE mode;
            int rate, sm;
            sys.getDriverInfo(i, out driverName, 30, out guid, out rate, out mode, out sm);
            drivers.Add(driverName);
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(drivers);
        dropdown.value = now;
        start = true;
    }

    public void Change(int value)
    {
        if (start)
        {
            FMODWrapper.GetSystem().setDriver(value);
        }
    }
}
