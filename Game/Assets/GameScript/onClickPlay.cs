using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onClickPlay : MonoBehaviour
{
    // Start is called before the first frame update
    public bool left=false;
    public bool right=false;
    // Update is called once per frame
    SpriteRenderer spr;
    void Start()
    {
        spr = gameObject.GetComponent<SpriteRenderer>();
    }
    void Update()
    {

        if (left &&Input.GetKeyDown(KeyCode.A)) spr.color = new Color(0.23F,0.7F,0.9F);
        if (Input.GetAxis("Scroll") != 0) spr.color = new Color(1F, 1F, 1F);
        if (Input.GetKey(KeyCode.S)) spr.color = new Color(0.23F, 0.7F, 0.9F);
        if (right && Input.GetKeyDown(KeyCode.D)) spr.color = new Color(0.23F, 0.7F, 0.9F);
        if (left && Input.GetMouseButtonDown(0)) spr.color = new Color(1F, 1F, 1F);
        if (right &&Input.GetMouseButtonDown(1)) spr.color = new Color(1F, 1F, 1F);
        if ((left && Input.GetAxis("LeftClick")==1)|| (right && Input.GetAxis("RightClick")==1) || (Input.GetAxis("Scroll")!=0))
            gameObject.GetComponent<Animator>().Play("",-1,0f);
    }
}
