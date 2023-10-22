using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class SpectrumBar : MonoBehaviour
{
    Bar[] bars;
    Texture2D texture;
    SpriteRenderer renderer;
    void Start()
    {        
        texture = new Texture2D(256, 32);
        renderer = gameObject.GetComponent<SpriteRenderer>();
        bars = new Bar[128];
        for (int i = 0; i < 128; i++) bars[i] = new Bar(1,1);
        Rect rect = new Rect(new Vector3(0, 0, 0), new Vector3(256, 32, 1));
        Sprite sp = Sprite.Create(texture, rect, new Vector2(0.5F, 0.5F));
        renderer.sprite = sp;
        Apply();
    }
    public float this[int i]
    {
        set { bars[i].height = value; }
    }
    public void Apply()
    {
        
        for (int i = 0; i < 256; i++)
        for (int j = 0; j < 32; j++)
        {
                texture.SetPixel(i, j, new Color(1, 1, 1, 0));
        }
            
        for(int i=0;i<128;i++)
        {
            Bar target = bars[i];
            int bar_width = 1;
            int scale = 1;
            Vector2Int pos = new Vector2Int(i*bar_width*2,0);
            for(int x=0;x<bar_width;x++)
            {
                for (int y = 0; y < scale*target.height; y++)
                {
                    texture.SetPixel(pos.x+x,pos.y+y,new Color(1,1,1));
                }
            }
        }
        texture.Apply();
    }
    class Bar
    {
        public float width, height;
        public Bar(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
}
