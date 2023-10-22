using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[RequireComponent(typeof(Button))]
public class SceneLoadButton : MonoBehaviour
{
    [SerializeField]
    public string sceneName;
    [SerializeField]
    FadeScreen fade;
    [SerializeField]
    FadeScreen.Motion fadeOutMotion;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => { StartCoroutine(Fade()); });
    }
    IEnumerator Fade()
    {
        fade.Animate(fadeOutMotion,() => { SceneManager.LoadScene(sceneName); });
        yield return null;
        ;
    }

}
