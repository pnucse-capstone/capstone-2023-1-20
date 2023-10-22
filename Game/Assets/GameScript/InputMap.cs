using Async;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMap
{ // 라인을 대응시키는 구현만... 

    KeyCode[] mapper;

    public int Count
    {
        get => mapper.Length;
    }

    public InputMap()
    {
        Reset();
    }

    public void Reset()
    {
        mapper = new KeyCode[16];
        for (int i = 0; i < Game.lineCount; i++)
        {
            Map(KeySetting.GetMappedKey("key" + i), i);
        }
    }

    public void Map(KeyCode From, int To)
    {
        mapper[To] = From;
    }

    public KeyCode mappedKey(int line)
    {
        return mapper[line];
    }
    public string getInputString(int line)
    {
        return mappedKey(line).ToString();
    }

}

