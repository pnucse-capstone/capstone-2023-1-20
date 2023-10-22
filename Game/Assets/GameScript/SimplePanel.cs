using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimplePanel : MonoBehaviour
{
    public Text loading_comment;
    // Start is called before the first frame update
    public void SetText(string text)
    {
        loading_comment.text = text;
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
