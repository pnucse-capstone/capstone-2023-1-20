using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstLaunch : MonoBehaviour
{
    [SerializeField]
    string prefs;
    [SerializeField]
    bool closeWhenPressAnykey;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt(prefs, 0) == 1) Close();
    }
    public void Close()
    {
        gameObject.SetActive(false);
        PlayerPrefs.SetInt(prefs, 1);
    }
    // Update is called once per frame
    void Update()
    {
        if (closeWhenPressAnykey && Input.anyKeyDown)
        {
            Close();
        }

    }
}
