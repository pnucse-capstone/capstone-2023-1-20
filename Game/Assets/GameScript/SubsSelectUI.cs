using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Steamworks.Ugc;
using UnityEngine.UI;
using System.IO.Compression;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.Profiling;
using Newtonsoft.Json.Linq;
using System.Threading;

public class SubsSelectUI : MonoBehaviour
{
    public Loading loading;
    public GameObject icon;
    public GameObject music_entry;
    public Text pagetxt;
    public SubsScoreboard score;
    public SimplePanel LoadingPanel;
    public LinkButton composerLink;
    [SerializeField] GameObject PanelEmpty;
    [SerializeField] GameObject Offline;
    [SerializeField] MusicPreview music;
    [SerializeField] DifficultySelect difficulty;
    [SerializeField] Button recordSceneButton;
    [SerializeField] FadeScreen fade;
    List<PbrffExtracter.PreviewEntry> entries;
    List<PbrffExtracter.PreviewEntry> entriesAll;
    public static int index = 0;
    static bool showExtrapackPopup = true;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isLock = false;
        entries = new List<PbrffExtracter.PreviewEntry>();
        recordSceneButton.interactable = PooboolServerRequest.instance.userRecords != null;
        StartCoroutine(DisplayAppear()); // GUI 
        StartCoroutine(EscapeHandler()); // GUI 



    }
    IEnumerator DisplayAppear()
    {

        yield return null;

        while (!EntryListLoader.isDone)
        {
            yield return null;
        }
        InitList();
        difficulty.Refresh();
        music_entry.GetComponent<Animator>().SetTrigger("Appear");
        LoadingPanel.Close();

        if (PackSelectUI.pack is MusicPackGimmick && showExtrapackPopup)
        {
            Debug.Log("Extra Pack");
            MessagePopUp.Open(Translation.GetUIText("extrapack"));
            showExtrapackPopup = false;
        }
    }
    PbrffExtracter.PreviewEntry focusMusic;
    public void InitList()
    {
        ChangeList(difficulty.GetNow().name);
        if (entries.Count == 0)
        {
            ChangeDifficultyNext();
        }
    }
    private void ChangeList(string difficulty)
    {
        entriesAll = EntryListLoader.GetEntries();
        var prev = entries;
        entries = entriesAll.Where((x) => { return x.difficulty.ToLower() == difficulty.ToLower(); }).ToList();
        // 특정 난이도만 찾음


        //곡 없음
        if(entriesAll.Count == 0)
        {
            PanelEmpty.SetActive(true);
            music_entry.GetComponent<Animator>().SetTrigger("Hide");
            return;
        }

        
        index = Mathf.Clamp(index, 0, Mathf.Max(0, entries.Count - 1));
        
        //현재 포커싱된 곡이 없음
        if (entries.Count != 0 && focusMusic == null)
        {
            focusMusic = entries[index];
        }

        // 난이도 변경시 이미 그 곡이 있는경우
        if (focusMusic != null )
        {
            var i = entries.FindIndex((x) => x.title == focusMusic.title && x.composer == focusMusic.composer );
            if (i != -1 )
            {
                index = i;
            }
        }
        // 해당 난이도 곡이 하나도 없는 경우 
        if (entries.Count == 0)
        {
        } 
        else
        {
            music_entry.SetActive(true);
            PanelEmpty.SetActive(false);
            Set(index);
            music_entry.GetComponent<Animator>().SetTrigger("Change");
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (isLock) return;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            onClickPrev();
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            ChangeDifficultyPrev();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            onClickPlay();
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            onClickNext();
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            ChangeDifficultyNext();

        }
    }
    IEnumerator EscapeHandler()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && isFade == false && isLock == false)
            {
                
                Exit();
                yield break;
            }
            yield return null;
        }

    }
    public static bool isLock= false;
    public void ChangeDifficultyNext()
    {
        if (EntryListLoader.GetEntries().Count != 0)
        {

            var now = difficulty.GetNow();
            do
            {
                now = GameInit.difficultySet.FindNext(now.name);
                ChangeList(now.name);
            } while (entries.Count == 0);
            difficulty.Change(now);
        }
    }
    public void ChangeDifficultyPrev()
    {

        var now = difficulty.GetNow();
        do
        {
            now = GameInit.difficultySet.FindPrev(now.name);
            ChangeList(now.name);
        } while (entries.Count == 0);
        difficulty.Change(now);
    }
    public void onDifficultyChange(string difficulty)
    {
        ChangeList(difficulty);
    }
    public void onClickNext()
    {
        index = (index + 1) % entries.Count;
        focusMusic = entries[index];
        Set(index);
        music_entry.GetComponent<Animator>().SetTrigger("Change");
    }
    public void onClickPlay()
    {
        isLock = true;
        if (PackSelectUI.pack.isDLCAvailable())
        {
            StartCoroutine(Play());
        }
        else
        {
            MessagePopUp.Open(Translation.GetUIText("msg_dlc")+$"\r\n(DLC - {PackSelectUI.pack.DLCTitle})");
        }
    }
    public void onClickPrev()
    {
        index = (index + entries.Count - 1) % entries.Count;
        focusMusic = entries[index];
        Set(index);
        music_entry.GetComponent<Animator>().SetTrigger("Change");
    }
    public void Exit()
    {
        Debug.Log("EXIT");
        music.enabled = false;
        enabled = false;
        fade.FadeOut(() => {
            SceneManager.LoadScene(SceneNames.INTRO);
            PbrffExtracter.ReleaseCache();
        });
    }
    bool isFade = false;
    public IEnumerator Play()
    {
        isFade = true;
        var entry = entries[index];
        loading.Popup();
        Game.autoplay = false;
        string url = entry.item.Directory; 
        var pbrff = new PbrffExtracter(PackSelectUI.pack.packKey);
        music.FadeOut();
        yield return new WaitForSeconds(1);
        var task = pbrff.LoadFullAsync(url);
        while (!task.IsCompleted)
        {
            yield return null;
        }
        Game.ApplyOnGame(task.Result);
        Game.icon = entry.icon;
        Game.content_entry = entry.item;
        var oper = Game.LoadSceneFromAsync(0, "game");
        oper.allowSceneActivation = false;
        while (oper.progress < 0.9F)
        {
            yield return null;
        }
        yield return null;
        loading.End(0.6F);
        yield return new WaitForSeconds(1.0F);
        oper.allowSceneActivation = true;
        //        Game.loadScene(0,"select");

    }
    public void Set(int index)
    {
        index = index % entries.Count;
        var entry = entries[index];
        music_entry.GetComponent<SubsSelectSongInfo>().Set(entries[index]);
        pagetxt.text = (index + 1) + "/" + entries.Count;
        music.SetPCM(entry.preview);
        composerLink.Set(entry.composerLink);
        RefreshRecord();
    }
    public void RefreshRecord()
    {
        score.Set(entries[index].item, entries[index].hash);
    }
    [SerializeField]
    LeaderBoardPanel leaderboard;
    public void OpenLeaderboard()
    {
        leaderboard.Open(entries[index].item.Id, entries[index].composer+" - "+entries[index].title);
    }
}
public static class EntryListLoader
{
    public static bool isDone
    {
        get
        {
            Debug.Log((loadedList.Count,Count));
            if (loadedList == null) return false;
            return loadedList.Count == Count;
        }
    }
    public static (int,int) progress
    {
        get
        {
            if (loadedList == null) return (0,1);
            return (loadedList.Count,Count);
        }
    }
    static List<PbrffExtracter.PreviewEntry> loadedList;
    static int Count = 0;
    public static List<PbrffExtracter.PreviewEntry> GetEntries()
    {
        return loadedList;
    }
    public static async Task Add(Item item)
    {
        Count++;
        var entry = await Extract(new OnlineItemWrapper(item));
        loadedList.Add(entry);
    }
    public static void Remove(Item item)
    {
        loadedList = (from i in loadedList where i.item.Id != item.Id select i).ToList();
        Count = loadedList.Count;
    }
    public static async Task Listing(bool overwrite)
    {
        if (!overwrite && loadedList != null) return;
        loadedList = new List<PbrffExtracter.PreviewEntry>();

        foreach(var item in await PackSelectUI.pack.GetSongs())
        {
            var value = await Extract(item);
            if(value != null)
            {
                loadedList.Add(value);
            }
        }
        Count = loadedList.Count;

    }
    async static Task<PbrffExtracter.PreviewEntry> Extract(ItemWrapper item)
    {
        Debug.Log("Extract Start");
        var file_path = item.Directory;
        if (!File.Exists(file_path))
        {
            Debug.Log("File Not Exist:" + file_path);
            return null;
        }
        return await Task.Run(() =>
        {
            PbrffExtracter pbrff = new PbrffExtracter(PackSelectUI.pack.packKey);
            pbrff.LoadPreviewData(file_path);
            var data = pbrff.GetPreviewData(item);
            return data;
        });
    }
}
static class UGCManager
{
    public static async Task DownloadDefault()
    {
        Debug.Log("Download Default Charts");
        var list = await SubscribeDefault();
        await Touch(list);
    }
    public static void AddLaunchCount()
    {
        if (SteamClient.IsValid)
        {
            Steamworks.SteamUserStats.AddStat("launch_count", 1);
            SteamUserStats.StoreStats();
        }
    }
    public static int LaunchCount()
    {
        if (SteamClient.IsValid)
        {
            var cnt = SteamUserStats.GetStatInt("launch_count");
            Debug.Log(cnt);
            return cnt;
        }
        return -1;
    }
    public static async Task<List<Item>> SubscribeDefault()
    {
        var q = Steamworks.Ugc.Query.Items.WithTag("Official").MatchAnyTag().RankedByTextSearch();
        var p =await q.GetPageAsync(1);
        var tasks = new List<Task>();
        if (p.HasValue)
        {
            foreach(var i in p.Value.Entries)
            {
                tasks.Add(i.Subscribe());
            }
            foreach (var i in tasks)
            {
                await i;
            }
        }
        return p.Value.Entries.ToList();
    }
    public static async Task Touch(List<Item> items) // 확인하고 없으면 다운로드
    {
        foreach (var i in items)
        {
            if (!File.Exists(i.Directory + "/temp.pbrff"))
            {
                await i.DownloadAsync();
                Debug.Log(i.Title);
            }
        }
    }

}
public abstract class ItemWrapper
{

