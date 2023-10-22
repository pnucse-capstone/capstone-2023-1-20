using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LineProperties : MonoBehaviour
{
    public enum EntryType { TEXT, BUTTON}
    [SerializeField]
    EntryType type;
    [SerializeField]
    GameObject textPrefab;
    [SerializeField]
    GameObject colorPrefab;
    [SerializeField]
    GameEventEditEntry entry;
    List<GameObject> entries = new List<GameObject>();
    // Start is called before the first frame update
    void Awake()
    {
        if(entry == null) entry = GetComponentInParent<GameEventEditEntry>();
        Debug.Log("AWAKE: LINEPROPS");
        for (int i = 0; i < Game.lineCount; i++)
        {
            GameObject a;
            if(type == EntryType.TEXT)
            {
                a = Instantiate(textPrefab, transform);
            }
            else if (type == EntryType.BUTTON)
            {
                a = Instantiate(colorPrefab, transform);
            }
            else
            {
                throw new System.Exception("¹º°¡ Àß¸øµÆ¾î¿ä");
            }

            entries.Add(a);
        }

        var L = entries.Select(x => x.GetComponent<Selectable>()).ToList();
        foreach(var i in L)Debug.Log(i); 
        entry.SetFields(L);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
