using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadMusic : MonoBehaviour
{
    public GameObject progress_bar;
    public GameObject music_preview;
    public GameObject progress_text;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("load");
        gameObject.GetComponent<Animator>().speed=0F;
    }

    IEnumerator load()
    {
        Debug.Log("call");
        for (int i = 0; i < 10; i++)
        {
            var temp= Resources.Load("Song" + i, typeof(AudioClip));
            yield return null;
            progress_bar.transform.localScale=new Vector3((i+1)/10F,1,1);
            progress_text.GetComponent<Text>().text = string.Format("Loading: {0:f2}%",(i+1)/10F*100F);
        }
        Animator ani = gameObject.GetComponent<Animator>();
        ani.speed = 1F;
        ani.GetComponent<Animator>().Play("", -1, 0);
        while (ani.GetCurrentAnimatorStateInfo(0).normalizedTime < 1F) {  yield return null; };
        gameObject.SetActive(false);
        Debug.Log("done");
        music_preview.SetActive(true);
        unlockAlert.doPopup = true;
    }
    void Update()
    {
    }
}
