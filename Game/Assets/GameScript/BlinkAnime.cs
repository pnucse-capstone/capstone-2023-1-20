using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BlinkAnime : MonoBehaviour
{
    float seed = 0F;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        seed += Time.deltaTime;
        gameObject.GetComponent<Text>().color=new Color(0.6F,0.6F,1,0.8F+0.2F*Mathf.Sin(10*seed));
    }
}
