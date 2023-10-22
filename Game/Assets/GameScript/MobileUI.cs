using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileUI : MonoBehaviour
{
    public bool showOnlyMobile = false;
    // Start is called before the first frame update

    // Update is called once per frame
    void Start()
    {
#if UNITY_ANDROID
        gameObject.SetActive(showOnlyMobile);
#else
        gameObject.SetActive(!showOnlyMobile);
#endif
    }
}
