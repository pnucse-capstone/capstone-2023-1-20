using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PracticeModeLabel : MonoBehaviour
{
    [SerializeField]
    Text key;
    // Start is called before the first frame update
    void Start()
    {
        var minus = KeySetting.GetMappedKey("keySkipMinus").ToString();
        var plus = KeySetting.GetMappedKey("keySkipPlus").ToString();
        key.text = minus + " / " +plus;
        gameObject.SetActive(Game.practice && Game.caller != SceneNames.EDITOR);
        if(Game.caller == SceneNames.EDITOR)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
