using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeConfig : MonoBehaviour
{
    public GameObject icon;
    public GameObject[] icons;
    public HashSet<int> modes;
    public const int amount = 18;
    int page = 0;
    // Start is called before the first frame update
    void Awake()     
    {
        modes = new HashSet<int>();
        modes = loadList();
        icons = new GameObject[amount];
        for(int i=0;i<16; i++)
        {
            icons[i] = Instantiate(icon);
            icons[i].GetComponent<ButtonToggle>().setMode(i);
            icons[i].transform.SetParent(gameObject.transform);
            icons[i].transform.position = pos(i,new Vector3(4,4),new Rect(new Vector3(0.1F*Screen.width,0.35F*Screen.height),new Vector3(0.9F*Screen.width,Screen.height*0.5F)));
//            icons[i].transform.localScale = new Vector3(Screen.height/500F, Screen.height / 500F, 1);
        }
        foreach (int i in modes)
        {
            if (page * 16 < i && i < (page + 1) * 16)
            {
                int p = i % 16;
                ButtonToggle temp = icons[p].GetComponent<ButtonToggle>();
                temp.setState(true);
                Debug.Log(i);
            }
        }
    }
    public void nextPage()
    {
        page ++;
        page %= 2;
        int r = page * 16 + Mathf.Clamp(amount-16*page,0,16);
        for (int i = page*16; i < (page+1)*16; i++)
        {
            if(i<r)
            {
                Debug.Log(i);
                icons[i%16].GetComponent<ButtonToggle>().setMode(i);
                icons[i%16].SetActive(true);
            }
            else
            {
                icons[i%16].SetActive(false);
            }
        }
        foreach (int i in modes)
        {
            if(page*16<i && i<(page+1)*16)
            {
                int p = i % 16;
                ButtonToggle temp = icons[p].GetComponent<ButtonToggle>();
                temp.setState(true);
                Debug.Log(i);
            }
        }
    }
    void saveList()
    {
        string value = "";
        foreach (int i in modes)
        {
            value += i;
            value += ',';
        }
        if(value!="")value=value.Remove(value.Length-1);
        PlayerPrefs.SetString("modes", value);
//        Debug.Log(value);
    }
    public static HashSet<int> loadList()
    {
        HashSet<int> modes = new HashSet<int>();
        string value=PlayerPrefs.GetString("modes", "");
        string[] nums=value.Split(',');
        foreach(string i in nums)
        {
            if (i == "") continue;
            modes.Add(int.Parse(i));
        }
        return modes;
    }
    private Vector3 pos(int index,Vector3 div, Rect rect)
    {
        float i = index/ (int)div.y;
        float j = index % div.y;
        return new Vector3(i/div.x*rect.width+rect.x,j/div.y* rect.height+rect.y);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Close();
        if (Input.GetMouseButtonDown(0)) 
        {
            ButtonToggle temp = Physics2D.OverlapPoint(Input.mousePosition).gameObject.GetComponent<ButtonToggle>();
            int id=temp.mode_id;
            if(!temp.islocked)toggleMode(id);
            saveList();
        };
        int n = Mathf.Clamp(amount - 16 * page, 0, 16);
        for (int i=0;i<n; i++)
        {
            icons[i].GetComponent<ButtonToggle>().setState(false);
        }
        foreach (int i in modes)
        {
            if(page*16<=i && page*16+n>i)
            icons[i%16].GetComponent<ButtonToggle>().setState(true);
        }

    }
    void toggleMode(int mode_code)
    {
        if (modes.Contains(mode_code))
        {
            modes.Remove(mode_code);
        }
        else modes.Add(mode_code);
    }
    public void Open()
    {
        gameObject.SetActive(true);
//        gameObject.GetComponent<SimpleAnimation>().setAlpha(0);
        gameObject.GetComponent<SimpleAnimation>().playFadein();
    }
    public void Close()
    {
        gameObject.GetComponent<SimpleAnimation>().playFadeOut();
    }
}
