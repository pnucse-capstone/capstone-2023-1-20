using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordTree : MonoBehaviour
{
    [SerializeField]
    GameObject PackLine;
    List<RecordTreePackLine> packLines = new List<RecordTreePackLine>();

    public static List<(TableMetaData meta,ScoreEntry score)> scores;
    // Start is called before the first frame update
    void Start()
    {
        scores = new List<(TableMetaData, ScoreEntry)>();
        var list = MusicPack.GetPacks();
        foreach (var pack in list)
        {
            if (pack is MusicPackCustom || pack is MusicPackGimmick) continue;
            var obj = Instantiate(PackLine, transform);
            var packline = obj.GetComponentInChildren<RecordTreePackLine>();
            packline.Set(pack);
            packLines.Add(packline);
        }

        unlockAchievement("charote");

    }
    void unlockAchievement(string name)
    {
        Debug.Log("Try Unlock");
        var ach = new Steamworks.Data.Achievement(name);
        try
        {
            ach.Trigger();
        }
        catch (System.Exception)
        {
            Debug.Log("Achievement ERROR");
        }
    }
}
