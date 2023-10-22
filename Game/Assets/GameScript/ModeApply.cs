using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ModeApply : MonoBehaviour
{
    // Start is called before the first frame update
    HashSet<int> mode_ids;
    public void Start()
    {
        mode_ids = ModeConfig.loadList();
        foreach (int id in mode_ids)
        {
            Mode.getMode(id).Awake();
        }
        foreach(int id in mode_ids)
        {
            Mode.getMode(id).Start();
        }
    }
    // Update is called once per frame
    public void Update()
    {
        foreach (int id in mode_ids)
        {
            Mode.getMode(id).Update();
        }
    }
}
