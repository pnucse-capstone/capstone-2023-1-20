using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditUI : MonoBehaviour
{
    [SerializeField]
    Animator ani;   
    // Start is called before the first frame update
    public void Close()
    {
        ani.SetBool("Setting", false);
        gameObject.SetActive(false);
    }
    public void Open()
    {
        ani.SetBool("Setting", true);

        gameObject.SetActive(true);
    }
}
