using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class fadeIn : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isPlaying = false;
    public void play()
    {
        StartCoroutine(frame());
    }
    IEnumerator frame()
    {
        isPlaying = true;
        for (float i = 0; i <= 1F; i+=Time.deltaTime)
        {
            gameObject.GetComponent<Image>().color = new Color(0, 0, 0, i);
            yield return null;
        }
        isPlaying = false;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
