using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedMultiplierControl : MonoBehaviour
{
    [SerializeField]
    Text txt;
    KeyCode speedDown,speedUp;
    // Start is called before the first frame update
    void Start()
    {
        speedDown = KeySetting.keySpeeddown;
        speedUp = KeySetting.keySpeedup;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(speedDown))
        {
            Game.speed_multiplier = Mathf.Max(1F, Game.speed_multiplier - 0.1F);
            PlayerPrefs.SetFloat("NoteSpeed", Game.speed_multiplier);
            StopAllCoroutines();
            StartCoroutine(Show());
        }
        if (Input.GetKeyDown(speedUp))
        {
            Game.speed_multiplier = Mathf.Max(1F, Game.speed_multiplier + 0.1F);
            PlayerPrefs.SetFloat("NoteSpeed", Game.speed_multiplier);
            StopAllCoroutines();
            StartCoroutine(Show());
        }

    }
    IEnumerator Show()
    {
        txt.text = "x" + Game.speed_multiplier.ToString("F1");
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            txt.color = Color.white*(1-t);
            yield return null;
        }
    }
}
