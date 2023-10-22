using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class RecordTreeEntry : MonoBehaviour
{
    [SerializeField]
    Image cap;
    [SerializeField]
    FCAPIcon recordIcon;
    [SerializeField]
    Color hard, normal;
    ScoreEntry score;
    TableMetaData meta;
    [SerializeField]
    Color colorLow,colorNormal,colorHigh;
    [SerializeField]
    Image frame;

    [SerializeField]
    Sprite circleNight, circleDay;
    private void OnMouseEnter()
    {

        GameObject.Find("Balloon").GetComponent<RecordsBalloon>().Set(score, meta);
    }
    private void OnMouseExit()
    {

        GameObject.Find("Balloon").GetComponent<RecordsBalloon>().Refresh();
    }
    public void Set(ItemWrapper song,TableMetaData table)
    {
        var dif = GameInit.difficultySet.GetDifficulty(table.difficulty);
        cap.color = dif.color;
        var record = PooboolServerRequest.instance.userRecords.Find((x) => song.Id == x.songcode && x.modi == 0);
        recordIcon.SetIcon((FCAP)record.maxfcap);
        frame.sprite = circleNight;
        SetClearColor(record, colorNormal, colorHigh);
        meta = table;
        score = record;

        RecordTree.scores.Add((meta,record));
    }

    private void SetClearColor(ScoreEntry record,Color colorNormal, Color colorLow)
    {
        if (record.percent >= 0.99F && record.miss == 0)
        {
            frame.color = colorNormal;
        }
        else if (record.isClear())
        {
            frame.color = colorNormal;
        }
        else
        {
            frame.color = colorLow;
        }
    }
}
