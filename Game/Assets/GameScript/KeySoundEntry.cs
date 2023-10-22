using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeySoundEntry : MonoBehaviour
{
    public SoundInfo key_info;
    public SoundInfo getInfo()
    {
        return key_info;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Set(SoundInfo info)
    {
        key_info = info;
        GetComponentInChildren<Text>().text = info.name;
    }
}
