using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Cancel();
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void Cancel()
    {
        gameObject.SetActive(false);
    }
}
