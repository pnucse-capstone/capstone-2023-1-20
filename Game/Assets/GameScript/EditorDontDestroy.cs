using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EditorDontDestroy : MonoBehaviour
{
    public static bool isAlready_run = false;
    public static EditorDontDestroy instance;
    public float prev_time = 0;
    // Start is called before the first frame update
    
    void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }                   
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {

        if (SceneManager.GetActiveScene().name != SceneNames.EDITOR &&
            SceneManager.GetActiveScene().name != SceneNames.IN_GAME)
        {
            Destroy(gameObject);
            return;
        }
        if (isAlready_run)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Escape();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                PlayGame();
            }
        }
    }

    public void Escape()
    {
        Game.time = prev_time;
        SceneManager.LoadScene(SceneNames.EDITOR);
        isAlready_run = false;
    }

    public void PlayGame()
    {
        if (NoteEditor.PopupLock) return;
        if (!NoteEditor.isLoaded) return;
        isAlready_run = true;
        NoteEditor.selected_notes.Clear();
        GameEventEditor.selected.Clear();
        Game.LoadSceneFrom(Game.time, SceneNames.EDITOR);
        prev_time = Game.time;
    }
    // Update is called once per frame
}
