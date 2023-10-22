using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipCanvas : MonoBehaviour
{
    [SerializeField]
    RectTransform balloon;

    [SerializeField]
    Graphic[] images;
    public void SetPosition(Vector3 pos)
    {
        var canvas = gameObject.GetComponent<CanvasScaler>();
        var w = canvas.referenceResolution.x;
        var x = Mathf.Clamp(pos.x, 0, Screen.width- balloon.rect.width * Screen.width / w * 1.2F);
        balloon.position = new Vector2(x, pos.y);
    }
    public void Show(bool value)
    {
        foreach(var i in images)
        {
            i.enabled = value;
        }
    }

    void Update()
    {
        SetPosition(Input.mousePosition);
    }

}
