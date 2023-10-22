using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
public class Loading : MonoBehaviour
{
    [SerializeField]
    PostProcessVolume volume;
    public void Popup()
    {
        gameObject.SetActive(true);

    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Fade());
    }
    IEnumerator Fade()
    {
        for(float t = 0; t < 0.5F; t += Time.deltaTime)
        {
            DepthOfField df;
            volume.profile.TryGetSettings(out df);
            df.focalLength.Override(600*t);
            yield return null;
        }
    }
    [SerializeField]
    Image image;
    public void End(float duration)
    {
        StartCoroutine(Out(duration));
    }
    IEnumerator Out(float duration)
    {
        for(float t=0;t< duration; t+=Time.deltaTime)
        {
            Color color = Color.white;
            color.a = t/duration;
            image.color = color;
            yield return null;
        }
        image.color = Color.white;
        yield return null;
    }
}
