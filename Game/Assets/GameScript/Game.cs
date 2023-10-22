using Async;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static GameState state;
    public static float judgepoint_offset = 0f;
    public static AudioSourceExapnd expand = new AudioSourceExapnd();
    public static bool reduced
    {
        get
        {
            if (caller == SceneNames.EDITOR) return false;
            return PlayerPrefs.GetInt("reduced", 0) == 1;
        }
    }
    public static bool isEditor
    {
        get
        {
            return caller == SceneNames.EDITOR || SceneManager.GetActiveScene().name == SceneNames.EDITOR;
        }
    }
    public static bool practice { get => PlayerPrefs.GetInt("Practice", 0) == 1; }
    public static bool thickmode { get => PlayerPrefs.GetInt("thick", 0) == 1; }
    public static int lineCount
    {
        get => _lineCount;
        set
        {
            Game.table.getEvents().ForEach((x) => x.SetLineCount(value));
            _lineCount = value;
        }
    }
    static int _lineCount = 6;
    public static GameMech.Type mechanim_type
    {
        get
        {
            if (table.use_script)
            {
                return GameMech.Type.LUA;
            }
            return GameMech.Type.DEFAULT;
        }
    }
    public static GameMech mechanim;
    public static ItemWrapper content_entry;
    public static Sprite icon;
    public static bool autoplay = false;
    public static float speed_multiplier = 1F;
    public static float time
    {
        get
        {
            return _time;
        }
        set
        {

            _time = value;
        }
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause && caller != SceneNames.EDITOR)
        {
            Pause();
        }
        if (caller == SceneNames.EDITOR)
        {
            SyncMusic();
            if (pause)
            {
                UnityEngine.Debug.Log("Pause");
                MusicPlayer.Pause();
            }
            else MusicPlayer.Resume();
        }
    }
    public static PbrffExtracter.FullEntry pbrffdata;
    public static void SyncMusic()
    {
        expand.time = time - table.offset + PlayerPrefs.GetFloat("sync", 0.1F);
        AsyncInput.SetTime(time);
        renderTime = AsyncInput.GetTime();
        expand.Adapt();
    }
    public static Table table
    {
        get
        {
            if (pbrffdata == null) return null;
            return pbrffdata.table;
        }
        set => pbrffdata.table = value;
    }
    public static float _time = 0;
    public static Vector3 cursor = new Vector3(0, 0);
    public static float mouse_sens = 0.5F;
    public static bool isStereo = true;
    public static bool tutorial = false;
    public static bool oneSided = false;
    public static int death_cnt = 0;
    public static float playSpeed = 1F;
    public static int maxlife = 5;
    public static float start_time = 0;
    public static string caller = "";
    public static MODI modi = new MODINone();
    public static void LoadSceneFrom(float start_time, string caller_scene)
    {
        caller = caller_scene;
        Game.start_time = start_time;
        Game.isPlaying = false;
        AsyncInput.Reset();
        AsyncInput.Pause();
        AsyncInput.SetTime(start_time);
        SceneManager.LoadScene(SceneNames.IN_GAME);
    }
    public static AsyncOperation LoadSceneFromAsync(float start_time, string caller_scene)
    {
        caller = caller_scene;
        Game.start_time = start_time;
        Game.isPlaying = false;
        AsyncInput.Reset();
        AsyncInput.Pause();
        AsyncInput.SetTime(start_time);
        return SceneManager.LoadSceneAsync(SceneNames.IN_GAME);
    }
    public static bool reverse = false;
    public static void ApplyOnGame(PbrffExtracter.FullEntry data)
    {

        UnityEngine.Debug.Log("ApplyOnGame");
        UnityEngine.Debug.Log("Mechanim Type:" + mechanim_type);
        state = new GameState();
        NoteEditor.isLoaded = true;
        pbrffdata = data;
        mechanim = GameMech.Create(mechanim_type);
        pbrffdata.icon = data.icon;
        var pcm = data.audio.GetAudio();
        MusicPlayer.SetMusic(pcm);
        NoteEditor.RefreshBPMS();
        NoteEditor.now_open_url = data.url;
        try
        {
            AudioSource source = GameObject.Find("오디오").GetComponent<AudioSource>();
            GameObject.Find("Refresh").GetComponent<Refresher>().HardRefresh();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log("에디터 아닌듯");
        };
        DirtyChecker.CleanDirty();
        ApplyTimeDistance();
        modi.Modify(table);
        UnityEngine.Debug.Log((modi.code, modi.name));
    }

    private static void ApplyTimeDistance()
    {
        state.timedistance = new TimeDistance(1);
        var speeds = (from item in Game.table.getEvents()
                      where item.isUse("speed2")
                      select (item.time, item.speed2, item.interpole == 0 ? TimeDistance.Type.NONE : TimeDistance.Type.LINEAR)).ToList();
        if (!speeds.Exists((x) => x.time == 0))
        {
            speeds.Add((0, 1, TimeDistance.Type.NONE));
        }
        state.timedistance.SetBPMs(speeds, 1);
    }

    public static PbrffExtracter.FullEntry GetFromGame()
    {
        return pbrffdata;
    }
    void Awake()
    {
        FMODWrapper.Refresh();
    }
    void Start()
    {
        //        mechanim = GameMechanim.Create(mechanim_type);

        ScoreBoard.life = 1F;

        Restart();

        keyRestart = KeySetting.keyRestart;
        keySkipMinus = KeySetting.GetMappedKey("keySkipMinus");
        keySkipPlus = KeySetting.GetMappedKey("keySkipPlus");

    }
    KeyCode keyRestart, keySkipPlus, keySkipMinus;
    public void Restart()
    {
        UnityEngine.Debug.Log("HASH:" + pbrffdata.hash);
        MusicPlayer.Reset();
        MusicPlayer.SetMusic(pbrffdata.audio.GetAudio());
        //        MusicPlayer.volume = Game.table.music_volume;
        MusicPlayer.loop = false;
        time = start_time;
        SyncMusic();
        Cursor.visible = false;
        death_cnt = 0;
        isPlaying = false;
        if (caller != SceneNames.EDITOR)
        {
            NoteEditor.sketchmode = false;
        }
        Camera.main.transform.rotation = Quaternion.identity;
        mouse_sens = PlayerPrefs.GetFloat("sens", 0.5F);

        ScoreBoard.Reset();
        NoteEditor.tableNow.Reset();
        Game.reverse = false;
        //modi.Modify(table);
        IgnoreNotes();
        StartCoroutine(PlayDelation());
    }
    private static void IgnoreNotes()
    {
        foreach (var i in NoteEditor.tableNow.getAllNotes((x) => true))
        {
            if (i.data.time < Game.time) i.Ignore();
        }
    }
    [SerializeField]
    GameObject UICamera;
    IEnumerator PlayDelation()
    {
        Game.time = start_time;
        expand.time = start_time;
        expand.Stop();
        yield return new WaitForSeconds(2);

        UICamera.SetActive(false);
        SyncMusic();
        expand.Play();
        isPlaying = true;

        AsyncInput.Resume();
        AsyncInput.Drop(); // 버퍼 비움
        StartCoroutine(CoFinish());
    }
    static bool _isPlaying = false;
    public static bool isPlaying
    {
        set
        {
            _isPlaying = value;
        }
        get => _isPlaying;
    }
    float _renderTime = 0F;
    public static float renderTime
    {
        get; set;
    }

    public GameObject pause;
    void Pause()
    {
        pause.GetComponent<PauseUI>().Pause();
    }
    [SerializeField]
    Button retry;

    void SetTime(float time)
    {

        foreach (var i in NoteEditor.tableNow.getAllNotes((x) => true))
        {
            if (i.data.time < time)
            {
                i.state.length = 0;
                i.state.judge = true;
                i.state.judge_result = JudgeType.miss;
            }
            else
            {
                i.state.reset();
            }
        }
        EffectRender.reset();
        Game.time = time;
        AsyncInput.SetTime(time);
        Game.renderTime = time;
        ScoreBoard.life = 1F;
        ScoreBoard.Reset();
        SyncMusic();
        GetComponent<InputApply>().reset();
        GetComponent<Refresher>().Refresh();
    }
    void Update() //
    {
        if (isPlaying)
        {
            UnityEngine.Debug.Assert(Mathf.Abs(time - AsyncInput.GetTime()) < 0.05);
            renderTime += Time.deltaTime * playSpeed;

            if (Mathf.Abs(renderTime - time) > 3 * Time.deltaTime * playSpeed)
            {
                UnityEngine.Debug.LogError("Render-Input RESYNC");
                Game.renderTime = time;
            }
            if (practice || caller == SceneNames.EDITOR)
            {
                if (Input.GetKeyDown(keySkipPlus))
                {
                    SetTime(time + 5);
                }

                if (Input.GetKeyDown(keySkipMinus))
                {
                    SetTime(Mathf.Max(0, time - 5));
                }
            }
        }
        #region Debug
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Home))
        {
            UnityEngine.Debug.Log((time, AsyncInput.GetTime()));
        }
        if (Input.GetKeyDown(KeyCode.End))
        {
            AsyncInput.SetTime(AsyncInput.GetTime() + 600);
            time = AsyncInput.GetTime();
            foreach (var i in NoteEditor.tableNow.getAllNotes((x) => true))
            {
                i.state.length = i.data.length;
                i.state.judge = true;
                i.state.judge_result = JudgeType.perfect;
                ScoreBoard.Write(JudgeType.perfect);
            }
        }
