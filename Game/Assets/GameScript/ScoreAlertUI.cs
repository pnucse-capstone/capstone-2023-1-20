using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using Steamworks;

public class ScoreAlertUI : MonoBehaviour
{
    public Text title;
    public Text perfect;
    public Text good;
    public Text ok;
    public Text miss;
    public Text Lv;
    public Image Icon;
    public FCAPIcon icon;
    [SerializeField]
    RankObject rank;
    [SerializeField]
    Button btExit;
    [SerializeField]
    Button btRetry;
    [SerializeField]
    Text playbackSpeed;

    [SerializeField]
    GameObject newtext;
    [SerializeField] 
    Text scoretxt;

    [SerializeField]
    Text Mode;
    [SerializeField]
    GameObject ModeObj;
    // Start is called before the first frame update
    void unlockAchievement(string name)
    {

        var ach = new Steamworks.Data.Achievement(name); 
        try
        {
            Debug.Log("Unlock "+name);
            ach.Trigger();
        }
        catch (System.Exception)
        {
            Debug.Log("Achievement ERROR");
        }
    }
    void SendStastics()
    {
        try
        {
            WorkshopUtility.StageClearTag(Game.content_entry.item);
        }
        catch { }

    }
    void Start()
    {
        ModeObj.SetActive(Game.modi is not MODINone);
        Mode.text = $"{Game.modi.name}";
        if (Game.playSpeed != 1)
        {
            playbackSpeed.text = "x" + Game.playSpeed;
        }
        else
        {
            playbackSpeed.text = "";
        }
        Icon.sprite = Game.icon;
        Cursor.visible = true;
        //   if (Game.isCustom) setText(title, "Music - " + Game.custom_title + ' ' + " Lv." + Game.custom_level);
        ScoreEntry entry = ScoreBoard.GetScoreEntry();
        perfect.text = entry.perfect+"";
        good.text = entry.good + "";
        ok.text = entry.ok + "";
        miss.text = entry.miss + "";

        icon.SetIcon(entry);
        Debug.Log(entry.rank);
        rank.SetRank(entry.rank);
        title.text = Game.table.composer +" - "+Game.table.title + " ("+Game.table.difficulty+")";
        title.text = Utility.RemoveSizeTag(title.text);
        if (isRegularPlay())
        {
            Debug.Log("POST SCORE");
            PooboolServerRequest.instance.PostScore(entry);
        }
        if (Game.table.level>=0 && Game.table.level<=10)
        {
            Lv.text = "Lv." + Game.table.level;
        }
        else
        {
            Lv.text = "Lv.?";
        }
        if (SteamClient.IsValid)SendStastics();

        SetScore();
        //        unlockAchievement();
        //        sendStastics();
        StartCoroutine(CoUnlock());
    }
    IEnumerator CoUnlock()
    {
        while (true)
        {
            packClearAchievement();
            packDifficultyClearAchievement();
            yield return new WaitForSeconds(0.1F);
        }

    }
    bool isRegularPlay()
    {
        return Game.playSpeed >=1F;
    }
    void SetScore()
    {
        ScoreEntry entry = ScoreBoard.GetScoreEntry();
        scoretxt.text = string.Format("{0:F2}%", entry.percent * 100);

        if (SteamClient.IsValid) // new record
        {
            var prevRecord = PooboolServerRequest.instance.userRecords
                .Find((x) => x.songcode == Game.content_entry.Id 
                && x.modi == (int)Game.modi.code);
            Debug.Log((prevRecord.percent,entry.percent));

            if (prevRecord.percent < entry.percent)
            {
                newtext.SetActive(true);
                RecordsBalloon.trigger = "newRecord";
            }
            else
            {
                newtext.SetActive(false);

            }

        }
    }
    async void packClearAchievement()
    {
        var list = await PackSelectUI.pack.GetSongs();
        var records = PooboolServerRequest.instance.userRecords;
        /*
        foreach (var g in list.GroupBy(x => MusicPack.GetSonginfo(x.Id).meta.title + MusicPack.GetSonginfo(x.Id).meta.composer))
        {
            var clear = g.ToList().FindAll(x => records.Exists(y => x.Id == y.songcode && y.isClear()));
            if (clear.Count() == 0)
            {
                return;
            }
        }
        */
        var isPackClear = list.All((x) => records.Exists((y) => y.songcode == x.Id && y.isClear()));
        if (isPackClear)
        {
            unlockAchievement(PackSelectUI.pack.AchievementId + "clear");
        }
    }

    async void packDifficultyClearAchievement()
    {
        var list = await PackSelectUI.pack.GetSongs();
        var records = PooboolServerRequest.instance.userRecords;
        foreach (var g in list.GroupBy(x => MusicPack.GetSonginfo(x.Id).meta.difficulty))
        {
            var clear = g.ToList().FindAll(x => records.Exists(y => x.Id == y.songcode && y.isClear()));
            if (clear.Count() == g.Count())
            {
                unlockAchievement(PackSelectUI.pack.AchievementId + "clear"+g.Key);
            }
        }
        //        var isPackClear = list.All((x) => records.Exists((y) => y.songcode == x.Id && y.isClear()));
    }
    void setText(GameObject target, string text)
    {
        var txt = target.GetComponent<Text>();
        txt.text = text;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
        {
            btExit.onClick.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            btRetry.onClick.Invoke();
        }
    }
    bool isFade = false;
    public void toMain()
    {
        if(!isFade)StartCoroutine(FadeToMain());
    }
    [SerializeField]
    public FadeScreen fadescreen;
    IEnumerator FadeToMain()
    {
        isFade = true;
        fadescreen.FadeOut(() => SceneManager.LoadScene(SceneNames.SELECT));
        for (float i = 0; i < 0.5F; i += Time.deltaTime)
        {
            MusicPlayer.volume = (1 - i / 0.5F);
            yield return null;
        }

    }
    public void retry()
    {
        if (!isFade) StartCoroutine(Retry());
    }
    IEnumerator Retry()
    {
        isFade = true;
        Debug.Log(Game.table.isEnd());
        float vol = PlayerPrefs.GetFloat("volume", 0.4F);

        fadescreen.FadeOut(() => SceneManager.LoadScene(SceneNames.IN_GAME));
        yield return null;
    }
}