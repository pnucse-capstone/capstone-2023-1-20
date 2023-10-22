using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipIntervalComponent : MonoBehaviour
{
    [SerializeField]
    GameEventTrigger evs;
    [SerializeField]
    GameObject SkipDisplay;
    [SerializeField]
    Animator ani;
    [SerializeField]
    AnykeyDot[] dots;
    int cnt = 0;
    // Start is called before the first frame update
    AudioSourceExapnd expand;
    void Start()
    {
        expand = new AudioSourceExapnd();
        ani.Play("Appear", -1, 0);
        SetSkip(false);
    }
    bool isFade = false;
    // Update is called once per frame
    void Update()
    {
        var now = evs.Search("skip", Game.time, false);
        if (Game.isPlaying && now != null && Game.time < now.skip)
        {
            if (cnt > 3 && !isFade)
            {
                Debug.Log("Appear");
                ani.Play("Appear", -1, 0);
                cnt = 0;
            }
            SetSkip(true);
        }
        else
        {
            SetSkip(false);
        }
    }

    private void SetSkip(bool skip)
    {
        SkipDisplay.SetActive(skip);
        if (skip && Input.anyKeyDown && !isFade)
        {
            if (cnt < 3)
            {
                dots[cnt].Set(true);
                cnt++;
                GetComponent<FMODSFX>().Play();
            }
            if(cnt == 3)
            {
                StartCoroutine(fade());
                cnt++;
            }
        }
    }

    private void JumpTo(float time)
    {
        
        Game.time = time;
        Game.SyncMusic();
    }
    private IEnumerator fade()
    {
        isFade = true;
        ani.Play("Disappear", -1, 0);
        float target = 0.5F;
        for(float t = 0; t < 1; t += Time.deltaTime/target)
        {
            MusicPlayer.volume = 1-t;
            yield return null;
        }
        MusicPlayer.volume = 0;
        var now = evs.Search("skip", Game.time, false);
        JumpTo(now.skip);
        SkipDisplay.SetActive(false);
        for (float t = 0; t < 1; t += Time.deltaTime / target)
        {
            MusicPlayer.volume = t;
            yield return null;
        }
        MusicPlayer.volume = 1;
        Debug.Log(MusicPlayer.volume);
        dots[0].Set(false);
        dots[1].Set(false);
        dots[2].Set(false);
        isFade = false;
    }
}
