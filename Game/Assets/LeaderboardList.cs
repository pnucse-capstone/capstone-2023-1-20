using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LeaderboardList : MonoBehaviour
{
    [SerializeField] 
    GameObject entry;

    List<GameObject> pool = new List<GameObject>();
    void Start()
    {
        for(int i = 0; i < 30; i++)
        {
            var obj = Instantiate(entry,transform);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }
    public void Render(List<ScoreEntry> scores)
    {
        //범위 내에 있으면 렌더링
        foreach(var i in pool)
        {
            i.SetActive(false);
        }
        for (int i=0;i<scores.Count;i++)
        {
            if(IsVisible(i))
            {
                /*i번째 위치에 리스트의 i번째 원소를 표시*/
                var obj = pool[i % 12];
                obj.GetComponent<LeaderBoardEntry>().SetEntry(i+1, scores[i]);
                obj.transform.localPosition = Vector3.down * i * 52;
                obj.gameObject.SetActive(true);
            }
        }
    }
    public void Resize(int size)
    {
        GetComponent<RectTransform>().sizeDelta= Vector3.up*52*size;
    }

    internal void Init()
    {
        foreach (var i in pool)
        {
            i.GetComponent<LeaderBoardEntry>().Reset();
        }
    }
    public void Show(bool value)
    {
        foreach (var i in pool)
        {
            i.SetActive(value);
        }
    }

    internal bool IsVisible(int i)
    {
        var pos = Mathf.RoundToInt(transform.localPosition.y / 52F);
        return pos - 3 <= i && i < pos + 10;
    }
}
