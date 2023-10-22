using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum FCAP { N, C, FC, GP, AP ,NotPlay};

public class FCAPIcon : MonoBehaviour
{
    [SerializeField]Image icon;
    [SerializeField] Sprite AP;
    [SerializeField] Sprite GP;
    [SerializeField] Sprite FC;
    [SerializeField] Sprite C;
    [SerializeField] Sprite N;
    // Start is called before the first frame update
    public void SetIcon(ScoreEntry score)
    {
        if(score.percent == 0)
        {
            icon.sprite = N;
        }
        else if (score.isAllPerfect()) icon.sprite = AP;
        else if (score.isGoodPlay()) icon.sprite = GP;
        else if (score.isFullCombo()) icon.sprite = FC;
        else if (score.isClear()) icon.sprite = C;
        else icon.sprite = N;
    }
    public void SetIcon()
    {
        icon.sprite = N;
    }
    public void SetIcon(FCAP fcap)
    {
        switch (fcap)
        {
            case FCAP.AP:icon.sprite = AP;  break;
            case FCAP.GP: icon.sprite = GP; break;
            case FCAP.FC: icon.sprite = FC; break;
            case FCAP.C: icon.sprite = C; break;
            case FCAP.N: icon.sprite = N; break;
            default: icon.sprite = N; break;
        }
    }
}
