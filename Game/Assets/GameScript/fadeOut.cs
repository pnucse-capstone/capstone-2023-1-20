using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class fadeOut : MonoBehaviour
{
    // Start is called before the first frame update
    Image comp;
    void Start()
    {
        comp = gameObject.GetComponent<Image>();
        StartCoroutine(play());
    }
    IEnumerator play()
    {
        const float time = 0.5F;
        for(float i=0;i<=time; i+=Time.deltaTime)
        {
            comp.color = new Color(0, 0, 0, 1F-i/time);
            yield return null;
        }
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
