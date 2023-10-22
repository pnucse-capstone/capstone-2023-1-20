using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playAnimation : MonoBehaviour
{
    Animator animator;
    int isPlay = Animator.StringToHash("isPlay");
    // Start is called before the first frame update
    void Start()
    {
        animator=gameObject.GetComponent<Animator>();
        animator.SetBool(isPlay, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) animator.SetBool(isPlay, true);
    }
}
