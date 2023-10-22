using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankObject : MonoBehaviour
{
    [SerializeField]
    Image Rank;
    [SerializeField]
    Image plus;
    [SerializeField]
    Sprite S, A, B, C, D, plus1,plus2;
    // Start is called before the first frame update
    void Awake()
    {
        SetRank("");
    }
    public void SetRank(string rank)
    {
        Debug.Log("SetRank:"+rank);
        Rank.gameObject.SetActive(true);
        plus.sprite = plus1;
        switch (rank) 
        {
            case "S": Rank.sprite = S; plus.enabled = true; plus.sprite = plus2; break; 
            case "A+":Rank.sprite = A; plus.enabled = true; ; break;
            case "A": Rank.sprite = A; plus.enabled = false; break;
            case "B+": Rank.sprite = B; plus.enabled = true; break;
            case "B": Rank.sprite = B; plus.enabled = false; break;
            case "C+": Rank.sprite = C; plus.enabled = true; break;
            case "C": Rank.sprite = C; plus.enabled = false; break;
            case "D": Rank.sprite = D; plus.enabled = false; break;
            default: Rank.gameObject.SetActive(false); plus.enabled = false; break;
        }

    }
}
