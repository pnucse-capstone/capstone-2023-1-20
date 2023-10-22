using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField]
    RecordsBalloon balloon;
    int index = 0;
    [SerializeField]
    Image image;
    [SerializeField]
    List<Sprite> images;
    [SerializeField]
    Text text;
    [SerializeField]
    FadeScreen fade;
    // Start is called before the first frame update
    void Start()
    {
        Show(index);

    }
    public void End()
    {
        isLock = true;
        fade.FadeOut(() => SceneManager.LoadScene(SceneNames.SELECT));
        PlayerPrefs.SetInt("firstmeet", 0);
    }
    public void Prev()
    {
        index = Mathf.Max(0,index - 1);
        Show(index);
    }
    void Show(int index)
    {
        image.sprite = images[index];
        text.text = (index+1)+"/" + images.Count;
        balloon.Talk("tutorial"+index);

        if (isFirst() && index == 0)
        {
            balloon.SetText("firstmeet");
        }
    }
    bool isLock = false;
    public void Next()
    {
        if(index+1 == images.Count)
        {
            End();
        }
        else
        {
            index = Mathf.Min(images.Count-1, index + 1);
            Show(index);
        }
    }
    bool isFirst()
    {
        bool first = PlayerPrefs.GetInt("firstmeet", 1) == 1;
        return first;
        //        return true;
    }
    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) &&!isLock)
        {
            Prev();
        }
        if ((Input.GetKeyDown(KeyCode.D)|| Input.GetKeyDown(KeyCode.RightArrow)) &&!isLock)
        {
            Next();
        }   

    }
}
