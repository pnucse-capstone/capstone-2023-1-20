using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleFade : MonoBehaviour
{
    [SerializeField]
    float duration = 1;
    [SerializeField]
    AnimationCurve curve;

    Image image;
    // Start is called before the first frame update
    void Start()
    {
        image= GetComponent<Image>();
        image.raycastTarget = false;
        StartCoroutine(Co());
    }
    IEnumerator Co()
    {
        Color c;
        for (float t=0; t<duration; t+=Time.deltaTime)
        {
            c = image.color; 
            c.a = curve.Evaluate(t / duration);
            image.color = c;
            yield return null;
        }
        c = image.color;
        c.a = 0;
        image.color = c;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
