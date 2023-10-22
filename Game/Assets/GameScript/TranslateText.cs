using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TranslateText : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public string id;
    [TextArea]
    public string english_text;
    [TextArea]
    public string korean_text;
    void Start()
    {
        Apply();
    }

    private void Apply()
    {
        var text = gameObject.GetComponent<Text>();
        var lang = Translation.GetCurrentLanguage();
        if (id == "")
        {
            if (lang == "koreana") text.text = korean_text;
            else text.text = english_text;
        }
        else
        {
            text.text = Translation.GetUIText(id);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Apply();
    }
}
