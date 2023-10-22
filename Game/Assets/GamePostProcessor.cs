using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GamePostProcessor : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    UnityEvent updateTargets;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        updateTargets.Invoke();
    }
}
