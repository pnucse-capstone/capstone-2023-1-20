using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Linq;

public class UGCListPanel : MonoBehaviour , IListPanel
{
    public GameObject prefab;
    List<GameObject> list;
    public GameObject last;
    public PopUp popup;
    public static ulong select;
    public Text ErrorText;
    WorkshopCache cache;
    public bool english = true;
    // Start is called before the first frame update
    void Start()
    {
        Log(Translation.GetUIText("ugc_idle"));
        Setup();
    }
    void Setup()
    {
        cache = new WorkshopCache();
        cache.Load();
        LocalListing();
        StartCoroutine(CacheReloader());
        isIdle = true;
    }
    public void Delete(ulong fileid)
    {
        
        MessagePopUp.Open(Translation.GetUIText("msg_delete"), () =>
        {
            DoDelete(fileid);
        });
    }
    async void DoDelete(ulong fileid)
    {

        isIdle = false;
        popup.Open();
        popup.SetMessage(Translation.GetUIText("ugc_delete"));
        Debug.Log("Start Delete");
        await cache.Delete(fileid);
        Debug.Log("Delete");
        LocalListing();
        popup.SetMessage(Translation.GetUIText("ugc_complete"));
        popup.Close();
        Debug.Log("Complete");
        Log(Translation.GetUIText("ugc_complete"));
        isIdle = true;
    }
    IEnumerator CacheReloader()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
        }
    }
    public void Refresh()
    {
        Log(Translation.GetUIText("ugc_idle"));
        LocalListing();
    }
    public void Select(ulong id)
    {        
        Debug.Log("Selected:"+id);
        MessagePopUp.Open(Translation.GetUIText("msg_edit"),()=>{ Edit(id); });
        
    }
    bool CheckEditor()
    {
        if (!NoteEditor.isLoaded || NoteEditor.now_open_url == null)
        {
            ErrorLog(Translation.GetUIText("ugc_error1"));
            return false;
        }
        if (DirtyChecker.isDirty())
        {
            ErrorLog(Translation.GetUIText("ugc_error2"));
            return false;
        }
        return true;
    }
    async void Edit(ulong id)
    {
        if (!CheckEditor()) return;
        isIdle = false;
        popup.Open();
        popup.SetMessage(Translation.GetUIText("ugc_upload"));
        var result = await cache.Edit(id, new ProgressClass(popup));
        if (!result.Success)
        {
            ErrorLog(Translation.GetUIText("ugc_error3"));
        }
        var bytes = new ZipUtility(WorkshopCache.pbrff_url).ReadEntry("table.json");
        PooboolServerRequest.instance.PostHash(result.FileId, Utility.GetHash(bytes));

        LocalListing();
        popup.Close();
        Log(Translation.GetUIText("ugc_complete"));
        isIdle = true;
    }
    async void LocalListing()
    {
        await cache.Reload();
        if (list != null)
            foreach (var i in list)
            {
                Destroy(i);
            }
        list = new List<GameObject>();
        foreach (WorkEntry entry in cache)
        {
            var temp = Instantiate(prefab, transform);
            Debug.Log(entry.preview_url);
            temp.GetComponent<UGCEntry>().SetEntry(entry);
            list.Add(temp);
            //                Debug.Log(entry.Id);
        }
        last.transform.SetAsLastSibling();
    }
    public static bool isIdle = false;
    public void Upload()
    {
        if (isIdle) Create();
    }
    async void Create()
    {
        if (!CheckEditor()) return;
        Debug.Log("업로드 시도");
        if (!NoteEditor.isLoaded || NoteEditor.now_open_url == null)
        {
            ErrorLog(Translation.GetUIText("ugc_error1"));
            return;
        }
        popup.Open();
        isIdle = false;
        popup.SetMessage(Translation.GetUIText("ugc_upload"));
        var result = await cache.Create(new ProgressClass(popup));
        if (result.Result==Steamworks.Result.OK)
        {
            string hash = CalcHash();
            PooboolServerRequest.instance.PostHash(result.FileId, hash);

            Log(Translation.GetUIText("ugc_complete"));
        }
        else
        {
            switch (result.Result)
            {
                case Result.LimitExceeded: ErrorLog(Translation.GetUIText("ugc_error3")); break;
                default: ErrorLog(Translation.GetUIText("ugc_error3"));break;
            }
            Debug.Log("error");
        }
        popup.SetMessage(Translation.GetUIText("ugc_complete"));
        LocalListing();
        popup.Close();
        isIdle = true;
    }

    private static string CalcHash()
    {
        var pbrff = new ZipUtility(WorkshopCache.pbrff_url);
        var bytes = pbrff.ReadEntry("table.json");
        var hash = Utility.GetHash(bytes);
        return hash;
    }

    public void ErrorLog(string text)
    {
        Debug.Log(text);
        ErrorText.color = Color.red;
        ErrorText.text = "*"+text;
    }
    public void Log(string text)
    {
        ErrorText.color = Color.white;
        ErrorText.text = "*"+text;
    }
}

