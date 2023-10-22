using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboEffecter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(PlayerPrefs.GetInt("WaveEffect",1) == 1);
        rip = gameObject.GetComponent<makeRipple>();
    }
    int prev=0;
    makeRipple rip;
    // Update is called once per frame
    void Update()
    {
        if(ScoreBoard.combo%20==0 && ScoreBoard.combo!=0 && prev!=ScoreBoard.combo)
        {
            rip.playRipple();
        }
        prev = ScoreBoard.combo;
    }
}
