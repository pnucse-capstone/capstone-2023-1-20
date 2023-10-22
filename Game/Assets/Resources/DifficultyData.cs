using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

[CreateAssetMenu(fileName = "DifficultySet", menuName = "Scriptable Objects/Difficulty Setting", order = 1)]
public class DifficultyData : ScriptableObject
{
    [Serializable]
    public class DifficultyType
    {
        public string name;
        public Color color;
        public bool showInRecord= true;

        public static bool operator ==(string lhs, DifficultyType rhs)
        {
            return lhs.ToLower().Trim() == rhs.ToString().ToLower().Trim();
        }
        public static bool operator !=(string lhs, DifficultyType rhs)
        {
            return lhs.ToLower().Trim() != rhs.ToString().ToLower().Trim();
        }
        public override string ToString()
        {
            return name;
        }
    }

    [SerializeField]
    List<DifficultyType> difficulties;
    public DifficultyType GetDifficulty(string name)
    {
        return difficulties.Find(x => x.name.ToLower() == name.ToLower());
    }

    internal DifficultyType[] GetDifficulties()
    {
        return difficulties.ToArray();
    }

    public DifficultyData.DifficultyType FindNext(string name)
    {
        var i = difficulties.FindIndex(x => x.name == name);
        var next = (i + 1) % difficulties.Count;
        return difficulties[next];
    }

    internal DifficultyType First()
    {
        return difficulties[0];
    }

    internal DifficultyData.DifficultyType FindPrev(string name)
    {
        var i = difficulties.FindIndex(x => x.name == name);
        var next = (i + difficulties.Count - 1) % difficulties.Count;
        return difficulties[next];
    }
}
