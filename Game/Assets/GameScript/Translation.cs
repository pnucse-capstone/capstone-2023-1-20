using ExcelDataReader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
public static class Translation
{
    public static string path = Path.Combine(Application.dataPath,"StreamingAssets/lang.json");
    static TranslationData translate;
    public static Charote.Emotion GetEmotion(string id)
    {
        id = id.Trim().ToLower();

        switch (translate.GetEmotion(id).ToLower().Trim())
        {
            case "smile": return Charote.Emotion.SMILE;
            case "idle": return Charote.Emotion.IDLE;
            case "sad": return Charote.Emotion.SAD;
            case "sleep": return Charote.Emotion.SLEEP;
            case "wake": return Charote.Emotion.WAKE;
            default: return Charote.Emotion.IDLE;
        }
    }
    public static string GetDialog(string id)
    {
        id = id.Trim().ToLower();
        return translate.GetDialog(id);
    }
    static Translation()
    {
        var text = File.ReadAllText(path);
        translate= JsonConvert.DeserializeObject<TranslationData>(text);
    }
    public static void ChangeLanguage(string lang)
    {
        Debug.Log("Change language:" + lang);
        translate.SetCurrentLanguage(lang);
    }
    public static List<string> GetLanguages()
    {
        return translate.GetLanguages();
    }
    public static string GetCurrentLanguage()
    {
        return translate.GetLanguageNow();
    }

    public static string GetUIText(string id)
    {
        id = id.Trim().ToLower();
        return translate.GetUIText(id);
    }
    // Start is called before the first frame update
}
#if UNITY_EDITOR
public class MyAssetPostprocessor : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (var i in importedAssets)
            if (Path.GetDirectoryName(i).EndsWith("lang"))
            {
                Debug.Log(i);
                if (Path.GetFileName(i).EndsWith("lang.xlsx"))
                {
                    var copypath = Path.Combine(Path.GetDirectoryName(i), "lnag2.xlsx");
                    if (File.Exists(copypath)) File.Delete(copypath);
                    File.Copy(i, copypath);

                        using (var stream = File.OpenRead(copypath))
                    {
                        // IExcelDataReader 인스턴스 생성
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var trans = new TranslationData();
                            // 각 행을 반복하며 데이터 출력
                            while (reader.Read())
                            {
                                var entry = new List<string>();
                                for(int column = 0; column < reader.FieldCount; column++)
                                {
                                    entry.Add(reader.GetString(column));
                                }
                                trans.AddDialog(entry);
                            }
                            reader.NextResult();
                            while (reader.Read())
                            {
                                var entry = new List<string>();
                                for (int column = 0; column < reader.FieldCount; column++)
                                {
                                    entry.Add(reader.GetString(column));
                                }
                                trans.AddUIText(entry);
                            }
                            File.WriteAllText(Translation.path,JsonConvert.SerializeObject(trans,Formatting.Indented));
                        }
                    }
                    File.Delete(copypath);
                }
                else
                {
                    Debug.Log("엑셀파일 아님!");
                }
            }

    }
}
#endif
[Serializable]
class TranslationData
{
    public List<List<string>> dialog = new List<List<string>>();
    public List<List<string>> uitext = new List<List<string>>();
    public TranslationData()
    {
        language = PlayerPrefs.GetString("lang", "english");
    }
    public void AddDialog(List<string> entry)
    {
        dialog.Add(entry);
    }
    public void AddUIText(List<string> entry)
    {
        uitext.Add(entry);
    }
    public string GetEmotion(string id)
    {
        var lang = "emotion";
        var index = dialog[0].FindIndex(x => x == lang);
        var target = dialog.Find((x) => x[0].Trim().ToLower() == id.Trim().ToLower());
        return target[index];
    }
    public string GetDialog(string id)
    {
        var lang = GetLanguageNow();
        var index = dialog[0].FindIndex(x => x == lang);
        var target = dialog.Find((x) => x[0].Trim().ToLower() == id.Trim().ToLower());
        return target[index];

    }

    public string GetUIText(string id)
    {
        var lang = GetLanguageNow();
        var index = uitext[0].FindIndex(x => x == lang);
        var target = uitext.Find((x) => x[0].Trim().ToLower() == id.Trim().ToLower());
        return target[index];
    }

    public List<string> GetLanguages()
    {
        return dialog[0].GetRange(1, dialog[0].Count-1);
    }
    string language;
    public void SetCurrentLanguage(string language)
    {
        if (GetLanguages().Contains(language))
        {
            Debug.Log(language);
            this.language = language;
            PlayerPrefs.SetString("lang", language);
        }
        else
        {
            throw new Exception("해당 언어는 존재하지 않습니다.");
        }
    }
    public string GetLanguageNow()
    {
        return language;
    }

}