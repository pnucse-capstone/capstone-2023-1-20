using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SocialPlatforms.Impl;

public static class ScoreBoard
{
    static Dictionary<JudgeType,Utility.XORInt32> _amount = null;
    static int _combo;
    static int _maxcombo;
    public static float life = 1;
    public static ScoreEntry GetScoreEntry()
    {
        return new ScoreEntry() {
            life = life,
            maxcombo = _maxcombo,
            perfect = _amount[JudgeType.perfect],
            good = _amount[JudgeType.good],
            ok = _amount[JudgeType.ok],
            miss = _amount[JudgeType.miss],
            modi = (int)Game.modi.code,
            songcode = Game.content_entry?.Id ?? 0
        };
    }

    public static bool isAllPerfect()
    {
        return _amount[JudgeType.good] == 0 && _amount[JudgeType.ok] == 0 && _amount[JudgeType.miss] == 0;
    }

    internal static bool isGoodPlay()
    {
        return _amount[JudgeType.ok] == 0 && _amount[JudgeType.miss] == 0;
    }

    public static bool isFullCombo()
    {
        return _amount[JudgeType.miss] == 0;
    }
    public static bool isEmpty()
    {
        return _amount[JudgeType.miss] + _amount[JudgeType.good] + _amount[JudgeType.ok] + _amount[JudgeType.perfect] == 0;
    }

    public static int combo
    {
        set
        {
            _maxcombo = Math.Max(value, _maxcombo);
            _combo = value;
        }
            get => _combo;
    }
    static ScoreBoard()
    {
        Reset();
    }
    public static void Reset()
    {
        combo = 0;
        _maxcombo = 0;
        _amount = new Dictionary<JudgeType, Utility.XORInt32>();
        foreach (JudgeType type in Enum.GetValues(typeof(JudgeType)))
        {
            _amount.Add(type,0);
        };
    }
    public static float percent
    {
        get => GetScoreEntry().percent;
    }
    public static string rank
    {
        get
        {
            if (percent >= 0.99F && _amount[JudgeType.miss] == 0)
            {
                return "A+";
            }
            else if(percent >= 0.95F)
            {
                return "A";
            }
            else if(percent >= 0.90F)
            {
                return "B+";
            }
            else if (percent >= 0.85F)
            {
                return "B";
            }
            else if (percent >= 0.80F)
            {
                return "C+";
            }
            else if (percent >= 0.75F)
            {
                return "C";
            }
            else 
            {
                return "D";
            }

        }
    }

    static byte fcap
    {
        get
        {
            if (_amount[JudgeType.good] == 0 && _amount[JudgeType.miss] == 0)
            {
                return 3;
            }
            if (_amount[JudgeType.miss] == 0)
            {
                return 2;
            }
            if (life !=0)
            {
                return 1;
            }
            return 0;
        }
    }
    static float[] life_bonus = { 0.01F*1.5F, 0.005F * 1.5F, 0F, -0.1F * 1.5F };
    public static void Write(JudgeType type)
    {
        if(life !=0F)
            life += Game.table.penalty*life_bonus[(int)type];
        life = Mathf.Clamp(life, 0F, 1F);
        if (type == JudgeType.miss) combo = 0;
        else combo++;
        _amount[type]++;
        _amount[type].SetSalt();
    }
    public static void Erase(JudgeType type)
    {
        Debug.Log("ERASE:" + type);
        if (life != 0F)
            life -= Game.table.penalty * life_bonus[(int)type];
        life = Mathf.Clamp(life, 0F, 1F);
        if (type != JudgeType.miss) combo--;
        _amount[type]--;

    }

}
[Serializable]
public struct ScoreEntry
{
    public ulong songcode;
    public ulong userid;
    public int perfect;
    public int good;
    public int ok;
    public int modi;
    public int miss;
    public float life;
    public int maxcombo;
    public string date;
    public int maxfcap;
    public float percent
    {
        get
        {
            var p = perfect;
            var g = good;
            var o = ok;
            var m = miss;
            if (p + g + o + m == 0) return 0;
            return (p + 0.8F * g+0.6F*o) / (p + g +o+ m);
        }
    }
    public bool isFullCombo()
    {
        return miss==0 && percent != 0;
    }
    public bool isGoodPlay()
    {
        return miss == 0 && ok==0 && percent != 0;
    }
    public bool isAllPerfect()
    {
        return miss == 0 && ok == 0 && good ==0 && percent != 0;
    }

    public bool isClear()
    {
        return life != 0 && percent != 0;
    }

    public string fcap
    {
        get
        {
            if (percent == 0)
            {
                return "";
            }
            else if (isAllPerfect()) return "All Perfect!";
            else if (isGoodPlay()) return "Good Play!";
            else if (isFullCombo()) return "Full Combo!";
            else if (isClear()) return "Clear!";
            else return "Fail...";
        }
    }

    public string rank
    {
        get
        {
            if(percent >= 0.99F)
            {
                return "S";
            }
            else if (percent >= 0.95F)
            {
                return "A+";
            }
            else if (percent >= 0.90F)
            {
                return "A";
            }
            else if (percent >= 0.80F)
            {
                return "B+";
            }
            else if (percent >= 0.70F)
            {
                return "B";
            }
            else if (percent >= 0.50F)
            {
                return "C+";
            }
            else if (percent >= 0.30F)
            {
                return "C";
            }
            else if (percent == 0 && perfect + good + ok + miss == 0)
            {
                return "";
            }
            else
            {
                return "D";
            }
        }
    }
}
