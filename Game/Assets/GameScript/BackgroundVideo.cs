using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class BackgroundVideo : MonoBehaviour
{
    Material mat;
    // Start is called before the first frame update
    VideoPlayer video;
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        if (Game.reduced)
        {
            gameObject.SetActive(false);
            return;
        }
        StartCoroutine(Load());
    }
    IEnumerator Load()
    {
        video = GetComponent<VideoPlayer>();
        video.source = VideoSource.Url;
        video.url = Game.pbrffdata.video_url;
        if(Game.pbrffdata.video_url== null)
        {
            gameObject.SetActive(false);
            yield break;
        }
        video.Prepare();
        while (!video.isPrepared) yield return null;
        video.time = Game.time;
        Debug.Log("·Îµù³¡");
        video.playbackSpeed = Game.playSpeed;
        video.Play();
    }
    // Update is called once per frame
    void Update()
    {
        if (Game.isPlaying) video.Play();
        else
        {
            video.Pause();
        }
        mat.SetColor("_ColorB", Game.state.properties.bgColor);
        mat.color = Game.state.properties.bgColor;
    }
}
