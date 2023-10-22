using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RippleEffect : MonoBehaviour
{
    public bool fade = true;
    public bool resize = true;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            StopCoroutine(play());
            StartCoroutine(play());
        }
    }

    IEnumerator play()
    {
        float t = Time.time;
        float dur = 0.4F;
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        while (Time.time-t<dur)
        {
            float x = (Time.time - t)/dur;
            if(resize)gameObject.transform.localScale = new Vector3(x , x, 1);
            if(fade)gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1-x);
            yield return null;
        }
    }
}
