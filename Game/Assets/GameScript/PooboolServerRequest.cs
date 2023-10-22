#define LIVE
//TEST OR LIVE
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class PooboolServerRequest :MonoBehaviour
{
    [HideInInspector]
    public bool isDone = false;
#if TEST
    public static string server_url = "http://localhost:24464";
#else
    public static string server_url = "http://poobool1302.ddns.net:24464";
#endif
    #region Post Score
    IEnumerator UploadRecord(ScoreEntry score,AuthTicket ticket)
    {
        Debug.Log("Ticket Upload...");
        var data = ticket.Data;
        Dictionary<string, string> dic = GetRecordDictionary(score);
        dic.Add("token", BitConverter.ToString(data).Replace("-", ""));
        dic.Add("client_id", ""+SteamClient.SteamId);
        dic.Add("name", SteamClient.Name);
        dic.Add("music", Game.table.title);
        dic.Add("composer", Game.table.composer);
        dic.Add("difficulty", Game.table.difficulty);
        dic.Add("hash", Game.pbrffdata.hash);
        dic.Add("ownerId", ""+Game.content_entry.ownerID);
        dic.Add("modi", ""+(int)Game.modi.code);
        Debug.Log(Game.modi.code);
        UnityWebRequest www = UnityWebRequest.Post(server_url+"/api/auth/record", dic);
        yield return www.SendWebRequest();
        Debug.Log(www.result);
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Send Ticket Done");
        }
        isDone = true;
        GetUserRecords(SteamClient.SteamId);
    }
    public async void PostScore(ScoreEntry score)
    {
        isDone = false;
        var ticket = await SteamUser.GetAuthSessionTicketAsync();
        StartCoroutine(UploadRecord(score, ticket));
    }
#endregion