[Serializable]
public class WorkshopCache:IEnumerable, IEnumerator // 자신이 올린 UGC를 목록화. 주기적으로 서버에 쿼리해서 유효성 검사
{
    public List<WorkEntry> My;
    public List<ulong> deleted_ids;

    public static string root_url
    {
        get
        {
            string path = Path.Combine(Application.persistentDataPath, "IconCache");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
    }
    public static string pbrff_url
    {
        get => Path.Combine(root_url,"temp","temp.pbrff");
    }
    public static string content_url
    {
        get => Path.Combine(root_url,"temp");
    }
    public static string icon_url
    {
        get => Path.Combine(root_url , "icon.png");
    }
    public WorkshopCache() 
    { 
        My = new List<WorkEntry>();
        deleted_ids = new List<ulong>();
    }
    void Add(WorkEntry entry)
    {
        if (!My.Exists((x)=>x.fileid==entry.fileid))
        {
            My.Add(entry);
        }
        else
        {
            int i = My.FindIndex((x)=>x.fileid==entry.fileid);
            My[i].level = entry.level;
            My[i].leveler = entry.leveler;
            My[i].music = entry.music;
            My[i].preview_url = entry.preview_url;
        }
        Save();
    }
    public Steamworks.Ugc.Editor WithTags(Steamworks.Ugc.Editor target)
    {
        if (Game.table.use_script) target = target.WithTag("Special");
        if (new FileInfo(content_url+"/temp.pbrff").Length > 8*(1<<20)) target = target.WithTag("BigFile");
        return target.WithTitle((Game.table.composer ?? "Unknown") + " - " + (Game.table.title ?? "Unknown"))
        .WithDescription("Made by:" + Game.table.leveler + "\r\n" + Game.table.description)
        .WithPublicVisibility()
        .WithTag("Table")
        .WithTag(Game.table.difficulty)
        .WithTag("Lv." + Game.table.level)
        .WithPreviewFile(icon_url)
        .WithContent(new DirectoryInfo(content_url));
    }
    public async Task<Steamworks.Ugc.PublishResult> Create(IProgress<float> progress) // 현재 열려있는 테이블 정보 이용해서 생성
    {
        bool ready = ReadyContents();
        if (!ready) return new Steamworks.Ugc.PublishResult() { Result = Result.Fail};
        var result = await WithTags(Steamworks.Ugc.Editor.NewCommunityFile)
            .SubmitAsync(progress);
        if (result.Success)
        {
            var item = await Steamworks.Ugc.Item.GetAsync(result.FileId);
            
            if (item.HasValue)
            {
                await item.Value.Subscribe();
                string local_url = root_url + "/"+result.FileId.Value;
                File.Copy(icon_url, local_url);
                Add(new WorkEntry(result.FileId.Value, Game.table.composer + " - " + Game.table.title, Game.table.leveler, Game.table.level, local_url));
                Debug.Log(("ADD", result.FileId, Game.table.title));
            }
            else
            {
                Debug.LogError("Failed to Subscribe the item");
            }
        }
        else
        {
            await Steamworks.SteamUGC.DeleteFileAsync(result.FileId);
        }
//        Debug.Log("업로드 실패:"+(icon_url,result.Result));
        return result;
    }

    public async Task Reload()
    {
        var q = Steamworks.Ugc.Query.Items.WhereUserPublished(SteamClient.SteamId).RankedByTextSearch().RankedByVote();
        var page = await q.GetPageAsync(1);
        Debug.Log("reload:" +My.Count);
        if (page.HasValue)
        {
            Debug.Log($"This page has {page.Value.ResultCount}");
            List<WorkEntry> query_ids = new List<WorkEntry>();
            foreach (Steamworks.Ugc.Item entry in page.Value.Entries)
            {

                if (entry.Result == Result.OK)
                {
                    var temp = new WorkEntry(entry);
                    query_ids.Add(temp); 
                }
                else
                {
                    Debug.Log(entry.Result);
                };
            }
            // 1. 창작마당을 쿼리해서 내가 올린 걸 가져온다
            query_ids = query_ids.FindAll((x)=>!deleted_ids.Contains(x.fileid));

            // 2. 쿼리에 잡히지만 삭제한 것은 제외한다.
            My = My.FindAll((x)=>query_ids.Exists((y)=>y.fileid==x.fileid));
            Debug.Log(("step 3.", My.Count));
            // 3. 쿼리에 잡히지 않는 캐시를 제거한다

            query_ids = query_ids.FindAll((x) => !My.Exists((y) => y.fileid == x.fileid));
            Debug.Log(query_ids.Count);
            foreach(var i in query_ids)
            {
                Add(i);
            }
            // 4. 쿼리에 잡히지만 My에 없는 것을 복원한다.            
            Save();
        }
    }
    public async Task Delete(ulong fileid)
    {
        var result = await Steamworks.Ugc.Item.GetAsync(fileid);
        var a = await Steamworks.SteamUGC.DeleteFileAsync(fileid);
        Remove(fileid);
        Debug.Log("Delete file:"+fileid);
    }

    void Remove(ulong fileid) 
    {
        My = My.FindAll((x) => x.fileid != fileid);
        if(!deleted_ids.Contains(fileid))deleted_ids.Add(fileid);
        Save();

    }
    public void Save()
    {
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(Application.persistentDataPath+"/MySongList1",json);
    }
    public void Load()
    {
        string path = Application.persistentDataPath + "/MySongList1";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            My = JsonUtility.FromJson<WorkshopCache>(json).My;
        }
    }
    private static string CalcHash()
    {
        var pbrff = new ZipUtility(WorkshopCache.pbrff_url);
        var bytes = pbrff.ReadEntry("table.json");
        var hash = Utility.GetHash(bytes);
        return hash;
    }

