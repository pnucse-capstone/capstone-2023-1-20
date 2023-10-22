using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Linq;
using SFB;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using NAudio.Wave;

public static class Utility
{
    public struct XORInt32
    {
        int _value;
        int _salt;

        XORInt32(int value)
        {
            _salt = RandomNumberGenerator.GetInt32(int.MaxValue);
            _value = value ^ _salt;
        }

        public int GetValue()
        {
            return (int)(_value ^ _salt);
        }

        public void SetSalt()
        {
            _salt = RandomNumberGenerator.GetInt32(int.MaxValue);
            _value = GetValue() ^ _salt;
        }

        public static implicit operator int(XORInt32 value) => value.GetValue();

        public static implicit operator XORInt32(int value) => new XORInt32(value);
    }
    public static string RemoveSizeTag(string input)
    {
        string output = input;
        Regex regex = new Regex(@"<size=[0-9]*>(.*)</size>");

        var match = regex.Match(input);
        if (match.Success)
        {
            output = regex.Replace(input, match.Groups[1].Value);
        }
        return output;
    }
    public const ulong DevId = 76561198195002738;
    public static void XOR(ref byte[] bytes,byte key)
    {
        for(int i = 0; i < bytes.Length; i++)
        {
            bytes[i] ^= key;
        }
    }

     public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);

        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }

        result.Apply();
        return result;
    }
    public static string GetHash(byte[] bytes)
    {
        var hash = SHA512.Create();
        var data = hash.ComputeHash(bytes);
        return BitConverter.ToString(data).Replace("-", "");
    }
    public static string GetHash(string str)
    {
        byte[] bytes = Encoding.Default.GetBytes(str);
        return GetHash(bytes);
    }

    public static Color DefaultColor(NoteData data)
    {
        float hue = data.x * (1 / 6F);
        hue += data.y / 12F;
        Color c = Color.HSVToRGB(hue, 0.6F, 1 - data.x / 10F);
        return c;
    }
    public static byte GetKey(string url)
    {
        var zip = new ZipUtility(url);
        var bytes = zip.ReadEntry("table.json");
        byte key = (byte)(bytes[0] ^ '{');
        return key;
    }
