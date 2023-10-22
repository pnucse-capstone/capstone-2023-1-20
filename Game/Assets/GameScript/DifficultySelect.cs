using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DifficultySelect : MonoBehaviour
{

//    [SerializeField]
//    public SubsSelectUI selectUI;
    [SerializeField]
    DifficultySelectEntry Appear;
    [SerializeField]
    DifficultySelectEntry Disappear;
    DifficultyData difficulties;
    public Color GetColor(string difficulty)
    {
        var dif = difficulties.GetDifficulty(difficulty);
        return dif.color;
    }

    static DifficultyData.DifficultyType now;
    public string difficulty => now.name;
    void Start()
    {
        difficulties = GameInit.difficultySet;
        if(now ==null)now = difficulties.First();
        Show();
    }
    public void Change(DifficultyData.DifficultyType next)
    {
        var prev = now;
        now = next;
        DifficultyTransition(prev, next);
    }
    public DifficultyData.DifficultyType GetNow()
    {
        return now;
    }


    private void DifficultyTransition(DifficultyData.DifficultyType prev, DifficultyData.DifficultyType now)
    {
//        selectUI.onDifficultyChange(now.name);
        Appear.Show(now.name, now.color);
        Appear.Set(true);
        Disappear.Show(prev.name, prev.color);
        Disappear.Set(false);
    }

    void Show()
    {
        Appear.Set(true);
        Disappear.Set(false);

        Appear.Show(now.name,now.color);
    }

    public void Refresh()
    {
        Appear.Show(now.name, now.color);
        Appear.Set(true);

    }


    void Update()
    {
        
    }
}
