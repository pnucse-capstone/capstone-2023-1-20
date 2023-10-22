using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LanguageDropdown : MonoBehaviour
{
    Dropdown dropdown;
    // Start is called before the first frame update
    //기본적으로는 그대로, 대신 몇개는 매핑해주는방식으로..
    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        dropdown.AddOptions(Translation.GetLanguages().FindAll(x=>x!="emotion"));
        dropdown.value = dropdown.options.FindIndex(x => x.text == Translation.GetCurrentLanguage());
        dropdown.options.RemoveAll(x => string.IsNullOrEmpty(x.text));
        dropdown.options.ForEach(x => {
            Debug.Log(x.text);
            if(x.text == "koreana")x.text="korean";
            if (x.text == "tchinese") x.text = "chinese(traditional)";
            if (x.text == "schinese") x.text = "chinese(simplified)";
        });
    }

    public void Change(int value)
    {
        string lang = dropdown.options[value].text;
        if (lang == "korean") lang = "koreana";
        if (lang == "chinese(traditional)") lang = "tchinese";
        if (lang == "chinese(simplified)") lang = "schinese";
        int index = Translation.GetLanguages().FindIndex(x => x == lang);
        Translation.ChangeLanguage(Translation.GetLanguages()[index]);
    }
}
