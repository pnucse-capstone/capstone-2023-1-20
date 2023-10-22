using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorBall : MonoBehaviour
{
    /*
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale = new Vector3(0.4F,0.4F,0.2F);
    }
    public void Clear()
    {
        gameObject.GetComponent<TrailRenderer>().Clear();
    }
    Vector3 prev_pos = new Vector3(0, 0, 0);
    float prev_time = 0;
    // Update is called once per frame
    void Update()
    {
        if (prev_time >Time.time)
        {
            StartCoroutine(lateClear());
        }
        prev_time = Time.time;
        Vector3 pos =NotePosition.defaultFunction(Game.time);
        if (Game.table.isEnd())
        {
            var now = gameObject.transform.localScale;
            gameObject.transform.localScale = Vector3.MoveTowards(now, Vector3.zero, Time.deltaTime);
        }
        gameObject.transform.position = pos;
        prev_pos = pos;
    }
    IEnumerator lateClear()
    {
        yield return null;
        Clear();
    }
    */
}
