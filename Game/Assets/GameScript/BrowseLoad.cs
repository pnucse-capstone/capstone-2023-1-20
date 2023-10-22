using System.Collections;
using UnityEngine;
using SFB;
using System;
using System.IO;
using UnityEngine.UI;
using Steamworks;
using Steamworks.ServerList;

public class BrowseLoad : MonoBehaviour
{
    ExtensionFilter[] videoFilter= new ExtensionFilter[] { new ExtensionFilter("Video file", new string[] { "mp4", "wmv"}) };
    ExtensionFilter[] audioFilter = new ExtensionFilter[] { new ExtensionFilter("Audio file", new string[] { "wav", "mp3"}) };
    ExtensionFilter[] jsonFilter = new ExtensionFilter[] { new ExtensionFilter("json file", new string[] { "json" }) };
    ExtensionFilter[] zipFilter = new ExtensionFilter[] { new ExtensionFilter("pbrff file", new string[] { "pbrff" }) };
    ExtensionFilter[] midiFilter = new ExtensionFilter[] { new ExtensionFilter("midi file", new string[] { "mid" }) };
    public DrawSpectrum spectrum;
   
    string wdir 
    {
        get
        {
            string url = Application.persistentDataPath +"/Custom";
            if (!Directory.Exists(url)) Directory.CreateDirectory(url);
            return url;
        }
    }
    //    static byte[] loaded_audio;
    [SerializeField] Text wavfield;
    [SerializeField] Text waverror;
    public static PbrffExtracter pbrff = new PbrffExtracter(0x00);
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
        {
            onClickSave();

        }
    }
    #region button Callback
    public void onClickLua()
    {
        if (NoteEditor.isLoaded)
        {
            var url= LuaEngine.LUA_PATH;
            if (!File.Exists(url))
            {
                var txt = Resources.Load("default.lua") as TextAsset;
                File.WriteAllBytes(LuaEngine.LUA_PATH, txt.bytes);
            }
            System.Diagnostics.Process.Start(url);
        }
    }

    public void onClickMIDILoad()
    {
        string[] paths;
        paths = StandaloneFileBrowser.OpenFilePanel("Please select midi file.", "", midiFilter, false);
        if (paths.Length != 0)
        {
            string url = paths[0];
            Game.table.loadWithMIDI(url);
        }
    }
    public void onClickVideoLoad()
    {
        string[] paths;
        StopCoroutine("LoadBytesToClip");
        paths = StandaloneFileBrowser.OpenFilePanel("Please select video file.", wdir, videoFilter, false);
        if (paths.Length != 0)
        {
            string url = paths[0];
            Debug.Log(url);
            StartCoroutine(LoadVideo(url));
        }
        loadingIcon.SetActive(false);
    }
    public void onClickAudioLoad()
    {
        string[] paths;
        StopCoroutine("LoadBytesToClip");
        paths = StandaloneFileBrowser.OpenFilePanel("음악 파일을 골라주세요", wdir, audioFilter, false);
        if (paths.Length != 0)
        {
            string url = paths[0];
            StartCoroutine(LoadAudio(url));
        }
        
        loadingIcon.SetActive(false);
    }
    public void onClickLoad()
    {
        string[] paths;
        paths = StandaloneFileBrowser.OpenFilePanel("pbrff 파일을 골라주세요", wdir, zipFilter, false);
        if (paths.Length != 0)
        {
            string url = paths[0];
            ZipLoad(url);
            wavfield.text = "file";
        }
    }

    public void onClickSave()
    {
        if (NoteEditor.now_open_url!= null)
        {
            StartCoroutine(Save(NoteEditor.now_open_url));
        }
        else
        {
            onClickSaveAs();
        }
    }
    public void onClickSaveAs()
    {
        string path = null;
        path = StandaloneFileBrowser.SaveFilePanel("pbrff 파일을 골라주세요", wdir, "default.pbrff", zipFilter);
        if (path != null)
        {
            string url = path; 
            StartCoroutine(Save(url));
        }
    }
    #endregion

    public void ZipLoad(string url)
    {
        loadingIcon.SetActive(true);
        try
        {
            if (SteamClient.SteamId == Utility.DevId)
            {
                pbrff.SetKey(Utility.GetKey(url));
            }

        }
        catch
        {
        }
        try
        {
            var data = pbrff.LoadFull(url);
            Game.ApplyOnGame(data);
            NoteEditor.now_open_url = url;
            NoteEditor.history.Clear();
        }
        catch (Exception e)
        {
            MessagePopUp.Open(Translation.GetUIText("msg_jsonread"));
        }

        
        //        GameObject.Find("Refresh").GetComponent<Refresher>().HardRefresh();
        //
        loadingIcon.SetActive(false);

    }
    IEnumerator Save(string url)
    {
        yield return null;
        if (pbrff.canSave)
        {
            pbrff.FetchFrom(Game.pbrffdata);
            pbrff.SaveNowState(url);
            NoteEditor.now_open_url = url;
            NoticeMessage.Notify("Save Complete");
            DirtyChecker.CleanDirty();
        }
    }
    
    public GameObject loadingIcon;

    IEnumerator LoadAudio(string audio_path)
    {
        yield return null;
        try
        {
            pbrff.SetAudioFile(audio_path);
            var data = pbrff.GetFullData();
            Game.ApplyOnGame(data);
            if (data.audio != null)
            {
                GameObject.Find("Refresh").GetComponent<Refresher>().HardRefresh();
                wavfield.text = "file";
                waverror.text = "";
            }
        }
        catch(Exception e)
        {
            if(e.GetType() == typeof(ArgumentException)) 
            {
                Debug.LogError(e);
                waverror.text = "Error: We only support ogg/vorbis.";
            }
            else
            {
                Debug.LogError(e);
                waverror.text = "Error: Somethin goes Wrong.";
            }
        NoteEditor.isLoaded = false;
        }
    }
    IEnumerator LoadVideo(string video_path)
    {
        Game.pbrffdata.video_url = video_path;
        yield return null;
    }



}