    bool ReadyContents()
    {
        Debug.Log("Ready folder");
        if (!Directory.Exists(content_url)) Directory.CreateDirectory(content_url);
        else
        {
            Directory.Delete(content_url,true);
            Directory.CreateDirectory(content_url);
        }
        Debug.Log("Ready file");
        if (File.Exists(NoteEditor.now_open_url)) 
        {
//            bytes = File.ReadAllBytes(NoteEditor.now_open_url);
            File.Copy(NoteEditor.now_open_url, pbrff_url, true);

        }
        else return false;
        Debug.Log("Ready icon");
        if (Game.pbrffdata.icon != null)
        {
            File.WriteAllBytes(icon_url, Game.pbrffdata.icon.ToJPG());
        }
        else
        {
            File.WriteAllBytes(icon_url, PreviewIcon.DefaultSprite.texture.EncodeToPNG());
        }
        string hash = CalcHash();
        AttachHash(hash);
        return true;
    }
    void AttachHash(string hash)
    {

        var pbrff = new ZipUtility(WorkshopCache.pbrff_url);
        var json = Encoding.UTF8.GetString(pbrff.ReadEntry("meta.json"));
        var jobj = JObject.Parse(json);
        jobj["hash"] = hash;
        Debug.Log(jobj);
        pbrff.WriteEntry("meta.json", Encoding.UTF8.GetBytes(jobj.ToString()));
    }

