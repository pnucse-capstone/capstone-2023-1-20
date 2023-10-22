using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    [SerializeField]
    RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        if (Game.practice && Game.caller != SceneNames.EDITOR) gameObject.SetActive(false);
        Update();
    }

    // Update is called once per frame
    void Update()
    {
        rect.sizeDelta = new Vector2(100*ScoreBoard.life,3.5F);
    }
}