#region Get Leaderboard
    private Dictionary<string, string> GetRecordDictionary(ScoreEntry entry)
    {
        var dic = new Dictionary<string, string>();
        dic.Add("songcode", "" + entry.songcode);
        dic.Add("life", "" + entry.life);
        dic.Add("perfect", "" + entry.perfect);
        dic.Add("good", "" + entry.good);
        dic.Add("ok", "" + entry.ok);
        dic.Add("miss", "" + entry.miss);
        dic.Add("maxcombo", "" + entry.maxcombo);
        dic.Add("score", "" + entry.percent);
        return dic;
    }
    List<ScoreEntry> list = null;
    [Serializable]
    private struct SerializedLeaderboard
    {
        public List<ScoreEntry> leaderboard;
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
        }
    }
    IEnumerator CoGetLeaderboard(ulong songcode)
    {
        UnityWebRequest www = UnityWebRequest.Get(server_url+$"/api/leaderboard?songcode={songcode}");
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            var serial = JsonConvert.DeserializeObject<SerializedLeaderboard>(www.downloadHandler.text);
            list = serial.leaderboard;
        }
        isDone = true;
    }
    public List<ScoreEntry> Leaderboard
    {
        get => list;
    }
    public void GetLeaderboard(ulong songcode)
    {
        isDone = false;
        StartCoroutine(CoGetLeaderboard(songcode));
    }
    #endregion

    #region UserRecords
    List<ScoreEntry> records = null;
    public List<ScoreEntry> userRecords
    {
        get => records;
    }
    IEnumerator CoGetUserRecords(ulong userid)
    {
        UnityWebRequest www = UnityWebRequest.Get(server_url + $"/api/userRecords?userid={userid}");
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
        }
        else
        {
            var serial = JsonConvert.DeserializeObject<SerializedLeaderboard>(www.downloadHandler.text);
            records = serial.leaderboard;
        }
    }
    public void GetUserRecords(ulong userid)
    {
        records = null;
        StartCoroutine(CoGetUserRecords(userid));
    }
    #endregion
    #region Hash

    IEnumerator UploadHash(ulong songcode,string hash,AuthTicket ticket)
    {
        Debug.Log("Ticket Upload...");
        Debug.Log(ticket);
        var data = ticket.Data;
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("token", BitConverter.ToString(data).Replace("-", ""));
        dic.Add("client_id", "" + SteamClient.SteamId);
        dic.Add("name", SteamClient.Name);
        dic.Add("songcode",""+songcode);
        dic.Add("hash", hash);
        Debug.Log("Post Hash:"+hash);
        UnityWebRequest www = UnityWebRequest.Post(server_url + "/api/auth/hash", dic);
        yield return www.SendWebRequest();
        Debug.Log(www.result);
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Send Ticket Done");
        }
        isDone = true;
    }
    public async void PostHash(ulong songcode,string hash)
    {
        isDone = false;
        var ticket = await SteamUser.GetAuthSessionTicketAsync();
        StartCoroutine(UploadHash(songcode,hash,ticket));
    }
    #endregion
    #region Log
    IEnumerator coLog(string str)
    {
        Debug.Log("Send Log");
        var dic = new Dictionary<string, string>();
        dic.Add("log", str);

        UnityWebRequest www = UnityWebRequest.Post(server_url + "/log/log", dic);
        yield return www.SendWebRequest();
        Debug.Log(www.result);
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Send Log Done");
        }
        isDone = true;
    }

    public async void Log(string str)
    {
        isDone = false;
        StartCoroutine(coLog(str));
    }
    #endregion

    #region UserInfo
    IEnumerator coUserInfo(ulong userid, int count, string name)
    {
        Debug.Log("Send Log");
        var dic = new Dictionary<string, string>();
        dic.Add("userid", userid.ToString());
        dic.Add("count", count.ToString());
        dic.Add("name", name);

        UnityWebRequest www = UnityWebRequest.Post(server_url + "/user/info", dic);
        yield return www.SendWebRequest();
        Debug.Log(www.result);
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Send Log Done");
        }
        isDone = true;
    }
    public async void UserInfo(ulong userid, int count, string name)
    {
        isDone = false;
        StartCoroutine(coUserInfo(userid,count,name));
    }
    #endregion


    #region Checkhash
    Dictionary<(ulong,ulong), string> checkhash;//(userid,songcode)
    public bool isHashReady()
    {
        return checkhash != null;
    }
    public bool DoCheckhash(ItemWrapper item,string hash)
    {
        if (checkhash.ContainsKey((item.ownerID,item.Id)))
        {
            if (checkhash[(item.ownerID, item.Id)] == hash)
            {
                Debug.Log("Hash Valid");
                return true;
            } // 해쉬 검증
            else
            {
                Debug.Log("Hash Invalid");
                return false;
            }
        }
        Debug.Log("Hash Doesn't Exists");
        return false;
    }
    IEnumerator coGetHash()
    {
        UnityWebRequest www = UnityWebRequest.Get(server_url + "/api/Checkhash");
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            var serial = JObject.Parse(www.downloadHandler.text);
            checkhash = new Dictionary<(ulong,ulong), string>();
            Debug.Log(serial);
            foreach(JObject obj in serial.Property("hash").Value)
            {
                ulong userid = ulong.Parse(obj.Property("userid").Value.ToString());
                ulong songcode = ulong.Parse(obj.Property("songcode").Value.ToString());
                string hash = obj.Property("hash").Value.ToString();
                checkhash.Add((userid,songcode), hash);
            }
        }
        isDone = true;
    }
    public async void GetHash()
    {
        isDone = false;
        StartCoroutine(coGetHash());
    }
    #endregion
    private void Start()
    {
        if (_instance == null)
        {
            _instance = this;
            if (SteamClient.IsValid)
            {
                Log($"{SteamClient.Name} ({SteamClient.SteamId}) login, {UGCManager.LaunchCount()}");
                UserInfo(SteamClient.SteamId, UGCManager.LaunchCount(), SteamClient.Name);
                GetUserRecords(SteamClient.SteamId);
                GetHash();
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    public static PooboolServerRequest _instance;
    public static PooboolServerRequest instance 
    {
        get => _instance;
    }
}
