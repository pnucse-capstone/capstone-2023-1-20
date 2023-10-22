using Async;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class PauseUI : MonoBehaviour
{
    [SerializeField]
    GameObject UICamera;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Pause");
        isFade = false;
        gameObject.SetActive(false);
    }
    [SerializeField] Button retry;
    public GameObject music;
    [SerializeField]VideoPlayer video;
    [SerializeField] Animator ani;
    // Update is called once per frame
    void Update()
    {
        if (isFade) return;
        if (Input.GetKeyDown(KeySetting.keyRestart))
        {
            retry.onClick.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Resume();
        }
    }
    float pauseTime = 0;
    public void Pause()
    {
        UICamera.SetActive(true);
        if (!Game.isPlaying) return;
        ani.Play("Appear", -1, 0);
        Cursor.visible = true;
        Game.isPlaying = false;
        gameObject.SetActive(true);
        Game.expand.Stop();
        AsyncInput.Pause();
        isFade = true;
        StartCoroutine(fadeWait());
    }
    IEnumerator fadeWait()
    {
        yield return new WaitForSeconds(0.3F);
        isFade = false;
    }
    public void Retry()
    {
        if (isFade) return;
        ani.Play("Disappear", -1, 0);
        isFade = true;
        //        music.GetComponent<SongPlayer>().Play();
        //        gameObject.SetActive(false);
        fadescreen.FadeOut(() => SceneManager.LoadScene(SceneNames.IN_GAME));
    }
    public IEnumerator GotoPauseTime()
    {
        var prev = Game.time;
        while(Game.time >= pauseTime)
        {
            Game.time -=Time.deltaTime*3;
            Game.renderTime = Game.time;

            var notes = Game.table.getAllNotes((x) => x.data.time < prev && x.data.time >= Game.time 
            && x.state.judge_result != JudgeType.NONE && !x.state.judge);
            foreach (var note in notes)
            {
                Debug.Log("Cancel:"+note.data.time+","+note.state.length);
                note.state.cancel();
                ScoreEffect.ShowCombo();
                ScoreEffect.ShowPgm();
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.3F); // 애니메이션 끝 대기 
        Game.time = Game.renderTime= pauseTime;
        Game.SyncMusic();
        Game.isPlaying = true;
        Game.expand.Play();
        AsyncInput.SetTime(pauseTime);
        AsyncInput.Resume();
        gameObject.SetActive(false);
        UICamera.SetActive(false);
    }
    public void Resume()
    {
        if (isFade) return;
        isFade  = true;
        ani.Play("Disappear", -1, 0);
        var now = Game.time;
        EffectRender.reset();
        pauseTime = Mathf.Max(Game.time - 3, pauseTime); // 퍼즈타임보다 앞쪽으로는 가지 않음
        StartCoroutine(GotoPauseTime());
        Cursor.visible = false;
        var notes = Game.table.getAllNotes((x) => x.data.time < now && x.data.time >= pauseTime 
        && x.state.judge_result != JudgeType.NONE);
        foreach (var note in notes)
        {
            if (note.state.judge)
            {
                note.state.hide = true;

            }
            else
            {
                note.state.cancel();

                //                note.state.length = 0;
            }
        }
        video.time = pauseTime;

    }
    public static bool isFade = false;
    public void Exit()
    {
        if (isFade) return;
        ani.Play("Disappear", -1, 0);
        isFade = true;
        fadescreen.FadeOut(() => SceneManager.LoadScene(SceneNames.SELECT));
        KeySoundPallete.Release();
    }
    [SerializeField]
    public FadeScreen fadescreen;
}
