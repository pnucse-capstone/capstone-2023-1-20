using SFB;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using Newtonsoft.Json;
using System.Linq;

public class KeySoundPanel : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Text log;
    List<NoteData> select;

    [SerializeField] Toggle toggle_loop;

    [SerializeField] InputField input_start;
    [SerializeField] InputField input_finish;
    void Start()
    {
    }
    public void Open(List<NoteData> select)
    {
        NoteEditor.PopupLock = true;
        this.select = select.ToList();
        if(select.Count == 1)
        {
            GetComponentInChildren<KeyGrid>().Select(KeySoundPallete.GetSoundInfo(select[0].key));
        }
        else
        {
            GetComponentInChildren<KeyGrid>().Select(null);

        }
        gameObject.SetActive(true);
    }
    public void Close()
    {
        NoteEditor.PopupLock = false;
        
        foreach(var i in select)
        {
            int key = GetComponentInChildren<KeyGrid>().now_id;
            if (key!=-1)
            {
                i.key = key;
            }
        }
        this.select = null;
        gameObject.SetActive(false);
    }
    ExtensionFilter[] audioFilter = new ExtensionFilter[] { new ExtensionFilter("Sound file", new string[] { "wav" }) };
    public void KeyAdd()
    {
        string[] paths;
        paths = StandaloneFileBrowser.OpenFilePanel("audio file", "",audioFilter, false);
        if (paths.Length != 0)
        {
            string url = paths[0];
            if(new FileInfo(url).Length > 500 * 1024)
            {
                log.text = "Error: File size is over 500KB";
                return;
            }
            KeySoundPallete.Add(url, Path.GetFileName(url));
        }
        log.text = "";
    }
    public void KeyRemove()
    {
        KeySoundPallete.Remove(GetComponentInChildren<KeyGrid>().now_id);
    }
    public void KeyRename(string value)
    {
        KeySoundPallete.SetName(GetComponentInChildren<KeyGrid>().now_id,value);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !NoteEditor.PopupLock)
        {
            Close();
        }
    }
}
public static class KeySoundPallete
{
    public static string DefaultHitSoundPath => Path.Combine(Application.streamingAssetsPath,"Hitsound",$"{PlayerPrefs.GetString("hitsoundtype","tamb")}.wav");

    static FMOD.Sound default_snd;    // Start is called before the first frame update
    public static SoundInfo default_info = new SoundInfo(0, "default");
    static Dictionary<int,FMOD.Sound> snd;
    static bool isReady = false;
    public static void Setup()
    {
        if (snd == null)
        {
            snd = new Dictionary<int, FMOD.Sound>();
        }
        if (!isReady)
        {
            isReady = true;
            FMODWrapper.GetSystem().createSound(DefaultHitSoundPath, FMOD.MODE.CREATESAMPLE, out default_snd);
            if(!snd.ContainsKey(0))
                snd.Add(0, default_snd);
        }
        else
        {
            Release();
            Setup();
        }
        //다른 사운드는 추가될때마다 LoadURL쪽으로 ㄱㄱ
    }
    public static FMOD.Channel FMODPlay(int id)
    {

        var info = dic.Find((x) => x.id == id);
        if (info != null)
        {
            snd[id].setMode(FMOD.MODE.LOOP_OFF);
            snd[id].setLoopCount(0);
            return FMODWrapper.PlayKeySound(snd[id]);
          
        }
        else 
        {
            return FMODWrapper.PlayKeySound(snd[id]);
        }
    }
    public static void Release()
    {
        if (isReady)
        {
            isReady = false;

            foreach (var i in snd)
            {
                i.Value.release();
            }   
            snd = null;
            
            default_snd.release();
        }
    }
    public static List<SoundInfo> dic = new List<SoundInfo>(); // id, 이름 쌍
    public static int Count
    {
        get => dic.Count;
    }
    public static SoundInfo GetSoundInfo(int id)
    {
        var temp = dic.Find((x) => x.id == id);
        if (temp == null) return default_info;
        return temp;
    }

    public static int Add(string url,string name) // 해당 위치의 사운드를 팔레트에 추가. id를 반환
    {
        int id = 10000;//그냥 0에서 시작하면 인덱스랑 헷갈리니 10000에서 시작하자
        while (dic.Exists((x) => x.id == id)) id++;

        File.Copy(url, FormatSoundPath(id), true);
        Load(new SoundInfo(id,name));

        return id;
    }

    private static string FormatSoundPath(int id)
    {
        return Application.persistentDataPath + "/Sound" + id + ".wav";
    }

    public static void SetName(int id,string name)
    {
        if (dic.Exists((x) => x.id== id)) 
        {
            dic.Find((x) => x.id == id).name = name;
        };
    }
    static void Load(SoundInfo info)
    {
        Debug.Log("Sound Load:" + info);
        // 오디오 클립 생성
        LoadSound(info);
        dic.Add(info);

    }

    private static void LoadSound(SoundInfo info)
    {
        Debug.Log("LoadSound");
        if (snd == null) snd = new Dictionary<int, FMOD.Sound>();

        if (snd.ContainsKey(info.id))
        {
            snd[info.id].release();
            snd.Remove(info.id);
        }

        var temp = FMODWrapper.CreateKeySound(FormatSoundPath(info.id), info);
        snd.Add(info.id, temp);

    }
    public static void Remove(int id)
    {
        dic.RemoveAll((x)=>x.id==id);
    }
    public static void Deserialize(string url) // 팔레트가 저장된 파일에서 팔레트를 불러옴
    {
        Debug.Log("Deserialize");
        dic = new List<SoundInfo>();
        string info_url = "";
        using (var zip = ZipFile.Open(url,ZipArchiveMode.Read))
        {
            foreach(var i in zip.Entries)
            {
                i.ExtractToFile(Application.persistentDataPath + "/"+i.Name,true);
                if (i.Name == "info.txt")info_url = Application.persistentDataPath + "/info.txt";
            }
        }
        if (info_url == "") return;

        byte[] bytes = File.ReadAllBytes(info_url);
        string d = System.Text.Encoding.UTF8.GetString(bytes);
        var a = JsonConvert.DeserializeObject<SoundInfoFile>(d);
        foreach(var i in a.list)
        {
            Load(i);
        }
    }
    public static string Serialize() // 현재 팔레트값을 파일 바이너리로 변환
    {
        string zipurl = Application.persistentDataPath + "/PalleteTemp.zip";
        if (File.Exists(zipurl)) File.Delete(zipurl);

        using(var zip = ZipFile.Open(zipurl,ZipArchiveMode.Update))
        {
            foreach (var i in dic)
            {
                zip.CreateEntryFromFile(FormatSoundPath(i.id),"Sound"+i.id+".wav");
            }
            var entry = zip.CreateEntry("info.txt");
            var file = new SoundInfoFile();
            file.list = dic;
            string data = JsonConvert.SerializeObject(file, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
            using (var info = zip.GetEntry("info.txt").Open())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                info.Write(bytes,0,bytes.Length);
            }

        }
        return zipurl;
    }
    
}
[Serializable]
public class SoundInfoFile
{
    public List<SoundInfo> list;
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
    }
}

[Serializable]
public class SoundInfo
{
    public SoundInfo(int id, string name)
    {
        this.id = id;
        this.name = name;
    }
    public int id;
    public string name;

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
    }
}