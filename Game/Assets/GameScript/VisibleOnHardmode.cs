using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VisibleOnHardmode : MonoBehaviour
{
    [SerializeField]
    List<Behaviour> comps;
    // Start is called before the first frame update
    void Start()
    {
        Refresh();
    }
    void Refresh()
    {
        foreach(var i in comps)
        {
            i.enabled = false;
        }
    }
    // Update is called once per frame
}