    public async Task<Steamworks.Ugc.PublishResult> Edit(ulong fileid,IProgress<float> callback)
    {
        bool ready = ReadyContents();
        if (!ready) return new Steamworks.Ugc.PublishResult() { Result = Result.Fail};
        var result = await WithTags(new Steamworks.Ugc.Editor(fileid))
                            .WithChangeLog(DateTime.Now.ToString())
                            .SubmitAsync(callback);
        if (result.Result == Result.OK)
        {
            var target = My.Find((x) => x.fileid == fileid);
            if (target != null)
            {
                target.fileid = fileid;
                target.level = Game.table.level;
                target.leveler = Game.table.leveler;
                target.music = Game.table.composer + " - " + Game.table.title;
                string local_url = root_url+"/"+fileid;
                File.Copy(icon_url, local_url,true);
                target.preview_url = local_url;
                Add(target);
            }
            return result;
        }
        else
        {
            MessagePopUp.Open("ERROR: Fail to Edit");
        }
        return result;
    }


    int index = 0;
    public void Reset()
    {
        index = 0;
    }
    public bool MoveNext()
    {
        if(index == My.Count - 1)
        {
            Reset();
            return false;
        }
        index++;
        return true;
    }
    public object Current
    {
        get=>My[index];
    }
    public IEnumerator GetEnumerator()
    {
        foreach(var i in My)
        {
            Debug.Log(JsonConvert.SerializeObject(i));
            yield return i;
        }
    }
}
[Serializable]
public class WorkEntry
{
    public ulong fileid;
    public string music;
    public int level;
    public string leveler;
    public string preview_url;
    public bool equals(WorkEntry a, WorkEntry b)
    {
        return a.fileid==b.fileid;
    }
    public WorkEntry(ulong fileid, string music, string leveler, int level, string preview_url) // 널값이면 로컬저장
    {
        this.fileid = fileid;
        this.music = music;
        this.leveler = leveler;
        this.level = level;
        this.preview_url = preview_url;

        if (string.IsNullOrEmpty(this.preview_url))
        {
            this.preview_url = Path.Combine(Application.persistentDataPath, "" + fileid);
            File.WriteAllBytes(this.preview_url, Game.pbrffdata.icon.bytes);
        }
    }
    public WorkEntry(Steamworks.Ugc.Item item)
    {
        fileid = item.Id;
        music = item.Title;
        preview_url = item.PreviewImageUrl;
        string[] tags = item.Tags;
        foreach (var i in tags)
        {
            if (i.StartsWith("lv."))
            {
                level = int.Parse(i.Substring("Lv.".Length));
            }
        }
        using (var s = new StringReader(item.Description))
        {
            string line = s.ReadLine();
            if (line.StartsWith("Made by:"))
            {
                leveler = line.Substring("Made by:".Length);
            }
        }

    }
    /*
    public static async Task<WorkEntry> Create(ulong fileid)
    {
        var f = await Steamworks.Ugc.Item.GetAsync(fileid);
        WorkEntry entry = new WorkEntry();
        entry.fileid = fileid;
        entry.music = f.Value.Title;
        entry.preview_url = f.Value.PreviewImageUrl;
        Debug.Log(f.Value.Result);
        if (f.HasValue)
        {
            entry.preview_url = f.Value.PreviewImageUrl;
            string[] tags = f.Value.Tags;
            foreach (var i in tags)
            {
//                Debug.Log(i);
                if (i.StartsWith("made by : "))
                {
                    entry.leveler= i.Substring("made by : ".Length);
                }
                if (i.StartsWith("lv."))
                {
                    entry.level = int.Parse(i.Substring("Lv.".Length));
                }
            }
            Debug.Log(("Created Entry...:", entry.fileid, entry.music, entry.level, entry.leveler));
        }
        else
        {
            Debug.Log("error");
        }
        return entry;
    }*/
}