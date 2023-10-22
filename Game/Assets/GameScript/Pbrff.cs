using UnityEngine;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using UnityEngine.Profiling;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using System.Collections.Generic;
using System.CodeDom;
using NAudio.Wave;

public class PbrffExtracter 
{
    ZipUtility zip;
    FullEntry loaded; // 현재 상태값. 부분적으로 로딩될 수 있음
    TableMetaData meta;
    public bool canSave = true;
    bool isPreviewChanged = false;
    string OggPath => Application.persistentDataPath + "/CompressedAudio.ogg";
    string PreviewPath => Application.persistentDataPath + "/CompressedPreview.ogg";
    string TempPreviewPath => Application.persistentDataPath + "/RawPreview.wav";
    byte packkey;
    public PbrffExtracter(byte packkey)
    {
        this.packkey = packkey;
        zip = new ZipUtility();
        loaded = new FullEntry();
    }
    public void SetKey(byte key)
    {
        packkey = key;
    }
    public FullEntry GetFullData()
    {
        return loaded;
    }
    public PreviewEntry GetPreviewData(ItemWrapper item)
    {
        if (loaded.preview == null)
        {
            return null;
        }
        else
        {
            return new PreviewEntry(item, loaded.icon, loaded.preview.GetAudio(), meta, item.isOriginal);
        }
    }
    Table LoadTable(byte packkey)
    {
        Table table = new Table();
        byte[] bytes = zip.ReadEntry("table.json");
        loaded.hash = Utility.GetHash(bytes);
        Debug.Log("HASH:" + loaded.hash);

        Utility.XOR(ref bytes, packkey);

        table.loadWithJson(Encoding.Default.GetString(bytes));
        loaded.table = table;
        return table;
    }
    async Task CompressAudio(string path)
    {
        try
        {
            if (File.Exists(OggPath)) File.Delete(OggPath);
            await OggEncoder.Encode(path, OggPath);
            byte[] bytes = File.ReadAllBytes(OggPath);
            loaded.audio.LoadAudio(bytes);
            loaded.table.format_code = 2;
            Game.table.format_code = 2;
            Debug.Log("Compress Finished");
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }
    async Task CompressPreview()
    {
        if (File.Exists(PreviewPath)) File.Delete(PreviewPath);
        await OggEncoder.Encode(TempPreviewPath, PreviewPath);
        byte[] bytes = File.ReadAllBytes(PreviewPath);
        loaded.preview.LoadAudio(bytes);
    }
    public void SetAudioFile(string audio_path)
    {
        int code = FormatCode(audio_path);
        byte[] loaded_audio = File.ReadAllBytes(audio_path);
        loaded.audio.LoadAudio(loaded_audio);
        RefreshPreview(audio_path);

        if (code == 0)
        {
            Debug.Log("Start Compression");
            var audioTask = CompressAudio(audio_path);
        }


        if (loaded.table == null) loaded.table = new Table();
        loaded.table.format_code = code;
        Game.table.format_code = code;
    }
    private async Task RefreshPreview(string source)
    {
        using (MediaFoundationReader reader = new MediaFoundationReader(source))
        {
            var provider = reader.ToSampleProvider().Skip(reader.TotalTime / 2).Take(new TimeSpan(0, 0, 10));
            WaveFileWriter.CreateWaveFile16(TempPreviewPath, provider);
        }
        await CompressPreview();
        var bytes = File.ReadAllBytes(PreviewPath);
        loaded.preview.LoadAudio(bytes);
        isPreviewChanged = true;
    }

    AudioData LoadFullMusic()
    {
        byte[] bytes = zip.ReadEntry("audio");
        loaded.audio.LoadEncryptedAudio(bytes);
        var pcm = loaded.audio.GetAudio();
        isPreviewChanged = false;
        return pcm;
    }
    static Dictionary<(string, string), AudioAdapter> cache = new Dictionary<(string, string), AudioAdapter>();
    public static void ReleaseCache()
    {
        foreach(var i in cache)
        {
            i.Value.GetAudio().Release();
        }
        cache.Clear();
    }
    AudioData LoadPreview()
    {
        if (zip.FindEntry("preview") == null)
        {
            Debug.Log("Preview doesn't exist");
            return null;
        }
        else
        {
            Debug.Log("Preview:" + loaded.table.title);
            var key = (meta.title,meta.composer);
            if (cache.ContainsKey(key) )
            {
                Debug.Log("load preview by Cache");
                loaded.preview = cache[key];
            }
            else
            {
                Debug.Log($"load preview by Origin: {key} {cache.Count}");
                byte[] bytes = zip.ReadEntry("preview");
                loaded.preview.LoadEncryptedAudio(bytes);
                cache.Add(key, loaded.preview);
            }
            return loaded.preview.GetAudio();
        }
    }

    AudioData LoadPreviewWithoutCache()
    {
        if (zip.FindEntry("preview") == null)
        {
            Debug.Log("Preview doesn't exist");
            return null;
        }
        else
        {
            byte[] bytes = zip.ReadEntry("preview");
            loaded.preview.LoadEncryptedAudio(bytes);
            return loaded.preview.GetAudio();
        }
    }
    void LoadBGA(string datapath)
    {
        string name = zip.FindEntry("bga");
        try
        {
            File.WriteAllBytes(datapath + "/" + name, zip.ReadEntry(name));
            loaded.video_url = datapath + "/" + name;
        }
        catch (Exception) { };
    }

    public bool isProtect(string url)
    {
        using (var protect = ZipFile.Open(url,ZipArchiveMode.Read))
        {
            using (var entry = protect.GetEntry("table.json").Open())
            {
                var c= entry.ReadByte();
                return c != '{';
            }
        }
    }

    void LoadKeyPallete()
    {
        string fileName = "Pallete.zip";
        KeySoundPallete.Setup();
        byte[] bytes = zip.ReadEntry(fileName);
        if (File.Exists(Application.persistentDataPath + "/" + fileName)) File.Delete(Application.persistentDataPath + "/" + fileName);
        File.WriteAllBytes(Application.persistentDataPath + "/"+ fileName, bytes);
        KeySoundPallete.Deserialize(Application.persistentDataPath + "/" + fileName);
    }
    void LoadIcon() // 왜 굳이 이러느냐? 텍스쳐 로딩은 비동기처리가 안됨
    {

        loaded.icon = new SpriteWrapper(zip.ReadEntry("icon.png"));
    }
    public void LoadPreviewData(string url)
    {
        zip.SetPath(url);
        if (zip.FindEntry("meta.json") == null)
        {
            Debug.Log("No meta data:" + url);
            LoadTable(packkey);
            meta = new TableMetaData();
            meta.Load(loaded.table);
        }
        else
        {
            LoadMetaData();
        }
        LoadPreview();
        LoadIcon();
    }
    public void LoadMetaData()
    {
        byte[] bytes = zip.ReadEntry("meta.json");
        var table = JsonUtility.FromJson<TableMetaData>(Encoding.Default.GetString(bytes));
        meta = table;
        Debug.Log("check:"+meta.hash);
    }
    public void FetchFrom(FullEntry data)
    {
        loaded = data;
    }
    public FullEntry LoadFull(string url)
    {
        zip.SetPath(url);
        loaded = new FullEntry();
        LoadTable(packkey);
        LoadFullMusic();
        LoadIcon();
        LoadBGA(Application.dataPath);
        LoadKeyPallete(); // 이 세가지는 여전히 로컬 상태가 아님...
        LoadPreviewWithoutCache();
        return loaded;
    }
    public Task<FullEntry> LoadFullAsync(string url)
    {
        zip.SetPath(url);
        var datapath = Application.dataPath;
            LoadKeyPallete();
        return Task.Run(() => {
            loaded = new FullEntry();
            LoadTable(packkey);
            LoadFullMusic();
            LoadBGA(datapath); //
            LoadPreviewWithoutCache();
            return loaded;
        });
    }
    void SaveBGA()
    {
        Debug.Log(Path.GetExtension(Game.pbrffdata.video_url));
        if(loaded.video_url !=null)
        zip.WriteEntry("bga"+Path.GetExtension(Game.pbrffdata.video_url), Game.pbrffdata.video_url);
    }
    async void SaveFullAudio()
    {
        zip.WriteEntry("audio", loaded.audio.GetEncryptedBytes());
    }
    async void SavePreview()
    {
        if (isPreviewChanged)
        {
            zip.WriteEntry("preview", loaded.preview.GetEncryptedBytes());
        }
        else
        {
            zip.WriteEntry("preview", loaded.preview.GetEncryptedBytes());
        }
        isPreviewChanged = false;
    }
    void SaveTable()
    {
        string json = loaded.table.ToJson();
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        zip.WriteEntry("table.json", bytes);
        loaded.hash = Utility.GetHash(bytes);

    }
    void SaveKeyPallete()
    {
        string zipurl = KeySoundPallete.Serialize();
        zip.WriteEntry("Pallete.zip",zipurl);
    }
    void SaveIcon()
    {
        zip.WriteEntry("icon.png",loaded.icon.bytes);
    }
    public void SaveNowState(string url)
    {
        if (!canSave) return;
        zip.SetPath(url);
        SaveKeyPallete();
        SaveIcon();
        SaveBGA();
        SaveTable();
        SaveMetaData();
        SaveFullAudio();
        SavePreview();
        //        Game.table.CleanDirty();
    }

    private void SaveMetaData()
    {
        var meta = new TableMetaData();
        meta.Load(loaded.table);
        meta.hash = loaded.hash;
        byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(meta));
        zip.WriteEntry("meta.json", bytes);
    }


