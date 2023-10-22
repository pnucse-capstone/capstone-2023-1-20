using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonToggle : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Sprite> iconType;
    public List<string> iconText;
    public List<int> unlockLevel;
    public int mode_id = 0;
    public bool islocked = false;
    void Start()
    {
        
    }
    public void setMode(int mode_id)
    {
        if (PlayerPrefs.GetInt("totalExp", 0) / 300+1 >= unlockLevel[mode_id]) 
        {
            gameObject.GetComponentsInChildren<Image>()[1].sprite = iconType[mode_id];
            gameObject.GetComponentInChildren<Text>().text = iconText[mode_id];
            islocked = false;
        }
        else lockMode() ;
        this.mode_id = mode_id;
    }
    void lockMode()
    {
        setState(false);
        islocked = true;
        gameObject.GetComponentInChildren<Text>().text = "???";
    }
    public void setState(bool isUsing)
    {
        Color c1 = new Color(0.3F,0.3F,0.5F);
        Color c2 = new Color(0.14F, 0.14F, 0.23F);
        if (isUsing)
        {
            gameObject.GetComponentInChildren<Image>().color = Color.HSVToRGB(0, 0, 1);
            gameObject.GetComponentsInChildren<Image>()[1].color = c1;
            gameObject.GetComponentInChildren<Text>().color = Color.HSVToRGB(0, 0, 1);
        }
        else
        {
            gameObject.GetComponentInChildren<Image>().color = Color.HSVToRGB(0, 0, 0.55F);
            gameObject.GetComponentsInChildren<Image>()[1].color = c2;
            gameObject.GetComponentInChildren<Text>().color = Color.HSVToRGB(0, 0, 0.55F);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
