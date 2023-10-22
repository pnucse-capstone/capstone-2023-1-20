using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JudgeUIControl : MonoBehaviour
{

    float offset = 0F;
    [SerializeField]
    Text txt;
    [SerializeField]
    Transform trans;
    // Start is called before the first frame update
    KeyCode UIDown, UIUp;
    void Start()
    {
        UIDown = KeySetting.keyJudgeUIDown;
        UIUp = KeySetting.keyJudgeUIUp;
        offset = PlayerPrefs.GetFloat("JudgeUI", 0.1F);
        SetPosition();

    }

    public void SetPosition()
    {
        trans.localPosition = Vector3.up * 100 * offset;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(UIDown))
        {
            offset = Mathf.Clamp(offset - 0.05F, -1F, 1F);
            PlayerPrefs.SetFloat("JudgeUI", offset);
            StopAllCoroutines();
            StartCoroutine(Show());
            SetPosition();
        }
        if (Input.GetKeyDown(UIUp))
        {
            offset = Mathf.Clamp(offset + 0.05F, -1F, 1F);
            PlayerPrefs.SetFloat("JudgeUI", offset);
            StopAllCoroutines();
            StartCoroutine(Show());
            SetPosition();
        }

    }
    IEnumerator Show()
    {
        txt.text = offset.ToString("F2");
        if (offset > 0) txt.text = "+"+txt.text;
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            txt.color = Color.white * (1 - t);
            yield return null;
        }
    }
}