    private int FormatCode(string path)
    {
        int code = -1;
        foreach (var i in new (int, string)[] { (0, ".wav"), (1, ".mp3"), (2, ".ogg") })
            if (path.EndsWith(i.Item2))
            {
                code = i.Item1;
            }// 포맷을 인식해서 기록
        return code;
    }

    public class PreviewEntry
    {
        public SpriteWrapper icon;
        public ItemWrapper item;
        public AudioData preview;
        public string title;
        public string composer;
        public int level;
        public string leveler;
        public bool isOriginal = false;
        public string composerLink;
        public string difficulty;
        public string description;
        public string hash;

        public PreviewEntry(ItemWrapper item, SpriteWrapper icon, AudioData audio, TableMetaData metadata, bool isOfficial = false)
        {
            this.item = item;
            this.icon= icon;
            this.preview = audio;
            title = metadata.title;
            composer = metadata.composer;
            level = metadata.level;
            leveler = metadata.leveler;
            this.isOriginal = isOfficial;
            composerLink = metadata.composer_link;
            difficulty = metadata.difficulty;
            description = metadata.description;
            hash = metadata.hash;
        }
        public void ApplyPcm(AudioClip clip)
        {
            clip.SetData(preview.GetPCM().Samples, 0);

        }

    }

    public class FullEntry
    {
        public string url;
        public Table table = new Table();
        public AudioAdapter audio = new AudioAdapter();
        public AudioAdapter preview = new AudioAdapter();
        public string video_url;
        public string hash;

