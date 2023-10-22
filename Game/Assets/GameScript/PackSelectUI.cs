#define RELEASE
// RELEASE or EARLYACCESS
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PackSelectUI : MonoBehaviour
{
    public List<MusicPack> packs = new List<MusicPack>();
    public static MusicPack pack;
    [SerializeField]
    Text packText;
    [SerializeField]
    GameObject blur;
    [SerializeField]
    FMODSFX source;
    [SerializeField]
    Animator ani;
    [SerializeField]
    Image packIcon;
    [SerializeField]
    FadeScreen fade;
    [SerializeField]
    FMODBGM music;
    int index = 0;
    bool uilock = false;
    // Start is called before the first frame update
    void Start()
    {
        packs = MusicPack.GetPacks();
        Select();
    }
    public void Open()
    {
        NoteEditor.PopupLock = true;
        gameObject.SetActive(true);
        ani.Play("Open", -1, 0);
        blur.SetActive(true);
    }
    public void Close()
    {
        NoteEditor.PopupLock = false;

        StartCoroutine(coClose());
    }
    IEnumerator coClose()
    {
        ani.Play("Close", -1, 0);
        yield return new WaitForSeconds(0.15F);
        gameObject.SetActive(false);
        blur.SetActive(false);
    }
    private void Update()
    {
        if (uilock) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Prev();
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            Next();
        }
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.S))
        {
            Finish();
        }
    }
    public void Finish()
    {
        if (uilock) return;
        uilock = true;
        StartCoroutine(coFinish());
    }
    IEnumerator coFinish()
    {
        bool fade2 = true;
        ani.Play("Close", -1, 0);
        var task = EntryListLoader.Listing(true);

        fade.FadeOut(() => { fade2 = false; });
        while (!task.IsCompleted || fade2)
        {
            yield return null;
        }
        Debug.Log("Loading Finish");
        SubsSelectUI.index = 0;
        RunPlaymode();
    }

    public void RunPlaymode()
    {
        NoteEditor.Reset();
        bool first = PlayerPrefs.GetInt("firstmeet", 1) == 1;
        if (first)
        {
            SceneManager.LoadScene(SceneNames.TUTORIAL);
        }
        else
        {
            SceneManager.LoadScene(SceneNames.SELECT);
        }
    }
    public void Select()
    {
        pack = packs[index];
        ani.Play("PackSelect", -1, 0);
        ani.SetBool("lock", !pack.isDLCAvailable());
        packIcon.sprite = pack.icon;
        packText.text = pack.title;
    }
    public void Next()
    {
        source.Play();
        index = (index + 1) % packs.Count;
        Select();
    }
    public void Prev()
    {
        source.Play();
        index = (index + packs.Count - 1) % packs.Count;
        Select();
    }
}
public abstract class MusicPack
{
    protected ulong EASY = 10000;
    protected ulong NORMAL = 11000;
    protected ulong HARD = 12000;
    protected ulong SPECIAL = 13000;
    protected ulong GIMMICK = 14000;
    private AppId DLCId = 0;
    public string DLCTitle => GetPackName(DLCId);
    static List<(ulong id, TableMetaData meta)> songinfo;
    static MusicPack()
    {
        LoadSonginfo();
    }