    abstract public bool isOriginal{ get; }

    abstract public Item item { get; }
    abstract public string Directory { get; }
    abstract public Steamworks.Data.PublishedFileId Id{ get; }

    abstract public ulong ownerID { get; }
    abstract public Task<UserItemVote?> GetUserVote();
    abstract public void Vote(bool up);
}
public class OnlineItemWrapper:ItemWrapper
{

    public override bool isOriginal { get => false; }
    Item _item;
    public override Item item => _item;
    override public string Directory{
        get => item.Directory+"/temp.pbrff";
    }
    override public Steamworks.Data.PublishedFileId Id
    {
        get => item.Id;
    }

    public override ulong ownerID => item.Owner.Id;

    public OnlineItemWrapper(Item item)
    {
        _item = item;
    }
    override public Task<UserItemVote?> GetUserVote()
    {
        return item.GetUserVote();
    }
    override public void Vote(bool up)
    {
        item.Vote(up);
    }
}

public class OfflineItemWrapper : ItemWrapper
{
    public override bool isOriginal { get => false; }

    string directory_path;
    ulong id;
    public OfflineItemWrapper(string directory_path)
    {
        this.directory_path = directory_path+"/temp.pbrff";
        id = ulong.Parse(Path.GetFileName(directory_path));
        Debug.Log((id, this.directory_path));
    }
    public override Item item => throw new Exception("로컬 데이터는 아이템을 참조할수 없습니다.");
    override public string Directory
    {
        get => directory_path;
    }
    override public Steamworks.Data.PublishedFileId Id
    {
        get => id;
    }

    public override ulong ownerID => throw new NotImplementedException();

    override public Task<UserItemVote?> GetUserVote()
    {
        return null;
    }
    override public void Vote(bool up)
    {
    }
}

public class EmbededItemWrapper : ItemWrapper
{
    public override bool isOriginal { get => _isOriginal; }
    bool _isOriginal = false;
    string directory_path;
    ulong id;

    public EmbededItemWrapper(string name,ulong id,bool isOriginal = false)
    {
        _isOriginal = isOriginal;
        
        directory_path = Application.dataPath + "/StreamingAssets/" + name;
        this.id = id;
    }
    public override Item item => throw new Exception("로컬 데이터는 아이템을 참조할수 없습니다.");
    override public string Directory
    {
        get => directory_path;
    }
    override public Steamworks.Data.PublishedFileId Id
    {
        get => id;
    }

    public override ulong ownerID => Utility.DevId;

    override public Task<UserItemVote?> GetUserVote()
    {
        return null;
    }
    override public void Vote(bool up)
    {

    }
}
