using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaySpeedButton : MonoBehaviour
{
    [SerializeField]
    Text txt;
    // Start is called before the first frame update
    void Start()
    {
        SetPlaybackSpeed(Game.playSpeed);
    }
    static int i = 0;
    public void Click()
    {
        i++;
        i %= 5;
        var speed = 1F - 0.2F * i;
        SetPlaybackSpeed(speed);
    }
    void SetPlaybackSpeed(float speed)
    {
        Game.playSpeed = speed;
        txt.text = "X" + string.Format("{0:0.0}", speed);
        FMODWrapper.SetSpeed(speed);
    }
}
