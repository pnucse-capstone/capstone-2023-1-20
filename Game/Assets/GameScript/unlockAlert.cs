using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class unlockAlert : MonoBehaviour
{
    public static bool doPopup = false;
    public GameObject data;
    ButtonToggle mode_data;
    [SerializeField]
    Text mode_name;
    [SerializeField]
    Image icon;
    [SerializeField]
    Sprite songicon;
    [SerializeField]
    Sprite modeicon;
    // Start is called before the first frame update
    void Start()
    {
        mode_data = Instantiate(data).GetComponent<ButtonToggle>();
        StartCoroutine(popup());
    }
    bool isPopup = false;
    bool reopen = false;
    IEnumerator popup()
    {
        while (!doPopup) yield return null;
        //PlayerPrefs.GetInt("newAlert", -1) != -1
        if (true)
        {
            int level = PlayerPrefs.GetInt("totalExp", 0) / 300 + 1;
            for (int mode_id = 0; mode_id < ModeConfig.amount; mode_id++)
            {
                if (mode_data.unlockLevel[mode_id] <= level && !Mode.isUnlock(mode_id)) 
                {
                    Mode.unlock(mode_id);
                    StartCoroutine(startPopup());
                    show(mode_id);
                    isPopup = true;
                    while (!Input.GetMouseButtonDown(0)) 
                        yield return null;
                    StartCoroutine(closePopup());
                    yield return new WaitForSeconds(0.5F);
                };
            };
            List<Level> unlockedSong = new List<Level>();
            Debug.Log(unlockedSong.Count);
            foreach(Level song in unlockedSong)
            {
                StartCoroutine(startPopup());
                showSong(song);
                isPopup = true;
                while (!Input.GetMouseButtonDown(0)) yield return null;
                StartCoroutine(closePopup());
                yield return new WaitForSeconds(0.5F);
            }
            PlayerPrefs.SetInt("newAlert", -1);
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
        yield return null;
    }
    void show(int mode_id)
    {
        mode_name.text ="NEW MODI:\n"+mode_data.iconText[mode_id];
        icon.sprite = modeicon;//mode_data.iconType[mode_id];
    }
    void showSong(Level song)
    {
        mode_name.text = "NEW SONG:\n" + song.title;
        icon.sprite = songicon;
    }
    IEnumerator startPopup()
    {
        isPopup = true;
        gameObject.GetComponent<SimpleAnimation>().playExapnd();
        gameObject.GetComponent<SimpleAnimation>().playFadein();
        gameObject.GetComponent<AudioSource>().Play();
        yield return null;
    }
    IEnumerator closePopup()
    {
        isPopup = false;
        gameObject.GetComponent<SimpleAnimation>().playShrink();
//        gameObject.GetComponent<SimpleAnimation>().playFadeOut();
        yield return null;
    }
}
