using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UILevelEvaluationPopup : MonoBehaviour
{


    [SerializeField] Button Upvote;
    [SerializeField] Button Downvote;
    [SerializeField]Color upcolor;
    [SerializeField] Color downcolor;

    [SerializeField]Animator ani;
    void Start()
    {

        if (SteamClient.IsValid)
        {
            Upload();
        }
    }
    async void Upload()
    {

        var vote = await Game.content_entry.GetUserVote();
        if (vote.HasValue)
        {
            SetButtonColor(vote.Value);
        }
        ani.SetTrigger("Appear");

    }
    void SetButtonColor(Steamworks.Ugc.UserItemVote vote)
    {
        Upvote.image.color = Color.white;
        Downvote.image.color = Color.white;
        if (vote.VotedUp)
        {
            Upvote.image.color = upcolor;
        }
        if(vote.VotedDown)
        {
            Downvote.image.color = downcolor;
        }
    }
    public void EvalGood()
    {
        Game.content_entry.Vote(true);
        Upvote.image.color = upcolor;
        Downvote.image.color = Color.white;
    }
    public void EvalNone()
    {
        gameObject.SetActive(false);
    }
    public void EvalBad()
    {
        Game.content_entry.Vote(false);
        Upvote.image.color = Color.white;
        Downvote.image.color = downcolor;
    }
    // Update is called once per frame
    void Update()
    {
    }
}

