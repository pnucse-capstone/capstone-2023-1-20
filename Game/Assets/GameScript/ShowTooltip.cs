using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent(typeof(BoxCollider2D))]
public class ShowTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string id;
    [SerializeField]
    [TextArea]
    public string text;
    static GameObject tooltip; 
    static Text txt;  
    // Start is called before the first frame update
    void Start()
    {
        if(tooltip == null)
        {
            tooltip = Instantiate(Resources.Load("Prefab/Tooltip") as GameObject);
            txt = tooltip.GetComponentInChildren<Text>();
            txt.text = text;
            tooltip.GetComponent<TooltipCanvas>().Show(false);
        }
    }
    public void OnPointerEnter(PointerEventData data)
    {
        tooltip.GetComponent<TooltipCanvas>().SetPosition(data.position);
        tooltip.GetComponent<TooltipCanvas>().Show(true);
        if (id == "")
        {
            txt.text = text;
        }
        else
        {
            txt.text = Translation.GetUIText(id);
        }
    }

    public void OnPointerExit(PointerEventData data)
    {
        txt.text = "";
        tooltip.GetComponent<TooltipCanvas>().SetPosition(data.position);
        tooltip.GetComponent<TooltipCanvas>().Show(false);
    }

    public void Show(bool v)
    {
        tooltip.GetComponent<TooltipCanvas>().Show(v);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GetComponent<Button>() != null)
        {
            if (GetComponent<Button>().interactable)
            {
                tooltip.GetComponent<TooltipCanvas>().Show(false);
            }
        }
    }
}
