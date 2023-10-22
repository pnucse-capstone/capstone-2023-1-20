using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideWhenFinished : MonoBehaviour
{
    void Update()
    {
        if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime == 1)
        {
            Debug.Log("fin");
        }
    }
}
