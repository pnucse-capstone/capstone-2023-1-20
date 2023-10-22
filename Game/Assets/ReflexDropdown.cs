using Newtonsoft.Json;
using NVIDIA;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
public class ReflexDropdown : MonoBehaviour
{
    [SerializeField]
    Dropdown dropdown;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(coLateStart());   
    }
    IEnumerator coLateStart()
    {
        yield return null;
        LateStart();
    }
    void LateStart()
    {

        Debug.Log("reflex option:" + Reflex.IsReflexLowLatencySupported());
        if (Reflex.IsReflexLowLatencySupported() != Reflex.NvReflex_Status.NvReflex_OK)
        {
            dropdown.options = dropdown.options.SkipLast(2).ToList();
            PlayerPrefs.SetInt("Reflex", 0);
        }
    }

}
