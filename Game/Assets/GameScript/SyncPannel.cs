using Async;
using FMOD;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SyncPannel : MonoBehaviour
{
    public Text tf_sync;
    public Transform sync_note;
    public Transform sync_line;
    public Slider slider;
    public float time = 0F;
    IEnumerator SoundPlay()
    {
        while (true)
        {
            FMODWrapper.PlaySimpleSound(sound);
            UnityEngine.Debug.Log("hit");

            time = Time.time;
            sync.Reset();
            yield return new WaitForSeconds(1);
        }
    }
    Sync sync;
    SyncAccurate syncAcc = new SyncAccurate();
    SyncUnity syncUnity = new SyncUnity();
    // Start is called before the first frame update

    Sound sound;
    void Awake()
    {
        syncAcc.Start();
        syncUnity.Start();

        sound = FMODWrapper.CreateSimpleSound(Path.Combine(Application.streamingAssetsPath, "syncTest.wav"));
    }
    void Start()
    {
        SetSync(GetSync());

        if (PlayerPrefs.GetInt("asyncInput", 1) == 1)
        {
            //            sync = syncAcc;
            sync = syncUnity;
        }
        else
        {
            sync = syncUnity;
        }
        StartCoroutine(SoundPlay());
    }
    public void ApplySlider(float value)
    {
        SetSync(value / 1000F);
    }
    private void SetSync(float sync)
    {
        sync = Mathf.Clamp(sync, -0.5F, 0.5F);
        tf_sync.text = formatSync(sync);
        PlayerPrefs.SetFloat("sync", sync);
        sync_line.localPosition = Vector3.up * sync * 200;

        slider.value = Mathf.RoundToInt(PlayerPrefs.GetFloat("sync", 0.1F) * 1000);

    }
    void Update()
    {
        syncAcc.Update();
        syncUnity.Update();
        var times = sync.GetTimes();

        foreach (var t in times)
        {
            SetSync(t);
        }
        sync_note.localPosition = Vector3.up * (Time.time - time - 0.5F) * 200;// 기준 라인 표시
        sync_line.localPosition = Vector3.up * (PlayerPrefs.GetFloat("sync", 0.1F)) * 200;
    }

    private void OnDestroy()
    {
        AsyncInput.Suspend();
    }

    private static float GetSync()
    {
        return PlayerPrefs.GetFloat("sync", 0.1F);
    }

    string formatSync(double value)
    {
        return string.Format((value > 0 ? "+" : "") + "{0:F3}" + "s", value);
    }
    public void toMain()
    {
        sound.release();
        StartCoroutine(FadeToMain());
    }
    public GameObject fadeScreen;
    public IEnumerator FadeToMain()
    {
        fadeScreen.SetActive(true);
        for (float time = 0; time < 0.25F; time += Time.deltaTime)
        {
            fadeScreen.GetComponent<Image>().color = new Color(0, 0, 0, time * 4F);
            yield return null;
        }
        SceneManager.LoadScene(SceneNames.INTRO);
    }
    public void ToggleAccurate(bool value)
    {
        StopAllCoroutines();
        Start();

    }
    interface Sync
    {
        void Start();
        void Update();
        void Reset();
        float[] GetTimes();
    }
    class SyncUnity : Sync
    {
        float[] times = new float[0];
        public float[] GetTimes()
        {
            return times;
        }
        float time = 0;
        public void Reset()
        {
            time = 0;
        }

        public void Start()
        {
        }

        public void Update()
        {
            time += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Z))
            {
                times = new float[1] { time - 0.5F };
            }
            else
            {
                times = new float[0];
            }
        }
    }
    class SyncAccurate : Sync
    {
        float[] times = new float[0];
        public float[] GetTimes()
        {
            return times;
        }

        public void Reset()
        {
            times = new float[0];
            AsyncInput.SetTime(0);
            //AsyncInput.SetTime(0);

        }

        public void Start()
        {
            AsyncInput.Init();
            AsyncInput.Register(User32.VirtualKey.VK_Z);
            AsyncInput.Resume();
        }

        public void Update()
        {
            var syncs = AsyncInput.GetInputs();

            times = syncs
                .Where((x) => x.state == AsyncInput.KeyInput.DOWN)
                .Select((x) => x.time - 0.5F).ToArray();

        }
    }
}
