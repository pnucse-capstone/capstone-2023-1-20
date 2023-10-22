using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedrawProcessing : MonoBehaviour
{
    /*
    // Start is called before the first frame update
    public GameObject prefab;
    GameObject[] list;
    SpriteRenderer[] spr;
    void Start()
    {
        list = new GameObject[30];
        spr = new SpriteRenderer[30];
        for(int i = 0; i < 30; i++)
        {
            list[i] = Instantiate(prefab);
            list[i].SetActive(false);
            spr[i] = list[i].GetComponent<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    bool isNeedRender(EventData ev) // 시간 범위 내에 있는가 
    {
        float stime = Game.table.searchEvent(Game.time).time;
        float ftime = Game.table.searchEvent(Game.time + 10F).time;
        return stime<=ev.time && ftime>=ev.time;
    }
    void Update()
    {
        List<EventData> events = Game.table.getEvents(isNeedRender);
        gameObject.transform.position = Camera.main.transform.position;
        gameObject.transform.localScale = Camera.main.transform.localScale;
        gameObject.GetComponent<Camera>().orthographicSize = Camera.main.orthographicSize;
        gameObject.transform.rotation = Camera.main.transform.rotation;
        EventData now = Game.table.searchEvent(Game.time);
        for (int i = 0; i < 30; i++) list[i].SetActive(false);
        for (int i = 1; i < events.Count; i++)
        {
            if (events[i-1].speed < events[i].speed) 
            {
                list[i].transform.position = NotePosition.defaultFunction(events[i].time);
                spr[i].color = new Color(1,0,0);
                list[i].SetActive(true);
            }
            else if (events[i - 1].speed > events[i].speed)
            {
                list[i].transform.position = NotePosition.defaultFunction(events[i].time);
                spr[i].color = new Color(0, 0, 1);
                list[i].SetActive(true);
            }
            else
            {
                list[i].SetActive(false);
            }
        };
        for(int i = 0; i < events.Count; i++)
        {
            Color c = spr[i].color;

            float alpha = 0.6F * 1F - (Mathf.Clamp(4 * (Game.time - events[i].time), 0F, 1F));
            if (Game.time + 5 <= events[i].time)
            {
                const float note_fade_duration = 0.5F;
                alpha = Mathf.Lerp(0F, alpha, (Game.time + 5 + note_fade_duration - events[i].time) / note_fade_duration);
            }
            c.a = alpha;
            spr[i].color = c;
        }

    }
    */
}
