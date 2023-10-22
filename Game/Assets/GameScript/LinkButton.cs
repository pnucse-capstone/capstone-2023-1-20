using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Button))]
public class LinkButton : MonoBehaviour
{
    string url = null;
    private void Start()
    {
        Set(url);
        GetComponent<Button>().onClick.AddListener(() => { Application.OpenURL(url); });
    }
    public void Set(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            this.url = url;
        }

    }
}
