using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimeStart : MonoBehaviour
{
    public float min_waitTime = 0F;
    public float max_waitTime=0F;
    float default_speed ;
    float wait_time;
    // Start is called before the first frame update
    void Start()
    {
        wait_time = Random.Range(min_waitTime, max_waitTime);
        default_speed = gameObject.GetComponent<Animator>().speed;
        gameObject.GetComponent<Animator>().speed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (wait_time<=0) return;
        wait_time -= Time.deltaTime;
        if (wait_time<=0) gameObject.GetComponent<Animator>().speed = default_speed;
    }
}
