using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShowScoreboard : MonoBehaviour
{
    public Text score;
    public Color Pcolor;
    public Color Gcolor;
    public Color Ocolor;
    public Color Ncolor;
    public Color Fcolor;
    [SerializeField]
    Text Combo;
    // Start is called before the first frame update
    // Update is called once per frame
    void Start()
    {
        if (Game.practice && Game.caller != SceneNames.EDITOR)
        {
            gameObject.SetActive(false);
            score.text = "";
        }
        StartCoroutine(Refresh());
    }
    IEnumerator Refresh()
    {
        float prev = 0 ;
        while (true)
        {
            float now = ScoreBoard.percent;
            if (prev != now)
            {

                score.text = string.Format("{0:F2}%", ScoreBoard.percent * 100);
                if (ScoreBoard.isAllPerfect())
                {
                    score.color = Pcolor;
                }
                else if (ScoreBoard.isGoodPlay())
                {
                    score.color = Gcolor;
                }
                else if (ScoreBoard.isFullCombo())
                {
                    score.color = Ocolor;
                }
                else 
                {
                    score.color = Ncolor;
                }
                Combo.color = Color.white;
            }
            prev = ScoreBoard.percent;
            yield return null;
        }
    }
}