#if UNITY_EDITOR

    [MenuItem("DevMacro/unlock achievement test")]
    static void unlockAchievement()
    {
        var ach = new Steamworks.Data.Achievement("pack1clear");
        try
        {
            Debug.Log("Unlock");
            ach.Trigger();
        }
        catch (System.Exception)
        {
            Debug.Log("Achievement ERROR");
        }
    }
    [MenuItem("DevMacro/lock achievement test")]
    static void lockAchievement()
    {

        var ach = new Steamworks.Data.Achievement("pack1clear");
        try
        {
            Debug.Log("Unlock");
            ach.Clear();
        }
        catch (System.Exception)
        {
            Debug.Log("Achievement ERROR");
        }
    }

    [MenuItem("DevMacro/OneTouch")] // 릴리즈 전에 해야하는거
    static void OneTouch()
    {
        PackXOR();
        PacksResave();
        HashRefresh();
    }
    [MenuItem("DevMacro/Packs XOR Encode")] // 전체 팩의 곡들을 다 지정된 키로 암호화함
    async static void PackXOR()
    {
        var list = MusicPack.GetPacks();
        foreach (var i in list)
        {
            if (!(i is MusicPackCustom))
            {
                List<ItemWrapper> songs = await i.GetSongs();
                foreach (var song in songs)
                {
                    var zip = new ZipUtility(song.Directory);
                    var bytes = zip.ReadEntry("table.json");
                    byte prekey = (byte)('{' ^ bytes[0]);
                    XOR(ref bytes, (byte)(prekey ^ i.packKey));
                    zip.WriteEntry("table.json", bytes);
                }
            }
        }
    }
    [MenuItem("DevMacro/Packs XOR Decode")] // 전체 팩의 곡들을 다 지정된 키로 암호화함
    async static void PackXORDecode()
    {
        var list = MusicPack.GetPacks();
        foreach (var i in list)
        {
            if (!(i is MusicPackCustom))
            {
                List<ItemWrapper> songs = await i.GetSongs();
                foreach (var song in songs)
                {
                    var zip = new ZipUtility(song.Directory);
                    var bytes = zip.ReadEntry("table.json");
                    byte prekey = (byte)('{' ^ bytes[0]);
                    XOR(ref bytes, prekey);
                    zip.WriteEntry("table.json", bytes);
                }
            }
        }
    }

    [MenuItem("DevMacro/Packs Recalc Metadata Hash")] //
    async static void PacksResave()
    {
        var list = MusicPack.GetPacks();
        foreach (var i in list)
        {
            if (!(i is MusicPackCustom))
            {
                List<ItemWrapper> songs = await i.GetSongs();
                foreach (var song in songs)
                {
                    var zip = new ZipUtility(song.Directory);
                    var bytes2 = zip.ReadEntry("table.json");
                    var hash = GetHash(bytes2);


                    var meta = new ZipUtility(song.Directory);
                    var bytes1 = meta.ReadEntry("meta.json");
                    var str = Encoding.UTF8.GetString(bytes1);
                    var jobj = JObject.Parse(str);

                    jobj["hash"] = hash;
                    Debug.Log(jobj.ToString());
                    var meta2 = Encoding.UTF8.GetBytes(jobj.ToString());
                    meta.WriteEntry("meta.json",meta2);

                }
            }
        }
    }
    [MenuItem("Test/tEST mACRO")]
    public static void ScorePostTest()
    {
    }

    [MenuItem("Test/Attach Ach")]
    async static void packClearAchievement()
    {
        var list = await PackSelectUI.pack.GetSongs();
        var records = PooboolServerRequest.instance.userRecords;
        foreach (var g in list.GroupBy(x => MusicPack.GetSonginfo(x.Id).meta.title + MusicPack.GetSonginfo(x.Id).meta.composer))
        {
            var clear = g.ToList().FindAll(x => records.Exists(y => x.Id == y.songcode && y.isClear()));
            Debug.Log("group:" + (g.Key, g.Count(),clear.Count()));
            if (clear.Count() == 0)
            {
                Debug.Log("조건 불만족");
                return;
            }
        }
        var ach = new Steamworks.Data.Achievement(PackSelectUI.pack.AchievementId + "clear");
        try
        {
            Debug.Log("Unlock");
            ach.Trigger();
        }
        catch (System.Exception)
        {
            Debug.Log("Achievement ERROR");
        }
        //        var isPackClear = list.All((x) => records.Exists((y) => y.songcode == x.Id && y.isClear()));
    }

    [MenuItem("DevMacro/File Hash Reregister")] // 기본 채보의 파일 해시값을 재등록
    public static async void HashRefresh()
    {
        var list = MusicPack.GetPacks();
        HashSet<ulong> duplicateCheck = new HashSet<ulong>(); 
        foreach (var i in list)
        {
            if (!(i is MusicPackCustom))
            {
                List<ItemWrapper> songs = await i.GetSongs();
                foreach (var song in songs)
                {
                    var zip = new ZipUtility(song.Directory);
                    var bytes = zip.ReadEntry("table.json");

                    var hash = GetHash(bytes);
                    
                    try
                    {
                        if (duplicateCheck.Contains(song.Id))
                        {
                            Debug.LogError($"중복된 SONG ID [{song.Id}] 가 존재합니다. ");
                        }
                        else
                        {
                            duplicateCheck.Add(song.Id);
                        }
                        Debug.Log($"RegisterHash:{song.Id},{hash}");
                        PooboolServerRequest.instance.PostHash(song.Id, hash);
                    }
                    catch (Exception ex) { Debug.LogError(ex); }
                }

            }
        }

    }

    [MenuItem("DevMacro/Custom Macro")]
    public static void CustomMacro()
    {
        Game.table.getAllNotes(x => true).ForEach(x => { x.data.y = 0;x.data.x = 5; });
    }
    [MenuItem("DevMacro/Reset Speed")]
    public static void ResetSpeed()
    {
        Game.table.getEvents().ForEach((x) => { x.speed2 = 1;x.Use("speed2", false); });
    }
    static ExtensionFilter[] jsonFilter = new ExtensionFilter[] { new ExtensionFilter("json file", new string[] { "json" }) };

    [MenuItem("DevMacro/Load From MIRAY")] // MIRAY 채보 불러오기
    public static async void MIRAY()
    {
        string[] paths;
        paths = StandaloneFileBrowser.OpenFilePanel("Please select mp3 file.", "", jsonFilter, false);
        if (paths.Length != 0)
        {

            string url = paths[0];
            byte[] bytes = File.ReadAllBytes(url);
            string json = Encoding.UTF8.GetString(bytes);
            var miray = JsonUtility.FromJson<MirayFile>(json);
            foreach(var i in miray.Notes)
            {
                var note = new Note(i);
                note.data.y = Mathf.Clamp(Mathf.RoundToInt(i.y),0,5);
                Game.table.addNote(note);
            }
            foreach (var i in miray.Events)
            {
                EventData e = new EventData();
                e.time = i.time;
                e.bgColor = new ColorAdapter(i.colorA[0], i.colorA[1], i.colorA[2]);
                e.bgColor2 = new ColorAdapter(i.colorA[0], i.colorA[1], i.colorA[2]);
                e.lineColors[0] = e.lineColors[1] = e.lineColors[2] = e.lineColors[3] = e.lineColors[4] = e.lineColors[5] 
                    = new ColorAdapter(i.colorB[0], i.colorB[1], i.colorB[2]);
                e.interpole = i.interpole;
                e.speed2 = i.speed;
                e.Use("speed2", true);
                e.Use("bgColor", true);
                e.Use("bgColor2", true);
                e.Use("lineColors", true);
                Game.table.addEvent(e);
            }
        }
    }

    [Serializable]
    public class MirayFile 
    {
        public List<NoteData> Notes;
        public List<MirayEvent> Events;
        [Serializable]
        public class MirayEvent
        {
            public float time;
            public float[] colorA;
            public float[] colorB;
            public int interpole;
            public float speed;
        }
    }

    [MenuItem("DevMacro/Auto Visibility")]
    public static void Dup() // 다중노트 가시성
    {
        var notes = Game.table.getAllNotes((x) => true);
        foreach(var i in notes)
        {
            bool s = notes.Exists((x) => Mathf.Abs(i.data.time - x.data.time)<0.001F&& i != x && x.data.x<i.data.x&& isLineHit(i.data.y, i.data.y + i.data.x, x.data.y, x.data.y + x.data.x));
            if (s)
            {
                i.data.invert = 0.4F;
                i.data.scaley = 0.8F;
            }
        }
    }

    [MenuItem("DevMacro/Color Reset")]
    public static void ColorReset() // 다중노트 가시성
    {
        var notes = Game.table.getAllNotes((x) => true);
        foreach (var i in notes)
        {
            i.data.color = DefaultColor(i.data);
            i.data.lineColorStart = DefaultLineColorStart(i.data);
            i.data.lineColorEnd = DefaultLineColorEnd(i.data);

        }
    }
    [MenuItem("DevMacro/Randomize Notes")]
    public static void Rand() // 다중노트 가시성
    {
        new MODIRandom().Modify(Game.table);
    }

    [MenuItem("DevMacro/Background darken")]
    public static void bgDark() // 다중노트 가시성
    {
        Game.table.Apply((EventData x) =>
        {
            float h, s, v;
            Color.RGBToHSV(x.bgColor, out h, out s, out v);
            x.bgColor = Color.HSVToRGB(h, s, v/ 1.5F);
            Color.RGBToHSV(x.bgColor2, out h, out s, out v);
            x.bgColor2 = Color.HSVToRGB(h, s, v / 1.5F);

        });
    }
    [MenuItem("DevMacro/Background Brighten")]
    public static void bgBright() // 다중노트 가시성
    {
        Game.table.Apply((EventData x) =>
        {
            float h, s, v;
            Color.RGBToHSV(x.bgColor, out h, out s, out v);
            x.bgColor = Color.HSVToRGB(h, s, v * 1.5F);
            Color.RGBToHSV(x.bgColor2, out h, out s, out v);
            x.bgColor2 = Color.HSVToRGB(h, s, v * 1.5F);

        });
    }

    [MenuItem("DevMacro/Line Color Brighten")]
    public static void linesBright() // 다중노트 가시성
    {
        Game.table.Apply((EventData x) =>
        {
            float h, s, v;
            for(int i = 0; i < 6; i++)
            {
                Color.RGBToHSV(x.lineColors[i], out h, out s, out v);
                x.lineColors[i] = Color.HSVToRGB(h, s, v * 1.5f);
            }

        });
    }

    [MenuItem("DevMacro/Line Color Darken")]
    public static void linesDark() // 다중노트 가시성
    {
        Game.table.Apply((EventData x) =>
        {
            float h, s, v;
            for (int i = 0; i < 6; i++)
            {
                Color.RGBToHSV(x.lineColors[i], out h, out s, out v);
                x.lineColors[i] = Color.HSVToRGB(h, s, v / 1.5F);
            }

        });
    }
    [MenuItem("DevMacro/Import Excel")]
    public static void ReadExcel()
    {
    }
#endif
    public static bool isLineHit(float s1,float e1,float s2, float e2)
    {
        return !(e1 < s2 || e2 < s1);
    }
    public static Color DefaultLineColorStart(NoteData data)
    {
        float hue = Mathf.Repeat(1F - data.x * (1 / 6F), 1F);
        float V = 1F;
        if (data.y % 2 == 0) hue = Mathf.Repeat(hue + 1 / 12F, 1);
        Color c = Color.HSVToRGB(hue, Mathf.Clamp(1F - data.length / 16F, 0, 1), V);
        c.a = 1F;
        return c;
    }
    public static Color DefaultLineColorEnd(NoteData data)
    {
        float hue = Mathf.Repeat(1F - data.x * (1 / 6F), 1F);
        float V = 0.7F;
        if (data.y % 2 == 0) hue = Mathf.Repeat(hue + 1 / 12F, 1);
        hue = Mathf.Repeat(hue - 1 / 3F, 1);
        Color c = Color.HSVToRGB(hue, Mathf.Clamp(1F - data.length / 16F, 0, 1), V);
        c.a = 1F;
        return c;
    }
}
