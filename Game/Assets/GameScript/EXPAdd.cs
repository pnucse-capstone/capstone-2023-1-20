using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXPAdd : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject resultCanvas;
    void Start()
    {
        StartCoroutine(add());
    }
    IEnumerator add()
    {/*

        yield return new WaitForSeconds(1);
        int before = PlayerPrefs.GetInt("totalExp", 0);
        int after = before + (int)((ScoreBoard.score* (Game.life == 0F ?0.5F:1F) * 200+1));

            PlayerPrefs.SetInt("newAlert", after);


        PlayerPrefs.SetInt("totalExp", after);
        yield return new WaitForSeconds(2);
        resultCanvas.SetActive(true);
    */
        yield return null;
        }

    // Update is called once per frame
    void Update()
    {
    }
}
