using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

public class ScoreEffect : MonoBehaviour
{
    [SerializeField]
    List<ScoreEffectEntry> fx;
    static Dictionary<JudgeType,ScoreEffectEntry> _fx;
    public GameObject comboObj;
    public static Text combo;
    public static Animation comboFade;


    [SerializeField]
    GameObject pgmObj;
    public static Text pgmText;
    public static Animation pgmFade;
    public const string yellow = "#BBC089";
    public const string blue = "#88BFB6";
    public const string red = "#C08990";
    public const string green = "#7AB14F";


    [SerializeField]
    GameObject elObj;
    [SerializeField] 
    Color early;
    [SerializeField]
    Color late;

    static ScoreEffect instance;
    static bool useEL = false;
    // Start is called before the first frame update
    static float elrange = 0.04F;
    void Start()
    {
        elrange = PlayerPrefs.GetFloat("elrange", 0.04F);
        useEL = PlayerPrefs.GetInt("EL", 1) == 1;
        instance = this;

        combo = comboObj.GetComponent<Text>();
        comboFade = comboObj.GetComponent<Animation>();

        pgmText = pgmObj.GetComponent<Text>();
        pgmFade = pgmObj.GetComponent<Animation>();

        _fx = new Dictionary<JudgeType, ScoreEffectEntry>();
        _fx.Add(JudgeType.perfect,fx[0]);
        _fx.Add(JudgeType.good, fx[1]);
        _fx.Add(JudgeType.ok, fx[2]);
        _fx.Add(JudgeType.miss, fx[3]);

        ShowPgm();
        StartCoroutine(Refresh());
    }
    IEnumerator Refresh()
    {
        ScoreEntry prev= new ScoreEntry();
        while (true)
        {
            var score = ScoreBoard.GetScoreEntry();
            if (prev.perfect != score.perfect || prev.good != score.good || prev.ok != score.ok || prev.miss != score.miss)
            {
                ShowPgm();
            }
            prev = score;
            yield return null;

        }

    }
    public static void Effect(JudgeType type)
    {
        //        if (!_fx.ContainsKey(type)) return;
        var obj = _fx[type];
        foreach (var i in _fx.Values) i.Hide();
        obj.Make();

        ShowCombo();
        ShowPgm();
    }

    public static void ShowEarlyLate(float offset)
    {
        if (useEL)
        {
            ShowEL(offset);
        }
    }
    public static void ShowPgm()
    {
        ScoreEntry entry = ScoreBoard.GetScoreEntry();
        pgmText.text = $"|  <color={yellow}>{entry.perfect}</color> / <color={blue}>{entry.good}</color> / <color={green}>{entry.ok}</color> / <color={red}>{entry.miss}</color>  |";
    }
    public static void ShowCombo()
    {

        var next = "COMBO\n" + ScoreBoard.combo;
        if(next != combo.text)
        {
            combo.text = next;
            comboFade.Stop();
            comboFade.Play();
        }
    }
    public static void ShowEL(float x)
    {
        Text txt = instance.elObj.GetComponent<Text>();
        Animation fade = instance.elObj.GetComponent<Animation>();

        txt.color = x < 0 ? instance.early : instance.late;
        txt.text = x < 0 ? "EARLY" : "LATE";
        if (Mathf.Abs(x) < elrange) txt.text = "";
        fade.Stop();
        fade.Play();
    }
}
