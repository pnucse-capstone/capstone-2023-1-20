using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using Color = UnityEngine.Color;
using System.Linq;
using System;
using Steamworks;
using System.Threading.Tasks;
using System.Diagnostics;
using UnityEngine.Profiling;

public class LeaderBoardEntry : MonoBehaviour
{
    [SerializeField]
    Text tf_user;
    [SerializeField]
    Image im_profile;
    [SerializeField]
    Text tf_perfect;
    [SerializeField]
    Text tf_good;
    [SerializeField]
    Text tf_ok;
    [SerializeField]
    Text tf_miss;
    [SerializeField] FCAPIcon fcap;

    [SerializeField]
    Text tf_maxcombo;

    [SerializeField]
    Text tf_score;


    [SerializeField]
    Text tf_timestamp;
    // Start is called before the first frame update
    ScoreEntry entry;
    int ranking;
    public void Reset()
    {

        gameObject.SetActive(false);
        entry.userid = 0;
        tf_user.text = "Loading...";
        im_profile.sprite = null;
    }
    public bool isLoaded()
    {
        if (entry.userid == 0) return false;
        if (tf_user.text== "Loading...") return false;
        if (im_profile.sprite == null) return false;
        return true;
    }
    public void SetEntry(int ranking,ScoreEntry entry)
    {
        gameObject.SetActive(true);
        this.entry = entry;
        this.ranking = ranking;
        if (ranking <= 3 || entry.percent == 1F)
        {
            tf_user.color = new Color(0.914F, 0.78F,1);
        }
        else
        {
            tf_user.color = Color.white;
        }
        tf_user.text = "Loading...";
        im_profile.sprite = null;
        SetName();
        tf_score.text = string.Format("{0:F2}%", entry.percent*100);
        tf_perfect.text = entry.perfect + "";
        tf_good.text = entry.good + "";
        tf_ok.text = entry.ok + "";
        tf_miss.text = entry.miss + "";
        tf_maxcombo.text = entry.maxcombo + "";
        tf_timestamp.text = entry.date + "";
        fcap.SetIcon(entry);

        RefreshProfile(entry.userid);
    }
    private void SetName()
    {
        string Name = new Friend(entry.userid).Name;
        if (Name == "[unknown]") Name = "Loading...";

        tf_user.text = FormatRanking(ranking) +" "+ Name;
    }

    static Dictionary<ulong,Sprite> profileCache = new Dictionary<ulong,Sprite>();
    async void RefreshProfile(ulong steamid)
    {
        var image = await SteamFriends.GetSmallAvatarAsync(steamid);


        if (image.HasValue)
        {
            if (profileCache.ContainsKey(steamid))
            {
                im_profile.sprite = profileCache[steamid];
            }
            else
            {
                Texture2D texture = GetTexture(image);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                im_profile.sprite = sprite;
                profileCache.Add(steamid, sprite);
            }
            SetName();
        }

    }

    private static Texture2D GetTexture(Steamworks.Data.Image? image)
    {
        int w = (int)image.Value.Width;
        int h = (int)image.Value.Height;
        Texture2D texture = new Texture2D(w, h);
        byte[] data = image.Value.Data;
        Color[] colors = new Color[w * h];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(data[i * 4 + 0] / 255F, data[i * 4 + 1] / 255F, data[i * 4 + 2] / 255F, data[i * 4 + 3] / 255F);
        }
        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }

    private string FormatRanking(int ranking)
    {
        string postfix;
        switch (ranking)
        {
            case 1: postfix = "st."; break;
            case 2: postfix = "nd."; break;
            case 3: postfix = "rd."; break;
            default: postfix = "th."; break;
        }
        return ranking+postfix;
    }
}
