using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonVisibility : MonoBehaviour
{
    [SerializeField]
    List<Sprite> icons;
    public enum Type{Private, FriendsOnly, Public };
    public void Set(Type type)
    {
        GetComponent<Image>().sprite = icons[(int)type];
    }
}