    public static List<(ulong id, TableMetaData meta)> GetSonginfos()
    {
        return songinfo;
    }
    public static (ulong id, TableMetaData meta) GetSonginfo(ulong id)
    {
        var x = songinfo.Find(x => x.id == id);
        return x;
    }
    async static void LoadSonginfo()
    {
        songinfo = new List<(ulong, TableMetaData)>();
        foreach (var pack in GetPacks())
        {
            if (pack is MusicPackCustom) continue;
            foreach (var song in await pack.GetSongs())
            {
                var zip = new ZipUtility(song.Directory);
                byte[] bytes = zip.ReadEntry("meta.json");
                var table = JsonUtility.FromJson<TableMetaData>(Encoding.Default.GetString(bytes));
                songinfo.Add((song.Id, table));
            }
        }

    }
    string GetPackName(ulong id)
    {
        switch (id)
        {
            case 2557340: return "Full Version";
            case 2320420: return "Author Wind Pack";
            default: return "ERROR";
        }
    }
    public virtual string AchievementId
    {
        get;
    }
    public MusicPack(int DLCId = 0)
    {
        this.DLCId = DLCId;
    }
    public bool isDLCAvailable()
    {
        if (DLCId != 0)
        {
            if (SteamClient.IsValid)
            {
                return Steamworks.SteamApps.IsDlcInstalled(DLCId);
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }
    public abstract string title { get; }
    public virtual string tooltip { get; }
    public abstract byte packKey { get; }
    public abstract Sprite icon { get; }
    public abstract Task<List<ItemWrapper>> GetSongs();
    public static List<MusicPack> GetPacks()
    {
        var packs = new List<MusicPack>();
#if UNITY_EDITOR
        packs.Add(new MusicPackTest());
#endif
#if RELEASE
        //        packs.Add(new MusicPackBIC());

        packs.Add(new MusicPackFree());
        packs.Add(new MusicPackDefault());
        packs.Add(new MusicPackDefault2());
#elif EARLYACCESS
        packs.Add(new MusicPackEarlyAccess());
#endif
        //      packs.Add(new MusicPackGuest());
        //        packs.Add(new MusicPackDLC1());
        packs.Add(new MusicPackGimmick());
        packs.Add(new MusicPackCustom());
        return packs;


    }

}

public class MusicPackEarlyAccess : MusicPack
{
    public override string title => "Default Pack vol.1";
    public override byte packKey => 0xF4;

    public override Sprite icon => Resources.Load<Sprite>("PackIcons/Pack2");
    public override string AchievementId
    {
        get => "pack2";
    }

    public override async Task<List<ItemWrapper>> GetSongs()
    {
        var list = new List<ItemWrapper>
        {
            /*normal*/
            new EmbededItemWrapper("Default/final_canon.pbrff", 2855387457),
            new EmbededItemWrapper("Default/PastelMoonNormal.pbrff", 2925804757),
            new EmbededItemWrapper("Default/tunes_normal.pbrff", 2856184126),
            new EmbededItemWrapper("Default/GameClearNormal.pbrff", 2881836682),
            new EmbededItemWrapper("Default/afterrain_normal.pbrff", 2925819960),
            new EmbededItemWrapper("Default/cafeNormal.pbrff", 2925823057),
            new EmbededItemWrapper("Default/aquaNormal.pbrff", 2868282509),
            new EmbededItemWrapper("Default/SpringBreezeNormal.pbrff", 2877786215),

            /*HARD*/
            new EmbededItemWrapper("Default/final_canon_hard.pbrff", 2791095975),
            new EmbededItemWrapper("Default/PastelMoonHard.pbrff", 2925236066),
            new EmbededItemWrapper("Default/tunes.pbrff", 2807658501),
            new EmbededItemWrapper("Default/GameClearHard.pbrff", 2880836357),
            new EmbededItemWrapper("Default/afterrain_hard.pbrff", 2906442821),
            new EmbededItemWrapper("Default/cafeHard.pbrff", 2925501556),
            new EmbededItemWrapper("Default/aquaHard.pbrff", 2868031988),
            new EmbededItemWrapper("Default/SpringBreezeHard.pbrff", 2873547270),
        };
        return list;
    }
}

public class MusicPackTest : MusicPack
{

    public override string title => "Test Pack";
    public override byte packKey => 0xF0;

    public override string AchievementId
    {
        get => "test";
    }
    public override Sprite icon => Resources.Load<Sprite>("PackIcons/Pack2");

    public override async Task<List<ItemWrapper>> GetSongs()
    {
        var list = new List<ItemWrapper>
        {
            new EmbededItemWrapper("Default/ForTest.pbrff", 0),
        };

        return list;
    }
}


public class MusicPackBIC : MusicPack
{

    public override string title => "BIC Pack";
    public override byte packKey => 0xF0;

    public override string AchievementId
    {
        get => "test";
    }
    public override Sprite icon => Resources.Load<Sprite>("PackIcons/Pack2");

    public override async Task<List<ItemWrapper>> GetSongs()
    {
        var list = new List<ItemWrapper>
        {

            new EmbededItemWrapper("Default/final_canon_easy.pbrff", EASY+1),
            new EmbededItemWrapper("Default/final_canon.pbrff", NORMAL+1),
            new EmbededItemWrapper("Default/final_canon_hard.pbrff", HARD+1),

            new EmbededItemWrapper("Default/PastelMoonEasy.pbrff", EASY +5),
            new EmbededItemWrapper("Default/PastelMoonNormal.pbrff", NORMAL + 5),
            new EmbededItemWrapper("Default/PastelMoonHard.pbrff", HARD + 5),

            new EmbededItemWrapper("Default/AlyciaEasy.pbrff", EASY + 8),
            new EmbededItemWrapper("Default/AlyciaNormal.pbrff",NORMAL+ 8),
            new EmbededItemWrapper("Default/Alycia.pbrff",HARD + 8),

            new EmbededItemWrapper("Default/aquaEasy.pbrff", EASY+ 9),
            new EmbededItemWrapper("Default/aquaNormal.pbrff", NORMAL+9),
            new EmbededItemWrapper("Default/aquaHard.pbrff", HARD+9),

            new EmbededItemWrapper("Default/SiriusNormal.pbrff", NORMAL+16),
            new EmbededItemWrapper("Default/Sirius.pbrff", HARD+16),

            new EmbededItemWrapper("Default/LovelyGlassNormal.pbrff", NORMAL+17),
            new EmbededItemWrapper("Default/LovelyGlass.pbrff", HARD+17),

            new EmbededItemWrapper("Default/Ing2Easy.pbrff", EASY+2, true),
            new EmbededItemWrapper("Default/Ing2Normal.pbrff", NORMAL+2, true),
            new EmbededItemWrapper("Default/Ing2.pbrff", HARD+2, true),

            new EmbededItemWrapper("Default/GameClearEasy.pbrff", EASY+3),
            new EmbededItemWrapper("Default/GameClearNormal.pbrff", NORMAL+3),
            new EmbededItemWrapper("Default/GameClearHard.pbrff", HARD+3),

            new EmbededItemWrapper("Default/afterrain_easy.pbrff", EASY+4),
            new EmbededItemWrapper("Default/afterrain_normal.pbrff", NORMAL+4),
            new EmbededItemWrapper("Default/afterrain_hard.pbrff", HARD + 4),


            new EmbededItemWrapper("Default/ClockEasy.pbrff", EASY + 6),
            new EmbededItemWrapper("Default/ClockNormal.pbrff", NORMAL + 6),
            new EmbededItemWrapper("Default/Clock.pbrff", HARD + 6),


            new EmbededItemWrapper("Default/CaramelEasy.pbrff", EASY+ 7),
            new EmbededItemWrapper("Default/CaramelNormal.pbrff", NORMAL + 7),
            new EmbededItemWrapper("Default/Caramel.pbrff", HARD+ 7),

            new EmbededItemWrapper("Default/StarryEasy.pbrff", EASY+10),
            new EmbededItemWrapper("Default/StarryNormal.pbrff", NORMAL+10),
            new EmbededItemWrapper("Default/Starry.pbrff", HARD+10),

            new EmbededItemWrapper("Default/cafeEasy.pbrff", EASY+11),
            new EmbededItemWrapper("Default/cafeNormal.pbrff", NORMAL+11),
            new EmbededItemWrapper("Default/cafeHard.pbrff", HARD+11),

            new EmbededItemWrapper("Default/UnexpectedEasy.pbrff", EASY+12),
            new EmbededItemWrapper("Default/UnexpectedNormal.pbrff", NORMAL+12),
            new EmbededItemWrapper("Default/Unexpected.pbrff", HARD+12),

            new EmbededItemWrapper("Default/AnhedoniaEasy.pbrff", EASY+13),
            new EmbededItemWrapper("Default/AnhedoniaNormal.pbrff", NORMAL+13),
            new EmbededItemWrapper("Default/Anhedonia.pbrff", HARD+13),


            new EmbededItemWrapper("Default/cherry.pbrff", NORMAL+14),

            new EmbededItemWrapper("Default/cherryHard.pbrff", HARD+14),

            new EmbededItemWrapper("Default/SirenaNormal.pbrff", NORMAL+15),
            new EmbededItemWrapper("Default/Sirena.pbrff", HARD+15),


            new EmbededItemWrapper("Default/tunes_normal.pbrff", NORMAL+18),
            new EmbededItemWrapper("Default/tunes.pbrff", HARD+18),

            new EmbededItemWrapper("Default/SummertimeNormal.pbrff", NORMAL+19),
            new EmbededItemWrapper("Default/Summertime.pbrff", HARD+19),

            new EmbededItemWrapper("Default/BlushNormal.pbrff", NORMAL+20),
            new EmbededItemWrapper("Default/Blush.pbrff", HARD+20),


            new EmbededItemWrapper("Default/SpringBreezeNormal.pbrff", NORMAL+21),
            new EmbededItemWrapper("Default/SpringBreezeHard.pbrff", HARD+21),
            
//            new EmbededItemWrapper("Default/ChronoEasy.pbrff", 2012),
            new EmbededItemWrapper("Default/ChronoEasy.pbrff", NORMAL+22),
            new EmbededItemWrapper("Default/ChronoNormal.pbrff", HARD+22),



            new EmbededItemWrapper("Default/tokkiNormal.pbrff", NORMAL+23),
            new EmbededItemWrapper("Default/tokkiHard.pbrff", HARD+23),

            new EmbededItemWrapper("Default/NeoMatrixNormal.pbrff", NORMAL+24,true),
            new EmbededItemWrapper("Default/NeoMatrix.pbrff", HARD+24,true),

            new EmbededItemWrapper("Default/AbsoluteNormal.pbrff", NORMAL+25),
            new EmbededItemWrapper("Default/Absolute.pbrff", HARD+25),

            new EmbededItemWrapper("Default/ChartreuseNormal.pbrff",NORMAL+26),
            new EmbededItemWrapper("Default/ChartreuseGreenHard.pbrff",HARD+26),
        };

        return list;
    }
}
public class MusicPackFree : MusicPack
{

    public override string title => "Free Pack";
    public override byte packKey => 0xF3;

    public override string AchievementId
    {
        get => "free";
    }
    public override Sprite icon => Resources.Load<Sprite>("PackIcons/Pack2");

    public override async Task<List<ItemWrapper>> GetSongs()
    {
        var list = new List<ItemWrapper>
        {

            new EmbededItemWrapper("Default/final_canon_easy.pbrff", EASY+1),
            new EmbededItemWrapper("Default/final_canon.pbrff", NORMAL+1),
            new EmbededItemWrapper("Default/final_canon_hard.pbrff", HARD+1),

            new EmbededItemWrapper("Default/PastelMoonEasy.pbrff", EASY +5),
            new EmbededItemWrapper("Default/PastelMoonNormal.pbrff", NORMAL + 5),
            new EmbededItemWrapper("Default/PastelMoonHard.pbrff", HARD + 5),

            new EmbededItemWrapper("Default/AlyciaEasy.pbrff", EASY + 8),
            new EmbededItemWrapper("Default/AlyciaNormal.pbrff",NORMAL+ 8),
            new EmbededItemWrapper("Default/Alycia.pbrff",HARD + 8),

            new EmbededItemWrapper("Default/aquaEasy.pbrff", EASY+ 9),
            new EmbededItemWrapper("Default/aquaNormal.pbrff", NORMAL+9),
            new EmbededItemWrapper("Default/aquaHard.pbrff", HARD+9),

            new EmbededItemWrapper("Default/SiriusNormal.pbrff", NORMAL+16),
            new EmbededItemWrapper("Default/Sirius.pbrff", HARD+16),

            new EmbededItemWrapper("Default/LovelyGlassNormal.pbrff", NORMAL+17),
            new EmbededItemWrapper("Default/LovelyGlass.pbrff", HARD+17),


        };

        return list;
    }
}
public class MusicPackDefault : MusicPack
{

    public override string title => "Default Pack vol.1";
    public override byte packKey => 0xF4;

    public override string AchievementId
    {
        get => "pack1";
    }
    public MusicPackDefault() : base(2557340)
    {

    }
    public override Sprite icon => Resources.Load<Sprite>("PackIcons/Pack6");

    public override async Task<List<ItemWrapper>> GetSongs()
    {
        var list = new List<ItemWrapper>
        {
            new EmbededItemWrapper("Default/Ing2Easy.pbrff", EASY+2, true),
            new EmbededItemWrapper("Default/Ing2Normal.pbrff", NORMAL+2, true),
            new EmbededItemWrapper("Default/Ing2.pbrff", HARD+2, true),

            new EmbededItemWrapper("Default/GameClearEasy.pbrff", EASY+3),
            new EmbededItemWrapper("Default/GameClearNormal.pbrff", NORMAL+3),
            new EmbededItemWrapper("Default/GameClearHard.pbrff", HARD+3),

            new EmbededItemWrapper("Default/afterrain_easy.pbrff", EASY+4),
            new EmbededItemWrapper("Default/afterrain_normal.pbrff", NORMAL+4),
            new EmbededItemWrapper("Default/afterrain_hard.pbrff", HARD + 4),


            new EmbededItemWrapper("Default/ClockEasy.pbrff", EASY + 6),
            new EmbededItemWrapper("Default/ClockNormal.pbrff", NORMAL + 6),
            new EmbededItemWrapper("Default/Clock.pbrff", HARD + 6),


            new EmbededItemWrapper("Default/CaramelEasy.pbrff", EASY+ 7),
            new EmbededItemWrapper("Default/CaramelNormal.pbrff", NORMAL + 7),
            new EmbededItemWrapper("Default/Caramel.pbrff", HARD+ 7),

            new EmbededItemWrapper("Default/StarryEasy.pbrff", EASY+10),
            new EmbededItemWrapper("Default/StarryNormal.pbrff", NORMAL+10),
            new EmbededItemWrapper("Default/Starry.pbrff", HARD+10),

            new EmbededItemWrapper("Default/cafeEasy.pbrff", EASY+11),
            new EmbededItemWrapper("Default/cafeNormal.pbrff", NORMAL+11),
            new EmbededItemWrapper("Default/cafeHard.pbrff", HARD+11),

            new EmbededItemWrapper("Default/UnexpectedEasy.pbrff", EASY+12),
            new EmbededItemWrapper("Default/UnexpectedNormal.pbrff", NORMAL+12),
            new EmbededItemWrapper("Default/Unexpected.pbrff", HARD+12),

            new EmbededItemWrapper("Default/AnhedoniaEasy.pbrff", EASY+13),
            new EmbededItemWrapper("Default/AnhedoniaNormal.pbrff", NORMAL+13),
            new EmbededItemWrapper("Default/Anhedonia.pbrff", HARD+13),


            new EmbededItemWrapper("Default/MirrorOfTwilightEasy.pbrff", EASY+28),
            new EmbededItemWrapper("Default/MirrorOfTwilightNormal.pbrff", NORMAL+28),
            new EmbededItemWrapper("Default/MirrorOfTwilight.pbrff", HARD+28),
        };

        return list;
    }
}


public class MusicPackDefault2 : MusicPack
{

    public MusicPackDefault2() : base(2557340)
    {

    }
    public override string title => "Default Pack vol.2";
    public override byte packKey => 0xF4;
    public override string AchievementId => "pack2";
    public override Sprite icon => Resources.Load<Sprite>("PackIcons/Pack5");

    public override async Task<List<ItemWrapper>> GetSongs()
    {
        var list = new List<ItemWrapper>
        {

            new EmbededItemWrapper("Default/cherry.pbrff", NORMAL+14),
            new EmbededItemWrapper("Default/cherryHard.pbrff", HARD+14),

            new EmbededItemWrapper("Default/SirenaNormal.pbrff", NORMAL+15),
            new EmbededItemWrapper("Default/Sirena.pbrff", HARD+15),

            new EmbededItemWrapper("Default/GardenNormal.pbrff",NORMAL+27),
            new EmbededItemWrapper("Default/GardenHard.pbrff",HARD+27),

            new EmbededItemWrapper("Default/tunes_normal.pbrff", NORMAL+18),
            new EmbededItemWrapper("Default/tunes.pbrff", HARD+18),

            new EmbededItemWrapper("Default/SummertimeNormal.pbrff", NORMAL+19),
            new EmbededItemWrapper("Default/Summertime.pbrff", HARD+19),

            new EmbededItemWrapper("Default/BlushNormal.pbrff", NORMAL+20),
            new EmbededItemWrapper("Default/Blush.pbrff", HARD+20),


            new EmbededItemWrapper("Default/SpringBreezeNormal.pbrff", NORMAL+21),
            new EmbededItemWrapper("Default/SpringBreezeHard.pbrff", HARD+21),
            
//            new EmbededItemWrapper("Default/ChronoEasy.pbrff", 2012),
            new EmbededItemWrapper("Default/ChronoEasy.pbrff", NORMAL+22),
            new EmbededItemWrapper("Default/ChronoNormal.pbrff", HARD+22),



            new EmbededItemWrapper("Default/tokkiNormal.pbrff", NORMAL+23),
            new EmbededItemWrapper("Default/tokkiHard.pbrff", HARD+23),

            new EmbededItemWrapper("Default/NeoMatrixNormal.pbrff", NORMAL+24,true),
            new EmbededItemWrapper("Default/NeoMatrix.pbrff", HARD+24,true),

            new EmbededItemWrapper("Default/AbsoluteNormal.pbrff", NORMAL+25),
            new EmbededItemWrapper("Default/Absolute.pbrff", HARD+25),

            new EmbededItemWrapper("Default/ChartreuseNormal.pbrff",NORMAL+26),
            new EmbededItemWrapper("Default/ChartreuseGreenHard.pbrff",HARD+26),




        };
        return list;
    }
}

public class MusicPackDefault3 : MusicPack
{
    public override string title => "Default Pack vol.3";
    public override byte packKey => 0xF4;

    public override Sprite icon => Resources.Load<Sprite>("PackIcons/Pack6");

    public override async Task<List<ItemWrapper>> GetSongs()
    {
        var list = new List<ItemWrapper>
        {
            
            /*normal*/
            new EmbededItemWrapper("Default/tunes_normal.pbrff", 2856184126),
            new EmbededItemWrapper("Default/GameClearNormal.pbrff", 2881836682),
            new EmbededItemWrapper("Default/afterrain_normal.pbrff", 2925819960),
            new EmbededItemWrapper("Default/cafeNormal.pbrff", 2925823057),
//            new EmbededItemWrapper("Default/radiant_normal.pbrff", 2870288097),
            new EmbededItemWrapper("Default/aquaNormal.pbrff", 2868282509),
            new EmbededItemWrapper("Default/SpringBreezeNormal.pbrff", 2877786215),
            new EmbededItemWrapper("Default/SiriusNormal.pbrff", 2005),
//            new EmbededItemWrapper("Default/restedNormal.pbrff", 2925914847),

            /*HARD*/
            new EmbededItemWrapper("Default/tunes.pbrff", 2807658501),
            new EmbededItemWrapper("Default/GameClearHard.pbrff", 2880836357),
            new EmbededItemWrapper("Default/afterrain_hard.pbrff", 2906442821),
            new EmbededItemWrapper("Default/cafeHard.pbrff", 2925501556),
            new EmbededItemWrapper("Default/radiant.pbrff", 2827225779),
            new EmbededItemWrapper("Default/aquaHard.pbrff", 2868031988),
            new EmbededItemWrapper("Default/SpringBreezeHard.pbrff", 2873547270),
            new EmbededItemWrapper("Default/Sirius.pbrff", 2105),
//            new EmbededItemWrapper("Default/rested.pbrff", 2835499419),
        };
        return list;
    }
}

public class MusicPackGuest : MusicPack
{
    public override string title => "Guest Patterners' Pack";
    public override byte packKey => 0xF4;

    public override Sprite icon => Resources.Load<Sprite>("PackIcons/Pack7");

    public override async Task<List<ItemWrapper>> GetSongs()
    {
        var list = new List<ItemWrapper>
        {
            /*normal*/
            new EmbededItemWrapper("Default/CaramelNormal.pbrff", 2001),

        };
        return list;
    }
}

public class MusicPackGimmick : MusicPack
{

    public MusicPackGimmick() : base(2557340)
    {

    }
    public override string title => "Extra Pack";
    public override byte packKey => 0xF4;
    public override Sprite icon => Resources.Load<Sprite>("PackIcons/Pack4");

    public override async Task<List<ItemWrapper>> GetSongs()
    {
        var list = new List<ItemWrapper>
        {

            /*GIMMICK*/
            new EmbededItemWrapper("Default/SpringBreezeGimmik.pbrff", GIMMICK+1),
            new EmbededItemWrapper("Default/Spehs.pbrff", GIMMICK+2,true),

#if RELEASE
            new EmbededItemWrapper("Default/Chrono.pbrff", SPECIAL+1),
            new EmbededItemWrapper("Default/Rested.pbrff", SPECIAL+2),
            new EmbededItemWrapper("Default/NeoMatrixSpecial.pbrff", SPECIAL+3),
            new EmbededItemWrapper("Default/ClockSpecial.pbrff", SPECIAL+4),
            new EmbededItemWrapper("Default/ClockGimmick.pbrff", SPECIAL+5),
            new EmbededItemWrapper("Default/tokkiSpecial.pbrff", SPECIAL+6),

//            new EmbededItemWrapper("Default/Chartreuse.pbrff",3014),

#endif
        };
        return list;
    }
}
public class MusicPackCustom : MusicPack
{
    public override string title => "Workshop Levels";
    public override Sprite icon => Resources.Load<Sprite>("PackIcons/Pack1");
    public override byte packKey => 0x00;

    public override async Task<List<ItemWrapper>> GetSongs()
    {
        var list = await GetSubscribedSteamItems();
        return list.Select((x) => (ItemWrapper)new OnlineItemWrapper(x)).ToList();
    }
    static async Task<List<Steamworks.Ugc.Item>> GetSubscribedSteamItems()
    {
        if (SteamClient.IsValid)
        {
            var items = new List<Steamworks.Ugc.Item>();
            var q = Steamworks.Ugc.Query.ItemsReadyToUse.RankedByTextSearch();
            var page = await q.GetPageAsync(1);
            if (page.HasValue)
            {
                foreach (var i in page.Value.Entries)
                {
                    if (i.IsSubscribed)
                    {
                        items.Add(i);
                    }
                }
                return items;
            }
        }
        return null;
    }

}

public class MusicPackDLC1 : MusicPack
{
    public override string title => "Author Wind Pack";
    public override byte packKey => 0xF1;

    public override Sprite icon => Resources.Load<Sprite>("PackIcons/Pack3");
    public MusicPackDLC1() : base(2320420)
    {

    }
    public override async Task<List<ItemWrapper>> GetSongs()
    {
        var list = new List<ItemWrapper>();
        list.Add(new EmbededItemWrapper("Wind/Allday.pbrff", 1001, true));
        list.Add(new EmbededItemWrapper("Wind/Inyourdream.pbrff", 1002));
        list.Add(new EmbededItemWrapper("Wind/PainterHard.pbrff", 1003));

        list.Add(new EmbededItemWrapper("Wind/InyourdreamNormal.pbrff", 1004));
        list.Add(new EmbededItemWrapper("Wind/PainterNormal.pbrff", 1005));
        return list;
    }
}