using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxMode : MonoBehaviour
{
    List<Mode> modes;
    // Start is called before the first frame update
    void Start()
    {
        modes = new List<Mode>();
        modes.Add(new Vertical());
        modes.Add(new oneSided());
        modes.Add(new Sandbox());
        foreach (Mode i in modes)
        {
            i.Start();
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Mode i in modes)
        {
            i.Update();
        }
    }
}

class Sandbox : Mode
{
    public override void Start()
    {
    }
    public override void Update()
    {
    }
}
