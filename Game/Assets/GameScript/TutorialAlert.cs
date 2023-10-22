using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TutorialAlert : MonoBehaviour
{
    public GameObject image;
    public GameObject script;
    public GameObject mouseIcon;
    public GameObject supportIcon;
    public GameObject judgeIcon;

    public Sprite[] sp = new Sprite[] { };
    enum Tutorial{CONTACT,LEFT,RIGHT,LONG_LEFT,LONG_RIGHT,WHEEL , LIFE, JUDGE, START,NULL}
    // Start is called before the first frame update
    void Start()
    {
        if (!Game.tutorial) gameObject.SetActive(false);
        script.SetActive(false);
        mouseIcon.SetActive(false);
        supportIcon.SetActive(false);
//        judgeIcon.SetActive(false);
    }
    void setSprite(Tutorial type)
    {
        mouseIcon.GetComponent<Image>().sprite = sp[(int)type];
    }
    void setText(string txt)
    {
        script.GetComponent<Text>().text = txt;
    }
    // Update is called once per frame
    void Update()
    {
        if (Game.time > 64.5F) popup(Tutorial.CONTACT);
        else if (Game.time > 47F) popup(Tutorial.LONG_LEFT);
        else if (Game.time > 34.5F) popup(Tutorial.LONG_RIGHT); //37
        else if (Game.time > 22F) popup(Tutorial.WHEEL);
        else if (Game.time > 12F) popup(Tutorial.LEFT); //30
        else if (Game.time > 7.5F) popup(Tutorial.JUDGE);
        else if (Game.time > 2.5F) popup(Tutorial.RIGHT);
        else if (Game.time > 0F) popup(Tutorial.START);
    }
    IEnumerator ShowToggle(Tutorial type1,Tutorial type2=Tutorial.NULL)
    {
        if (type2 == Tutorial.NULL) type2 = type1;
        //        Debug.Log("show" + type);
        for (int j = 0; j <= 5; j += 1)
            for (float i = 0F; i <= 1F; i+=Time.deltaTime)
            {
                if (i <0.5F) setSprite(type1);
                else setSprite(type2);
                yield return null;
            }

    }
    IEnumerator FadeIn(float delay)
    {
        float fl = 1F;
        for(float i=0F;i<=fl; i+=Time.deltaTime)
        {
            setAlpha((float)i/fl);
            yield return null;
        }
        for (float i = 0F; i < delay; i+=Time.deltaTime) yield return null;
        for (float i = 0F; i <= fl; i+=Time.deltaTime)
        {
            setAlpha((float)(fl-i) / fl);
            yield return null;
        }
        setAlpha(0F);
    }
    void setAlpha(float alpha)
    {
        mouseIcon.GetComponent<Image>().color = new Color(1, 1, 1, alpha);
        supportIcon.GetComponent<Image>().color = new Color(1, 1, 1, alpha);
        foreach(var image in judgeIcon.GetComponentsInChildren<Image>())
        {
            var c = image.color;
            c.a = alpha;
            image.color = c;
        }
        foreach (var image in judgeIcon.GetComponentsInChildren<Text>())
        {
            var c = image.color;
            c.a = alpha;
            image.color = c;
        }

        script.GetComponent<Text>().color = new Color(1, 1, 1, alpha);
    }
    Tutorial prevType = Tutorial.NULL;
    void popup(Tutorial type, bool use_wait=true)
    {
        if (prevType == type) return;
        prevType = type;
        Debug.Log("helo" + type);
        StartCoroutine(FadeIn(3));
        script.SetActive(true);
        mouseIcon.SetActive(true);
        supportIcon.SetActive(false);
        judgeIcon.SetActive(false);
        setSprite(type);
        if(type==Tutorial.LONG_LEFT || type==Tutorial.LONG_RIGHT) StartCoroutine(ShowToggle(type,type));
        else StartCoroutine(ShowToggle(type,Tutorial.CONTACT));
        switch (type)
        {
            case Tutorial.START:
                supportIcon.SetActive(false);
                mouseIcon.SetActive(false);
                judgeIcon.SetActive(false);
                setText("TUTORIAL");
                break;
            case Tutorial.LIFE:
                setText("Life");
                break;
            case Tutorial.JUDGE:
                supportIcon.SetActive(false);
                mouseIcon.SetActive(false);
                judgeIcon.SetActive(true);
                setText("");
//                setText("WHITE: Perfect \n SKYBLUE: Good \n RED: Miss");
                break;
            case Tutorial.CONTACT:
                supportIcon.transform.position = mouseIcon.transform.position;
                supportIcon.transform.position += new Vector3(-40, 0, 0);
                setText("Move Mouse\n(Need NOT mouse click)");
                supportIcon.SetActive(true);
                break;
            case Tutorial.LEFT:
                setText("Left Click\n or \nPress A");
                break;
            case Tutorial.RIGHT:
                setText("Right Click\n or \nPress D");
                break;
            case Tutorial.WHEEL:
                setText("Scroll Mouse Wheel\n or \nPress S");
                supportIcon.SetActive(true);
                supportIcon.transform.position = mouseIcon.transform.position;
                supportIcon.transform.position += new Vector3(-6,-6,0);
                break;
            case Tutorial.LONG_RIGHT:
                setText("Hold down Right");
                break;
            case Tutorial.LONG_LEFT:
                setText("Hold down Left");
                break;
        }
        if(use_wait)StartCoroutine(wait(5));
    }
    IEnumerator wait(float delay)
    {
        Game.isPlaying = false;
        for(float i=0;i<delay; i+=Time.deltaTime)
        {
            yield return null;
        }
        Game.isPlaying = true;
    }
}
