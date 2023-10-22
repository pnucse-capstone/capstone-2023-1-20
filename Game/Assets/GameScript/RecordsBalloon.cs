using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class RecordsBalloon : MonoBehaviour
{
    [SerializeField]
    Text text;
    [SerializeField]
    Charote charote;

    [SerializeField]
    FMODSFX source;

    bool sleep = false;
    bool wakeup = false;
    void Start()
    {
        if((DateTime.Now.Hour< 8 || DateTime.Now.Hour>22) && !tutorial) 
        {
            SetText("sleep");
            sleep = true;
        }
        else if (isComback())
        {
            SetText("comeback");
        }
        else if (trigger == "newRecord")
        {
            trigger = null;
            int no = UnityEngine.Random.Range(1, 3);
            SetText($"newrecord{no}");
        }
        else if (trigger == "packClear")
        {
            SetText("packClear");
        }
        else
        {
            int no = UnityEngine.Random.Range(1, 4);
            SetText($"meet{no}");
        }
        StartCoroutine(EasterEgg());
    }

    [SerializeField]
    bool tutorial = false;

    public static string trigger = null;
    bool cool = false;
    private void PlaySFX()
    {

        if (sleep) return;
        source.Play();
    }
    IEnumerator CoSFX()
    {
        cool = true;
        yield return new WaitForSeconds(1);
        cool = false;
    }
    IEnumerator EasterEgg()
    {
        while (sleep) yield return null;
        yield return new WaitForSeconds(600);
        text.text = Translation.GetDialog("wait");
    }
    public void Set(MusicPack pack, float percent,FCAP fcap)
    {

        if (sleep) return;
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("<size=20>");
        stringBuilder.AppendLine(pack.title);
        stringBuilder.Append("</size>");
        stringBuilder.Append($"{percent * 100:F2}%");
        if(fcap != FCAP.N)stringBuilder.AppendLine();
        switch (fcap)
        {
            case FCAP.AP: stringBuilder.Append("Pack All Perfect!");break;
            case FCAP.GP: stringBuilder.Append("All Good Play!"); break;
            case FCAP.FC: stringBuilder.Append("All Full Combo!"); break;
            case FCAP.C: stringBuilder.Append("All Clear!"); break;
            case FCAP.N:  break;
        }
        text.text = stringBuilder.ToString();
        // 

        if (percent == 1)
        {
            charote.Set(Charote.Emotion.SMILE);
        }
        else
        {
            charote.Set(Charote.Emotion.IDLE);
        }
        PlaySFX();
    }
    public void Set(ScoreEntry score, TableMetaData song)
    {
        if (sleep) return;
        StringBuilder stringBuilder= new StringBuilder();
        stringBuilder.Append("<size=20>");
        stringBuilder.AppendLine(Utility.RemoveSizeTag(song.composer));
        stringBuilder.Append("</size>");
        stringBuilder.Append("<b>");
        stringBuilder.AppendLine($"{song.title} ({song.difficulty.ToUpper()}) Lv.{song.level}");
        stringBuilder.Append("</b>");
        if(score.percent == 0 && score.miss == 0)
        {
            //            stringBuilder.Append("You haven't played it yet.");
            stringBuilder.Append(Translation.GetDialog("notplay"));
        }
        else
        {
            stringBuilder.Append("<size=20>");
            if(score.rank== "S")
            {
                stringBuilder.AppendLine($"{score.percent * 100:F2}% (<color=#781CFF>A+</color>)");
            }
            else
            {
                stringBuilder.AppendLine($"{score.percent * 100:F2}% ({score.rank})");
            }
            stringBuilder.AppendLine($"perfect:{score.perfect}");
            stringBuilder.AppendLine($"good:{score.good}");
            stringBuilder.AppendLine($"ok:{score.ok}");
            stringBuilder.AppendLine($"miss:{score.miss}");
            stringBuilder.AppendLine($"max combo:{score.maxcombo}");
            stringBuilder.Append("</size>");
            stringBuilder.Append(score.fcap);
        }
        if(score.percent == 0 && score.miss == 0)
        {
            charote.Set(Charote.Emotion.IDLE);
        }
        else if(score.isGoodPlay())
        {
            charote.Set(Charote.Emotion.SMILE);
        }
        else if (score.life == 0)
        {
            charote.Set(Charote.Emotion.SAD);
        }
        else
        {
            charote.Set(Charote.Emotion.IDLE);
        }
        text.text = stringBuilder.ToString();
        PlaySFX();

    }

    public void Refresh()
    {
        //        text.text = "Anything else?";
        Talk("else");
    }
    public void Talk(string id)
    {
        SetText(id);
        PlaySFX();
    }
    public void Talk()
    {
        float prob = UnityEngine.Random.Range(0F, 1F);
        if (sleep )
        {
            if (!wakeup)
            {
                Talk("sleep");
                source.Play();
                charote.Bounce();
                StartCoroutine(Wakeup());
            }
        }
        else if (prob < 0.9F)
        {
            RandomTalk();
        }
        else
        {
            SpecialTalk();
        }
    }

    public void BestSummarize()
    {

        if (sleep) return;
        StringBuilder stringBuilder = new StringBuilder();


        var list = RecordTree.scores.FindAll(x => x.score.isAllPerfect());
        if (list.Count != 0)
        {
            stringBuilder.AppendLine("<size=20>Best All Perfect</size>");
            list.Sort((x, y) => y.meta.level - x.meta.level);
            stringBuilder.AppendLine($"<b>{list[0].meta.title} ({list[0].meta.difficulty}) Lv. {list[0].meta.level}</b>");
        }


        list = RecordTree.scores.FindAll(x => x.score.isGoodPlay());
        if (list.Count != 0)
        {
            stringBuilder.AppendLine("<size=20>Best Good Play</size>");
            list.Sort((x, y) => y.meta.level - x.meta.level);
            stringBuilder.AppendLine($"<b>{list[0].meta.title} ({list[0].meta.difficulty}) Lv. {list[0].meta.level}</b>");
        }

        
        list = RecordTree.scores.FindAll(x => x.score.isFullCombo());
        if(list.Count != 0)
        {
            stringBuilder.AppendLine("<size=20>Best Full Combo</size>");
            list.Sort((x, y) => y.meta.level - x.meta.level);
            stringBuilder.AppendLine($"<b>{list[0].meta.title} ({list[0].meta.difficulty}) Lv. {list[0].meta.level}</b>");
        }


        list = RecordTree.scores.FindAll(x => x.score.isClear());
        if (list.Count != 0)
        {
            stringBuilder.AppendLine("<size=20>Best Clear</size>");
            list.Sort((x, y) => y.meta.level - x.meta.level);
            stringBuilder.Append($"<b>{list[0].meta.title} ({list[0].meta.difficulty}) Lv. {list[0].meta.level}</b>");
        }
        charote.Set(Charote.Emotion.IDLE, true);
        if(stringBuilder.Length == 0)
        {
            stringBuilder.Append(Translation.GetDialog("notplay"));
        }
        text.text = stringBuilder.ToString();
        PlaySFX();
    }
    IEnumerator Wakeup()
    {
        wakeup= true;
//        yield return new WaitForSeconds(1);
        charote.Set(Charote.Emotion.WAKE,false);
        yield return new WaitForSeconds(2);
        sleep = false;
        Talk("wake");

    }
    bool isComback()
    {
        int days = (int)(TimeSpan.FromTicks(DateTime.Now.Ticks).TotalDays);

        int prev = PlayerPrefs.GetInt("prevLogin", days);
        PlayerPrefs.SetInt("prevLogin",days);
        return days - prev > 3;
    }
    private void RandomTalk()
    {
        int no = UnityEngine.Random.Range(1, 22);
        var id = $"dialog{no}";
        Talk(id);
    }
    void SpecialTalk()
    {
        List<string> ids= new List<string>();
        if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
        {
            ids.Add("weekend");
        }
        else
        {
            if (DateTime.Now.Hour<12)
            {
                ids.Add("weekday");
            }
        }
        if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
        {
            ids.Add("friday");
        }
        if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
        {
            ids.Add("Monday");
        }
        if (DateTime.Now.Hour <12)
        {
            ids.Add("weekday");
        }
        if (ids.Count == 0)
        {
            RandomTalk();
        }
        else
        {
            Talk(ids[UnityEngine.Random.Range(0, ids.Count)]);
        }
    }
    public void SetText(string id)
    {
        if (sleep) return;
        text.text = Translation.GetDialog(id);
        charote.Set(Translation.GetEmotion(id));
    }
}