        SpriteWrapper _icon;
        public SpriteWrapper icon
        {
            get => _icon??PreviewIcon.DefaultSprite;
            set 
            { 
                _icon = value; 
            }
        }

    }
}

public class SpriteWrapper
{
    byte[] _bytes;
    Vector2 pivot;
    public byte[] bytes 
    {
        get => _bytes;
    }
    public SpriteWrapper(byte[] bytes)
    {
        pivot = Vector2.zero;
        _bytes = bytes;

    }
    public SpriteWrapper(byte[] bytes,Vector2 pivot)
    {
        this.pivot = pivot;
        _bytes = bytes;
    }
    public static implicit operator Sprite(SpriteWrapper r)
    {
        Texture2D texture = new Texture2D(512, 512,TextureFormat.RGBA32,false);
        texture.LoadImage(r.bytes);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), r.pivot);
        return sprite;
    }
    public static implicit operator SpriteWrapper(Sprite r)
    {
        Texture2D texture = r.texture;
        if (texture.width > 512 || texture.height > 512)
        {
            texture = Utility.ScaleTexture(texture, 512, 512);
        }
        SpriteWrapper wrap = new SpriteWrapper(texture.EncodeToJPG());
        return wrap;
    }
    public byte[] ToJPG()
    {
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        if(texture.width>512 || texture.height > 512)
        {
            Debug.Log("downScale");
            texture = Utility.ScaleTexture(texture, 512, 512);
        }
        return texture.EncodeToJPG();
    }
}