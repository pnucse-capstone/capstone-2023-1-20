using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequireSteam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Selectable>().interactable = Steamworks.SteamClient.IsValid;
    }
}
