using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FCAlert : MonoBehaviour
{
    [SerializeField]
    GameObject fcap;
    
    [SerializeField]
    FMODSFX apfx;
    [SerializeField]
    FMODSFX agfx;
    [SerializeField]
    FMODSFX fcfx;
    [SerializeField]
    FMODSFX cfx;

    Animator ani;
    public static bool finish = false;
    void Start()
    {
        finish = false;
        StartCoroutine(playAnime());
    }
    // Update is called once per frame
    void Update()
    {
    }
    public IEnumerator playAnime()
    {
        while (!NoteEditor.tableNow.isEnd()) yield return null;

        if (Game.practice && Game.caller != SceneNames.EDITOR)
        {
            finish = true;
            yield break;
        }
        ani = fcap.GetComponent<Animator>();
        var score = ScoreBoard.GetScoreEntry();
        if(score.isAllPerfect())
        {
            fcap.SetActive(true);
            apfx.Play();
            ani.SetTrigger("AP");
            
            yield return new WaitForSeconds(4F);
        }
        else if (score.isGoodPlay())
        {
            fcap.SetActive(true);
            agfx.Play();
            ani.SetTrigger("GP");
            yield return new WaitForSeconds(4F);
        }
        else if (score.isFullCombo())
        {
            fcap.SetActive(true);
            fcfx.Play();
            ani.SetTrigger("FC");
            yield return new WaitForSeconds(4F);
        }
        else if (score.isClear())
        {

            fcap.SetActive(true);
            cfx.Play();
            ani.SetTrigger("C");
            yield return new WaitForSeconds(4F);
        }
        finish = true;
    }
}
