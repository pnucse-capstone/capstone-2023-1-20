using NVIDIA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class CodeDropdown : MonoBehaviour
{
    [SerializeField]
    string code;
    [SerializeField]
    int init= 0;
    [SerializeField]
    List<string> list = new List<string>();
    Dropdown dropdown;
    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        dropdown.AddOptions(list);
        var i =PlayerPrefs.GetInt(code, init);
        dropdown.value = i;
        dropdown.onValueChanged.AddListener(OnChange);
    }
    public void OnChange(int value)
    {
        Debug.Log(value);
        PlayerPrefs.SetInt(code, value);
    }
}
