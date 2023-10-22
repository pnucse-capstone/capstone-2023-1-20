using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
public class UGCEntry : MonoBehaviour
{
    public Text music_text;
    public Text level;
    public Image difLabel;
    public Text leveler;
    public Image icon;
    public GameObject btn_remove;
    public GameObject btn_download;
    public GameObject auth;
    public GameObject Special;
    public Text recommend;
    public ShowTooltip warning;
    ulong fileid;
    string icon_url;
    [SerializeField]
    GameObject loadicon; 
    public static Dictionary<ulong, Sprite> cache = new Dictionary<ulong, Sprite>();
    void Start()
    {
        RefreshIcon();
    }
    public void SetEntry(WorkEntry entry)
    {
        loadicon.SetActive(true);

        fileid = entry.fileid;
        music_text.text = entry.music;
        if(entry.level>=0 && entry.level <= 10)
        {
            level.text = entry.level + "";
        }
        else
        {
            level.text = "?";
        }
        leveler.text = entry.leveler;
        icon_url = entry.preview_url;
        Debug.Log("url:"+entry.preview_url);
        RefreshIcon();
        showButton();
    }

    async void showButton()
    {
        btn_remove.SetActive(false);
        btn_download.SetActive(false);
        var entry = await Steamworks.Ugc.Item.GetAsync(fileid);
        if (entry.HasValue)
        {
            auth.SetActive(entry.Value.HasTag("confirmed") || entry.Value.HasTag("recommended"));
            Special.SetActive(false);
            btn_remove.SetActive(entry.Value.IsSubscribed);
            btn_download.SetActive(!entry.Value.IsSubscribed);
            warning.text = "Warning:";
            difLabel.color = Color.white * 0.75F;
            foreach (var dif in GameInit.difficultySet.GetDifficulties())
            {
                if (entry.Value.HasTag(dif.name.ToLower()))
                {
                    difLabel.color = dif.color;
                    break;
                }
            }
            if (entry.Value.HasTag("bigfile")) 
            {
                warning.text += "File size is bigger than 8MB.";
                Special.SetActive(true);
            }

            recommend.text = (int)entry.Value.VotesUp-(int)entry.Value.VotesDown + "";
        }
    }
    [SerializeField] ButtonVisibility btVisible;
    public void onClickVisiblity()
    {
        ChangeVisibility();
    }
    public void RefreshVisibility(Steamworks.Ugc.Item entry)
    {
        if (entry.IsPublic)
        {
            btVisible.Set(ButtonVisibility.Type.Public);
        }
        else if (entry.IsFriendsOnly)
        {
            btVisible.Set(ButtonVisibility.Type.FriendsOnly);
        }
        else if (entry.IsPrivate)
        {
            btVisible.Set(ButtonVisibility.Type.Private);
        }
        else
        {
            Debug.Log("공개설정이 뭔가 이상해요");
        }
    }
    async void ChangeVisibility()
    {
        var entry = await Steamworks.Ugc.Item.GetAsync(fileid);
        if (entry.HasValue)
        {
            if (entry.Value.IsPublic)
            {
                await new Steamworks.Ugc.Editor(fileid).WithFriendsOnlyVisibility().SubmitAsync();
                btVisible.Set(ButtonVisibility.Type.FriendsOnly);
            }
            else if (entry.Value.IsFriendsOnly)
            {
                await new Steamworks.Ugc.Editor(fileid).WithPrivateVisibility().SubmitAsync();
                btVisible.Set(ButtonVisibility.Type.Private);
            }
            else if (entry.Value.IsPrivate)
            {
                await new Steamworks.Ugc.Editor(fileid).WithPublicVisibility().SubmitAsync();
                btVisible.Set(ButtonVisibility.Type.Public);
            }
            else
            {
                Debug.Log("공개설정이 뭔가 이상해요");
            }
        }
    }
    void RefreshIcon()
    {
        string local_url = WorkshopCache.root_url+ "/" + fileid;
        if (cache.ContainsKey(fileid))
        {
            icon.sprite = cache[fileid];
            loadicon.SetActive(false);

        }
        else if (File.Exists(local_url))
        {
            var bytes = File.ReadAllBytes(local_url);
            //Game.pbrffdata.icon_bytes = (byte[])bytes.Clone();
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            icon.sprite = sprite;
            loadicon.SetActive(false);

            CacheAdd(sprite);
        }
        else if (icon_url.StartsWith("http"))
        {
            StartCoroutine(getIconFromWeb(icon_url));
        }
        else
        {
            Debug.Log("어느것도 아니라서 기본아이콘 출력함");
            loadicon.SetActive(false);

        }
    }

    public IEnumerator getIconFromWeb(string url)
    {
        Debug.Log("icon:" + url);
        WWW www = new WWW(url);
        while (!www.isDone)
        {
            yield return null;
        }
        Texture2D texture = www.texture;

        if (texture.width > 512 || texture.height > 512)
        {
            texture = Utility.ScaleTexture(texture, 512, 512);
        }
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        icon.sprite = sprite;

        byte[] bytes = texture.EncodeToPNG();
        string local_url = WorkshopCache.root_url + "/" + fileid;
        File.WriteAllBytes(local_url, bytes);

        loadicon.SetActive(false);

        CacheAdd(sprite);
    }

    private void CacheAdd(Sprite sprite)
    {
        if (!cache.ContainsKey(fileid))
        {
            cache.Add(fileid, sprite);
        }
    }

    public void Select() 
    {
        gameObject.GetComponentInParent<IListPanel>().Select(fileid);
        Game.icon = icon.sprite;
    }

    public void DeleteSelect()
    {
        gameObject.GetComponentInParent<IListPanel>().Delete(fileid);
    }

}
