using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class StreamingImageAssets
{
    // Start is called before the first frame update
    static List<Sprite> noteImages =new List<Sprite>();
    static Sprite lineImage;
    static StreamingImageAssets()
    {
        LoadNoteImages();
        LoadLineImage();
    }

    private static void LoadLineImage()
    {
        var path = Path.Combine(Application.streamingAssetsPath, "Skin", $"line.png");
        if (File.Exists(path))
        {
            var bytes = File.ReadAllBytes(path);

            Texture2D texture = new Texture2D(2,2,TextureFormat.RGBA32,false);
            texture.filterMode= FilterMode.Bilinear;
            texture.LoadImage(bytes);
            Sprite spr = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),Vector2.one/2F,100,1,SpriteMeshType.FullRect,new Vector4(7,0,7,0));
            lineImage = spr;
        }
    }

    private static void LoadNoteImages()
    {
        for (int i = 0; i < 10; i++)
        {
            var path = Path.Combine(Application.streamingAssetsPath, "Skin", $"note{i}.png");
            if (File.Exists(path))
            {
                var bytes = File.ReadAllBytes(path);

                Texture2D texture = new Texture2D(512, 512,TextureFormat.RGBA32,false);
                texture.filterMode = FilterMode.Bilinear;
                texture.LoadImage(bytes);
                Sprite spr = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5F,0.5F), 100, 1, SpriteMeshType.FullRect, new Vector4(24, 0, 24, 0));
                noteImages.Add(spr);
            }
        }
    }

    public static Sprite GetNoteImage(int id)
    {
        return noteImages[id];
    }
    public static Sprite GetLineImage()
    {
        var bytes = File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, "Skin", "line.png"));
        return new SpriteWrapper(bytes);
    }
}