#endif
        #endregion
        Game.mechanim.Camera(Camera.main.transform);
        if (Input.GetKeyDown(KeyCode.Escape) && !Game.table.isEnd())
        {
            if (caller != SceneNames.EDITOR) Pause();
        };
        if (expand != null) expand.Update();
        if (!isPlaying) return;
        time = AsyncInput.GetTime();
        if (Input.GetKeyDown(keyRestart))
        {
            retry.onClick.Invoke();
            UICamera.SetActive(true);
        }

    }
    public void TriggerFinish()
    {
        StartCoroutine(CoFinish());
    }
    [SerializeField]
    FadeScreen fadeScreen;
    IEnumerator CoFinish()
    {

        while (!NoteEditor.tableNow.isEnd()) yield return null;
        while (!FCAlert.finish) yield return null;
        while (!NoteEditor.tableNow.isEnd()) yield return null;
        while (time < MusicPlayer.expandedlength) yield return null;
        UICamera.SetActive(true);
        if (caller == SceneNames.EDITOR)
        {
            EditorDontDestroy.instance.Escape();
        }
        else
        {
            MusicPlayer.FadeOut();
            if (practice)
            {
                fadeScreen.FadeOut(() => SceneManager.LoadScene(SceneNames.SELECT));
            }
            else
            {
                fadeScreen.FadeOut(() => SceneManager.LoadScene(SceneNames.SCORE));
            }
        }
    }
}
public class GameStopwatch
{

