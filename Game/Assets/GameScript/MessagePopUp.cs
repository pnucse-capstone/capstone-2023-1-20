using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagePopUp : MonoBehaviour
{
    [SerializeField] Text text;
    static GameObject instance;
    [SerializeField] Button AcceptButton;
    [SerializeField] Button CloseButton;
    static Action AcceptAction, CloseAction;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = Instantiate(Resources.Load("Prefab/ÆË¾÷") as GameObject);
            instance.SetActive(false);
        }
    }
    public static void Open(string text)
    {
        SubsSelectUI.isLock = true;
        instance.GetComponent<MessagePopUp>().AcceptButton.gameObject.SetActive(false);
        instance.GetComponent<MessagePopUp>().CloseButton.gameObject.SetActive(true);
        AcceptAction = () => { };
        CloseAction = () => { };
        
        instance.SetActive(true);
        instance.GetComponent<MessagePopUp>().text.text = text;
    }
    public static void Open(string text, Action yes)
    {
        instance.GetComponent<MessagePopUp>().AcceptButton.gameObject.SetActive(true);
        instance.GetComponent<MessagePopUp>().CloseButton.gameObject.SetActive(true);
        instance.GetComponent<MessagePopUp>().text.text = text;
        AcceptAction = yes;
        CloseAction = () => { };
        instance.SetActive(true);
    }
    public static void Accept()
    {
        AcceptAction();
        instance.SetActive(false);
        SubsSelectUI.isLock = false;

    }
    public static void Close() // =No
    {
        CloseAction();
        instance.SetActive(false);
        SubsSelectUI.isLock = false;
    }

}
