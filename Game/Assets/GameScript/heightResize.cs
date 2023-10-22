using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heightResize : MonoBehaviour
{
    // Start is called before the first frame update
    SpriteRenderer spr;
    public bool moving_side= false;
    void Start()
    {
        spr=gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float scaleX = 1;
        if (moving_side && Game.oneSided) scaleX = -1;
//        spr.transform.localScale = new Vector3(scaleX,Game.judge_y);
        
    }
}
