using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MouseOverPanel : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField]
    GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnPointerEnter(PointerEventData data)
    {
        panel.SetActive(true);
    }
    public void OnPointerExit(PointerEventData data)
    {
        panel.SetActive(false);
    }
    public void OpenHelpURL()
    {
        Application.OpenURL("https://docs.google.com/document/d/1mKwGY9yS25fkZpbOuxYPE6Qoj4ECTF9z2CrlFhyZKc0/");
    }
}
