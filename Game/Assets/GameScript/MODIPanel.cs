using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MODIPanel : MonoBehaviour
{
    [SerializeField]
    GameObject Panel;
    [SerializeField]
    Animator ani;
    public void Open()
    {
        StopAllCoroutines();
        Panel.SetActive(true);
        ani.Play("Modi", -1, 0);
    }
    public void Close()
    {
        StartCoroutine(coClose());
    }
    IEnumerator coClose()
    {
        ani.Play("ModiClose", -1, 0);
        yield return new WaitForSeconds(0.5F);
        Panel.SetActive(false);
    }
}
