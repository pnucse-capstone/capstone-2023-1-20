using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioApplyVolumePreference : MonoBehaviour
{
    // Start is called before the first frame update
    public void Apply()
    {
        gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("volume", 0.4F);
    }
    void Start()
    {
        Apply();   
    }
    void Update()
    {
        Apply();
    }
}
