using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBackgroundImage : MonoBehaviour
{
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Image>().material;
        if (Game.pbrffdata.video_url != null)
        {
            gameObject.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!Game.reduced)
        {
            mat.SetColor("_ColorA", Game.state.properties.bgColor);
            mat.SetColor("_ColorB", Game.state.properties.bgColor2);
        }
        else
        {
            mat.SetColor("_ColorA", Color.black);
            mat.SetColor("_ColorB", Color.black);

        }
    }
}
