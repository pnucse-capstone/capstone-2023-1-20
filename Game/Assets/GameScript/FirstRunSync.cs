using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstRunSync : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("flag_sync", 0) == 0)
        {
            PlayerPrefs.SetInt("flag_sync", 1);
            SceneManager.LoadScene(SceneNames.SYNC);
        }
    }

}
