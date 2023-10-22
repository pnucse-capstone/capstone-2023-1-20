using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SyncButton : MonoBehaviour
{
    public void Open()
    {
        SceneManager.LoadScene(SceneNames.SYNC);
    }
    // Start is called before the first frame update
}
