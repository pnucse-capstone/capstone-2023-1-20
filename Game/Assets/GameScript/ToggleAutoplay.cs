using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleAutoplay : MonoBehaviour
{
    Toggle toggle;
    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.isOn = Game.autoplay;
        toggle.onValueChanged.AddListener(setAuto);
    }
    public void setAuto(bool value)
    {
        Game.autoplay = value;
        Debug.Log(Game.autoplay);
    }
}
