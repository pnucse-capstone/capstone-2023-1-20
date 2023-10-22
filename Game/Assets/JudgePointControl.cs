using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JudgePointControl : MonoBehaviour
{
    [SerializeField]
    Text txt;
    KeyCode JudgeDown, JudgeUp;
    // Start is called before the first frame update
    void Start()
    {

        Game.judgepoint_offset = PlayerPrefs.GetFloat("JudgePoint", 0F);
        JudgeDown = KeySetting.keyJudgeDown;
        JudgeUp = KeySetting.keyJudgeUp;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(JudgeDown))
        {
            Game.judgepoint_offset = Mathf.Clamp(Game.judgepoint_offset - 0.01F, 0F, 0.5F);
            PlayerPrefs.SetFloat("JudgePoint", Game.judgepoint_offset);
            StopAllCoroutines();
            StartCoroutine(Show());
        }
        if (Input.GetKeyDown(JudgeUp))
        {
            Game.judgepoint_offset = Mathf.Clamp(Game.judgepoint_offset + 0.01F, 0F, 0.5F);
            PlayerPrefs.SetFloat("JudgePoint", Game.judgepoint_offset);
            StopAllCoroutines();
            StartCoroutine(Show());
        }

    }
    IEnumerator Show()
    {
        txt.text = "+" + Game.judgepoint_offset.ToString("F2");
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            txt.color = Color.white * (1 - t);
            yield return null;
        }
    }
}
