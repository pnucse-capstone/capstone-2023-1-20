using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EXPView : MonoBehaviour
{
    // Start is called before the first frame update
    public Text level_txt;
    public Text bar_txt;
    public GameObject animation;
    int level;
    int exp;
    void Start()
    {
        int total = PlayerPrefs.GetInt("totalExp", 0);
        level = total / 300+1;
        exp = total % 300;
        level_txt.text = "LV" + level;
        setExpGauge(exp, 300);
        StartCoroutine(interpolation());
    }
    void setExpGauge(int exp, int exp_max)
    {
        int bar_amount = 25 * exp / exp_max;
        bar_txt.text = "exp. ";
        for (int i = 0; i < bar_amount; i++) bar_txt.text += "I";
    }
    IEnumerator interpolation()
    {
        while (true)
        {
            int total = PlayerPrefs.GetInt("totalExp", 0);
            int target_level = total / 300 + 1;
            int target_exp = total % 300;
            if (exp < target_exp || level < target_level)
            {
                exp = (exp + 5);
                if (exp >= 300)
                {
                    animation.SetActive(true);
                    GetComponent<AudioSource>().Play();
                    level++;
                    level_txt.text = "LV" + level;
                    exp = 0;
                }
            };
            setExpGauge(exp, 300);
            yield return new WaitForSeconds(0.01F);
        };
    }
    // Update is called once per frame
    void Update()
    {

//        if (Input.GetMouseButtonDown(1)) PlayerPrefs.SetInt("totalExp", PlayerPrefs.GetInt("totalExp", 0)+100);
    }
}
