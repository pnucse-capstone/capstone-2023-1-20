using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
public class InGameUI : MonoBehaviour
{
    public GameObject combo;
    public GameObject pgm;
    public GameObject score;
    public GameObject time;
    public AudioSource source;
    Text txt_time;

    const int amount = 6;
    private static GameObject[] combo_box = new GameObject[amount];
    private static Text[] combo_text = new Text[amount];
    private static int index = 0;
    private string lensound;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < amount; i++)
        {
            combo_box[i] = Instantiate(combo);
            combo_box[i].transform.SetParent(gameObject.transform);
            combo_box[i].SetActive(false);
            combo_text[i] = combo_box[i].GetComponent<Text>();
        }
        txt_time= time.GetComponent<Text>();
        lensound = " / "+timeFormat(MusicPlayer.expandedlength);
        StartCoroutine(Refresh());
    }

    IEnumerator Refresh()
    {
        float prev = -1;
        while (true)
        {
            float time = Mathf.Min(Game.time, MusicPlayer.expandedlength);
            if(Mathf.Round(time) != Mathf.Round(prev))
            {
                string timestamp = timeFormat(time) + lensound;
                txt_time.text = timestamp;
            }
            yield return null;
            prev = time;
        }
    }
    // Update is called once per frame
    private string timeFormat(float second)
    {
        int m = (int)second / 60;
        int s = (int)second % 60;
        string pad = s < 10 ? "0" : "";
        return $"{m}:{pad}{s}";
    }
}
