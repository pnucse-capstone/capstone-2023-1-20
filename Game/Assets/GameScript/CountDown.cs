using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CountDown : MonoBehaviour
{
    public Sprite[] numbers;
    AudioSource click;
    Image image;
    public TrailRenderer trail;
    void Start()
    {
        click = gameObject.GetComponent<AudioSource>();
        image = gameObject.GetComponent<Image>();
        image.enabled = false;
    }
    public void play()
    {
        StartCoroutine(coroutineWait());
    }
    IEnumerator coroutineWait()
    {
        trail.Clear();
        image.enabled = true;
        Game.isPlaying = false;
        for(int i=2;i>=0; i--)
        {
            image.sprite = numbers[i];
            click.Play();
            yield return new WaitForSeconds(0.7F);
        }
        image.enabled = false;
        Game.isPlaying = true;
    }
}
