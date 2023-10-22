using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using Steamworks.Data;
using UnityEngine.SocialPlatforms.Impl;
using System;
using System.Linq;
using System.Globalization;
using UnityEngine.Profiling;
using Steamworks.ServerList;

public class LeaderBoardPanel : MonoBehaviour
{
    bool friendOnly = false;
    [SerializeField]
    GameObject prefab;
    [SerializeField]
    Text tf_song;
    [SerializeField]
    Transform content;
    [SerializeField]
    GameObject bt_myrank;
    [SerializeField]
    GameObject loading;
    int myrank;
    [SerializeField]
    Text modiLabel;
    [SerializeField]
    GameObject contentObj;
    // Start is called before the first frame update
    [SerializeField]
    LeaderboardList LeaderList;
    ulong code;
    List<ScoreEntry> Leaderboard;

    List<ScoreEntry> ShowList;
    public void SetFriendOnly(bool value)
    {
        friendOnly = value;
        ApplyViewMode();
    }
    public IEnumerator show()
    {
        Debug.Log("Open:" + code);
        isLoaded = false;
        modiLabel.text = Game.modi.name;
        //서버로부터 리더보드를 가져옴
        var request = PooboolServerRequest.instance;
        request.GetLeaderboard(code);
        LeaderList.Init();
        SetLoading(true);
        while (!request.isDone) yield return null;
        SetLoading(false);

        Leaderboard = request.Leaderboard;
        Leaderboard.Sort((x, y) => DateTime.Parse(y.date).CompareTo(DateTime.Parse(x.date)));
        Leaderboard.Sort((x, y) => Math.Sign(y.percent - x.percent));
        Leaderboard = Leaderboard.FindAll((x) => x.modi == (int)Game.modi.code).ToList();

        ApplyViewMode();
        isLoaded = true;
    }

    private void ApplyViewMode()
    {
        if (friendOnly)
        {
            var friends = SteamFriends.GetFriends().Select(x => x.Id).ToList();
            ShowList = Leaderboard.FindAll(x => friends.Contains(x.userid) || SteamClient.SteamId==x.userid).ToList();
        }
        else
        {
            ShowList = Leaderboard.ToList();

        }

        myrank = ShowList.FindIndex(x => x.userid == SteamClient.SteamId) + 1;
        bt_myrank.SetActive(ShowList.Exists((x) => x.userid == SteamClient.SteamId));
        dirty = true;
    }

    bool isLoaded = false;
    void SetLoading(bool value)
    {
        loading.SetActive(value);
        loading.GetComponent<Animator>().Play("LoadingRotation", -1, 0);
    }
    int pos;
    const int window = 20;
    public void MakeDirty()
    {
        dirty = true;
    }


    bool dirty = false; 
    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
        if (dirty && isLoaded)
        {
            dirty = false;
            //해당하는 리더보드 정보만 추출
            LeaderList.Show(false);
            LeaderList.Resize(ShowList.Count);
            LeaderList.Render(ShowList);
        }
    }
    public void Close()
    {
        SubsSelectUI.isLock = false;
        gameObject.SetActive(false);
    }
    public void Open(ulong songcode, string songname)
    {
        LeaderList.Show(false);
        myrank = -1;
        SubsSelectUI.isLock = true;
        code = songcode;
        tf_song.text = " "+songname;    
        gameObject.SetActive(true);
        StartCoroutine(show());
    }
    const int etnry_height = 52;
    public void MoveToMyRanking()
    {
        content.localPosition = Vector3.up* etnry_height * (myrank-1);
    }
    // TODO : 롱노트 작동방식 바꾸기
}
