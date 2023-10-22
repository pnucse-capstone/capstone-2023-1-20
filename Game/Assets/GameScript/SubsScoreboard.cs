using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubsScoreboard : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] Image icon;
    [SerializeField] RankObject rank;
    // Start is called before the first frame update
    public async void Set(ItemWrapper item,string hash)
    {

        if (SteamInit.isAuth)
        {
            StartCoroutine(CoSet(item, hash));
        }
    }
    IEnumerator CoSet(ItemWrapper item,string hash)
    {
        ulong songcode = item.Id;
        while (PooboolServerRequest.instance.userRecords == null)
        {
            yield return null;
        }
        while (!PooboolServerRequest.instance.isHashReady())
        {
            yield return null;
        }
        Debug.Log("Show Record");
        var records = PooboolServerRequest.instance.userRecords;
        if (!PooboolServerRequest.instance.DoCheckhash(item,hash) && PackSelectUI.pack is not MusicPackCustom) 
            // 현재 곡의 제작자+송코드 조합에 해당하는 해시값이 일치 
        {
            text.text = "ERROR";
            GetComponentInChildren<FCAPIcon>().SetIcon();
            rank.SetRank("");
        }
        else if (isRecordExists(songcode, records))
        {
            ShowRecord(songcode, records);
        }
        else if (Steamworks.SteamClient.IsValid)
        {
            ShowNone();
        }
        else
        {
            ShowOffline();
        }
    }

    private static bool isRecordExists(ulong songcode, List<ScoreEntry> records)
    {
        return records.Exists((x) => x.userid == SteamClient.SteamId && x.songcode == songcode && x.modi == (int)Game.modi.code);
    }

    private void ShowOffline()
    {
        text.text = "OFFLINE";
        GetComponentInChildren<FCAPIcon>().SetIcon();
        rank.SetRank("");
    }

    private void ShowNone()
    {
        text.text = string.Format("NONE", 0);
        GetComponentInChildren<FCAPIcon>().SetIcon();
        rank.SetRank("");
    }

    private void ShowRecord(ulong songcode, List<ScoreEntry> records)
    {
        var record = records.Find((x) => x.userid == SteamClient.SteamId && x.songcode == songcode && x.modi == (int)Game.modi.code);
        text.text = string.Format("{0:F2}%", record.percent * 100);
        GetComponentInChildren<FCAPIcon>().SetIcon((FCAP)record.maxfcap); // 최고 fcap표시
        rank.SetRank(record.rank);
    }
}
