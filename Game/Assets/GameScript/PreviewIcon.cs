using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class PreviewIcon : MonoBehaviour
{
    public static Sprite DefaultSprite
    {
        get => Resources.Load<Sprite>("DefaultPreviewIcon");
    }
    ExtensionFilter[] bitmapFilter = new ExtensionFilter[] { new ExtensionFilter("image file", new string[] { "png", "jpg"}) };
    
    public void onClickImageLoad()
    {
        string[] paths;
        string wdir = Application.persistentDataPath;
        paths = StandaloneFileBrowser.OpenFilePanel("비트맵 파일을 골라주세요", "", bitmapFilter, false);
        if (paths.Length != 0)
        {
            string url = paths[0];
            Load(url);
        }
    }
    void Load(string url)
    {
        var bytes = File.ReadAllBytes(url);

        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        if (texture.width > 512 || texture.height > 512)
        {
            Debug.Log("downScale");
            texture = Utility.ScaleTexture(texture, 512, 512);
        }

        Game.pbrffdata.icon = new SpriteWrapper(texture.EncodeToJPG());
        GetComponent<Image>().sprite = Game.pbrffdata.icon;

//        Game.pbrffdata.MakeDirty();
    }
    
    void Refresh()
    {
        Debug.Log("Image Refresh");

        if(Game.pbrffdata != null && Game.pbrffdata.icon != null)
        {
            if(GetComponent<Image>() !=null) 
                GetComponent<Image>().sprite = Game.pbrffdata.icon;
        }
        else
        {
            if (GetComponent<Image>() != null) 
                GetComponent<Image>().sprite = DefaultSprite;
        }
    }
}
