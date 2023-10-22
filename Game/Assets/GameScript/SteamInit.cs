using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Steamworks.Data;
using System;

public class SteamInit : MonoBehaviour
{
    static bool isFirst = true;
    byte[] data;

    // Start is called before the first frame update
    byte[] SerializeForCloud()
    {
        return data;
    }
#if UNITY_STANDALONE
    async void LaunchCheck()
    {
        var ach = new Steamworks.Data.Achievement("welcome");
        try
        {
            ach.Trigger();
        }
        catch (System.Exception)
        {
            Debug.Log("Achievement ERROR");
        }
        if (isFirst)
        {
            DontDestroyOnLoad(gameObject);
            UGCManager.AddLaunchCount();
            isFirst = false;
        }
//        var task = EntryListLoader.Listing(false);
    }
    void Start()
    {
        try
        {
            SteamClient.Init(1735670, true);
            Debug.Log((SteamApps.GameLanguage,SteamApps.GameLanguage));

            if (!PlayerPrefs.HasKey("lang"))
            {
                Debug.Log("Set Language");
                Translation.ChangeLanguage(SteamUtils.SteamUILanguage);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
            //            StartCoroutine(GameShutdown());
        }
        CheckAuth();
        LaunchCheck();
    }
    async void CheckAuth()
    {
        try
        {
            var ticket = await SteamUser.GetAuthSessionTicketAsync();
            if(ticket != null)
            {
                Debug.Log("Auth Complete");
                _isAuth= true;
            }
            else
            {
                Debug.Log("Auth Failed");
                _isAuth = false;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            _isAuth = false;
        }

    }
    public GameObject errorPannel;
    public static bool isAuth { get => _isAuth; }
    static bool _isAuth = false;
    public static async void RecordScore(uint table_code, float score)
    {
        var leaderboard = await SteamUserStats.FindOrCreateLeaderboardAsync("score"+table_code,LeaderboardSort.Descending,LeaderboardDisplay.Numeric);
        if(leaderboard.HasValue)
        {
            var lb = leaderboard.Value;
            lb.SubmitScoreAsync((int)(score*100));   
        }
    }
    // Update is called once per frame
    void Update()
    {
//        if (Game.isDemo) return;
        if (Game.isPlaying) return;
        Steamworks.SteamClient.RunCallbacks();
        gameObject.transform.position = new Vector3(2 * Camera.main.orthographicSize, 0, 0);
    }
    private void OnApplicationQuit()
    {
        SteamClient.Shutdown();
    }
#endif
}