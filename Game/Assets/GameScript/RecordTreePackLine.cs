using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.EventSystems.EventTrigger;

public class RecordTreePackLine : MonoBehaviour
{
    [SerializeField]
    GameObject LinePrefab;
    [SerializeField]
    Image iconTextrue;
    [SerializeField]
    FCAPIcon packEffect;

    List<RecordTreeLine> lines = new List<RecordTreeLine>();
    public async void Set(MusicPack pack)
    {

        iconTextrue.sprite = pack.icon;
        var difficulty = GameInit.difficultySet;
        List<(ItemWrapper, TableMetaData)> list = await GetMetaData(pack);
        foreach (var dif in difficulty.GetDifficulties())
        {
            if (dif.showInRecord)
            {
                var difsongs = list.Where(x => x.Item2.difficulty == dif).Select((x) => x.Item1).ToList();
                if (difsongs.Count != 0)
                {
                    var e = Instantiate(LinePrefab, transform);
                    var line = e.GetComponentInChildren<RecordTreeLine>();
                    line.SetList(difsongs);
                }
            }
        }

        SetPackEffect(list);
        this.pack = pack;
    }

    private static async System.Threading.Tasks.Task<List<(ItemWrapper, TableMetaData)>> GetMetaData(MusicPack pack)
    {
        List<(ItemWrapper, TableMetaData)> list = new List<(ItemWrapper, TableMetaData)>();
        foreach (var song in await pack.GetSongs())
        {
            var zip = new ZipUtility(song.Directory);
            byte[] bytes = zip.ReadEntry("meta.json");
            var table = JsonUtility.FromJson<TableMetaData>(Encoding.Default.GetString(bytes));
            list.Add((song, table));
        }

        return list;
    }

    MusicPack pack;
    float percent;
    FCAP fcap;

    private void SetPackEffect(List<(ItemWrapper,TableMetaData)> songs)
    {
        songs = songs.FindAll((x) => GameInit.difficultySet.GetDifficulty(x.Item2.difficulty).showInRecord);
        var records = PooboolServerRequest.instance.userRecords;
        if(songs.Any((x) => records.Find((y) => x.Item1.Id == y.songcode).percent ==0)){
            packEffect.SetIcon(FCAP.NotPlay);
        }
        else if (songs.All((x) => records.Find((y) => x.Item1.Id == y.songcode).maxfcap >= (int)FCAP.AP))
        {
            Debug.Log("AP");
            fcap = FCAP.AP;
//            fcap = "Pack All Perfect!";
            packEffect.SetIcon(FCAP.AP);
        }
        else if (songs.All((x) => records.Find((y) => x.Item1.Id == y.songcode).maxfcap == (int)FCAP.GP))
        {
            Debug.Log("GP");
            fcap = FCAP.GP;
//            fcap = "All Good Play!";
            packEffect.SetIcon(FCAP.GP);
        }
        else if (songs.All((x) => records.Find((y) => x.Item1.Id == y.songcode).maxfcap >= (int)FCAP.FC))
        {
            fcap = FCAP.FC;
//            fcap = "All Full Combo!";
            packEffect.SetIcon(FCAP.FC);
        }
        else if (songs.All((x) => records.Find((y) => x.Item1.Id == y.songcode).maxfcap >= (int)FCAP.C))
        {
            fcap = FCAP.C;
//            fcap = "All Clear!";
            packEffect.SetIcon(FCAP.C);
        }
        else
        {
            fcap = FCAP.N;
            packEffect.SetIcon(FCAP.N);
        }
        percent = songs.Sum((x) => records.Find((y) => x.Item1.Id == y.songcode).percent)/songs.Count;

        Debug.Log(fcap);

    }
    public void TalkPack()
    {
        Debug.Log("Enter");
        GameObject.Find("Balloon").GetComponent<RecordsBalloon>().Set(pack, percent,fcap);
    }
    public void TalkElse()
    {

        GameObject.Find("Balloon").GetComponent<RecordsBalloon>().Refresh();
    }


}