    Stopwatch watch = new Stopwatch();
    public void Stop()
    {
        watch.Stop();
    }
    public void Start()
    {
        watch.Start();
    }
    double baseTime = 0;
    public void SetTime(double time)
    {
        time *= 1000;
        baseTime = time;
        if (watch.IsRunning)
        {
            watch.Restart();
        }
        else
        {
            watch.Reset();
        }
    }
    public double GetTime()
    {
        return (baseTime + watch.ElapsedMilliseconds) / 1000;
    }

    public void Restart()
    {
        baseTime = 0;
        watch.Restart();
    }

    public void Reset()
    {
        baseTime = 0;
        watch.Reset();
    }
}
abstract public class GameMech// 게임 메커니즘을 정의
{
    public enum Type { DEFAULT, ONEBUTTON, LUA };
    public abstract NoteType noteType(int type_id);
    public abstract NoteType[] noteTypes { get; }
    public abstract NoteType lineType { get; }
    abstract public void Camera(Transform transform);
    public GameMech()
    {

    }
    public static GameMech Create(Type type)
    {
        switch (type)
        {
            case Type.DEFAULT: return new DefaultMech();
            case Type.ONEBUTTON: return new OneButtonMech();
                //            case Type.LUA: return new LuaMechanim();
        }
        return null;
    }
}
public class DefaultMech : GameMech
{
    NoteType[] _noteTypes = new NoteType[] { new NormalNoteType(), new CatchNoteType() };
    public override NoteType lineType => _noteTypes[0];

    public override NoteType[] noteTypes => _noteTypes;

    public DefaultMech() : base()
    {
        Game.lineCount = 6;
        //        noteType = new NormalNoteType();
    }


    public override void Camera(Transform transform)
    {
        var now = Game.state.properties;
        transform.position = new Vector3(now.positionCam[0], now.positionCam[1], -2);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, now.rotationCam));
        UnityEngine.Camera.main.orthographicSize = now.zoomCam;

        return;
    }

    public override NoteType noteType(int type_id)
    {
        return _noteTypes[type_id];
    }
}
public interface LineInfo
{
    Vector3 Position(NoteType type, int input_line, float t);
}



public class OneButtonMech : GameMech
{
    NoteType[] _noteTypes = new NoteType[] { new OneButtonNoteType(), new CatchNoteType() };
    public override NoteType lineType => _noteTypes[0];

    public override NoteType[] noteTypes => _noteTypes;

    public OneButtonMech() : base()
    {
        Game.lineCount = 6;
        //        noteType = new NormalNoteType();
    }


    public override void Camera(Transform transform)
    {
        var now = Game.state.properties;
        transform.position = new Vector3(now.positionCam[0], now.positionCam[1], -2);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, now.rotationCam));
        UnityEngine.Camera.main.orthographicSize = now.zoomCam;

        return;
    }

    public override NoteType noteType(int type_id)
    {
        return _noteTypes[type_id];
    }
}

class OneButtonNoteType : NormalNoteType
{
    public override bool Condition(GameState state, Note note, int input_line)
    {
        return true;
    }
}
