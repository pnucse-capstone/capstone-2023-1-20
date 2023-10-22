using Steamworks;
using Steamworks.Ugc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UGCPlayListPanel : MonoBehaviour, IListPanel
{
    List<GameObject> list;
    Dictionary<ulong, Steamworks.Ugc.Item> entries;
    public GameObject prefab;
    public PopUp popup;
    [SerializeField] Text page_txt;
    bool tag_recommend= true;
    bool tag_confirm = true;
    bool tag_subscribed = false;
    [SerializeField] GameObject unconfirmText;
    int page_number = 1;
    int lv = 0;
    public Text errorlog;
    public void OpenDiscordLink()
    {
        Application.OpenURL("https://discord.gg/Xt4CE53");
    }
    public void pagePrev()  
    {
        page_number = Math.Max(page_number-1, 1);
        page_txt.text = page_number+"";
        Listing();
    }
    public void pageNext()
    {
        page_number = page_number + 1;
        page_txt.text = page_number + "";
        Listing();
    }
    public void SetLevel(float value)
    {
        lv = (int)value;
        Listing();
    }
    public void SetrecommendTag(bool value)
    {
        page_number = 1;
        tag_recommend = value;
        Listing();
    }
    public void SetconfirmTag(bool value)
    {
        page_number = 1;
        tag_confirm = value;
        Listing();
    }
    public void SetSubscribedTag(bool value)
    {
        page_number = 1;
        tag_subscribed = value;
        Listing();
    }
    public enum Pivot{ Like, Date, Title}
    Pivot pivot = Pivot.Date;
    public void ChangeSortPivot(int value)
    {
        pivot = (Pivot)value;
        Listing();
    }
    public void Back()
    {
        SceneManager.LoadScene(SceneNames.SELECT);
    }
    public void SearchWith(string txt)
    {
        Listing(txt);
    }
    // Start is called before the first frame update
    void Start()
    {
        list = new List<GameObject>();
        Listing();

        if (PlayerPrefs.GetInt("WorkShopAlert", 0)== 0)
        {
            MessagePopUp.Open(Translation.GetUIText("workshop_first"));
            PlayerPrefs.SetInt("WorkShopAlert", 1);
        }
    }
    async void Listing(string txt = "")
    {
        unconfirmText.SetActive(!tag_confirm && !tag_recommend);
        entries = new Dictionary<ulong, Steamworks.Ugc.Item>();
        var q = Steamworks.Ugc.Query.Items.WhereSearchText(txt);
        List<WorkEntry> show = new List<WorkEntry>();

        Debug.Log("page:" + page_number);
        ResultPage? page = new ResultPage?();
        try
        {
            page = await q.GetPageAsync(page_number);

        }
        catch
        {
            Debug.Log("Page not exists");
        }

        if (page.HasValue)
        {
            Debug.Log("Page Has Value");
            var es = page.Value.Entries.ToList();
            switch (pivot)
            {
                case Pivot.Date:
                    es.Sort((x, y) => y.Created.CompareTo(x.Created));break;
                case Pivot.Like:
                    es.Sort((x, y) => Math.Sign(((int)y.VotesUp - (int)y.VotesDown)- ((int)x.VotesUp - (int)x.VotesDown))); break;
                case Pivot.Title:
                default:
                    es.Sort((x, y) => x.Title.CompareTo(y.Title)); break;
            }

            foreach (var i in es)
            {                
                if (!(new List<string>(i.Tags)).Contains("lv."+lv) && lv != 0) continue;

                if (
                    (
                     ((new List<string>(i.Tags)).Contains("recommended") && tag_recommend ) 
                     || 
                     ((new List<string>(i.Tags)).Contains("confirmed") 
                     && tag_confirm) 
                     ||
                     (!tag_recommend && !tag_confirm)
                    )
                    &&
                    (i.IsSubscribed || !tag_subscribed)) 
                {
                    WorkEntry entry = new WorkEntry(i);
                    show.Add(entry);
                    entries.Add(i.Id, i);
                };

            }
        }
        //        show.Sort((x,y));
        while (list.Count<show.Count)
        {
            Debug.Log("New Instance:" + list.Count);
            list.Add(Instantiate(prefab, transform) as GameObject);
        }
        for (int i = 0; i < list.Count; i++)
            list[i].SetActive(false);

        //        show.Sort((x,y)=>x.);
        for (int i=0;i<show.Count;i++)
        {
            list[i].SetActive(true);
            var temp = list[i];
            var entry = show[i];
            temp.GetComponent<UGCEntry>().SetEntry(entry);
//            list.Add(temp);
        }

    }


    public void Select(ulong id)
    {
        Download(id);
    }
    async void Download(ulong id)
    {
        Debug.Log("install");
        popup.Open();
        var entry = entries[id];
        Debug.Log(entry.Title);
        popup.SetMessage("Downloading...");
        if (!entry.IsSubscribed)
        {
            var result = await entry.Subscribe();
            if (result)
            {
                await entry.DownloadAsync((x) => popup.SetLoading(x));
                await EntryListLoader.Add(entry);
            }
            else
            {
                Debug.LogWarning("Subscribe Failed");
                errorlog.text = "ERROR: Steamworks Error";
            }
        }

        Listing();
        popup.Close();
    }
    public void Delete(ulong id)
    {
        DeleteAndUnsubs(id);
    }
    async void DeleteAndUnsubs(ulong id)
    {
        Debug.Log("delete:"+id);
        popup.Open();
        var entry = entries[id];
        popup.SetMessage("Unsubscribing...");
        var result = await entry.Unsubscribe();
        Debug.Log("unsubs:"+result);
        if(Directory.Exists(entry.Directory))
            Directory.Delete(entry.Directory,true);
        EntryListLoader.Remove(entry);
        Listing();
        popup.Close();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
interface IListPanel
{
    void Select(ulong id);
    void Delete(ulong id);
}
class ProgressClass : IProgress<float>
{
    float lastvalue = 0;
    PopUp popup;
    public ProgressClass(PopUp popup)
    {
        this.popup = popup;
    }
    public void Report(float value)
    {
        if (lastvalue >= value) return;
        lastvalue = value;
        popup.SetLoading(value);
    }
}