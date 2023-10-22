using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SongPlayer : MonoBehaviour
{
    static float play_offset;
    void Awake()
    {
    }
}
[Serializable]
public class Level
{
    public string title ;
    public string composer;
    public float bpm;
    public List<uint> level;
    public List<uint> table_code;
    public uint song_code;
    public int unlock_level = 0;
}
