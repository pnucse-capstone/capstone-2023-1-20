using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GradientControl : MonoBehaviour
{
    /*
    GradientNew gradSky;
    public float test = 0F;
    public Color bot;
    public Color top;
    // Start is called before the first frame update
    void Start()
    {
        gradSky = gameObject.GetComponent<GradientNew>();
        if (PlayerPrefs.GetInt("BackgroundEffect", 1) == 0) gameObject.GetComponent<SpriteRenderer>().enabled=false;
    }
    static Color target =new Color(0,0,0);
    // Update is called once per frame
    static float lumin = 0F;
    static float att = 0F;
    static float comb = 0F;
    static float tone = 0F;
    static float l_offset = 0F;
    static float tone_offset = 0F;
    public static void attack() //순간 밝아짐
    {
        att= 1F;
    }
    public static void transition() //분위기 전환
    {
        tone = UnityEngine.Random.Range(0,3.141592F);
    }
    void Update()
    {
        comb = ScoreBoard.combo ;
        lumin = att*0.1F+Mathf.Log(comb+0.5F)/30F;
        tone_offset = tone;
        EventData ev=Game.table.searchEvent(Game.time);
        tone_offset += ev.color;
        bot = Color.HSVToRGB((float)Math.Sin(Game.time/10+tone_offset) / 16 + 0.73F, 0.8F, 0.15F+l_offset/3);
        top = Color.HSVToRGB((float)Math.Sin(Game.time/10+tone_offset+1.5F)/16+0.52F, 0.8F, 0.15F+l_offset/3);
        if (Game.life == 0F)
        {
            lumin = 0.01F;
            top = new Color(0, 0, 0);
            bot.r= 0.25F;
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            Camera.main.backgroundColor = new Color(0.1F, 0, 0);
        };

        gradSky.setColor(bot, top);
        l_offset = Mathf.Lerp(l_offset,lumin,0.2F);
        att = Mathf.Lerp(att,0,Time.deltaTime);
        tone += 0.001F;

    }*/
}
