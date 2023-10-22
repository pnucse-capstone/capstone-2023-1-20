using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System.Threading.Tasks;

public class WorkshopUtility 
{
    public static async void StageClearTag(Steamworks.Ugc.Item item)
    {
        if (ScoreBoard.GetScoreEntry().isClear() && item.Owner.Name == SteamClient.Name)
        {
            Debug.Log("item:" + (item.Id,item.Title));
            var edit = new Steamworks.Ugc.Editor(item.Id);
            foreach(var i in item.Tags)
            {
                Debug.Log(i);
                edit = edit.WithTag(i);
            }
            var result = await edit.WithTag("Maker Clear").SubmitAsync();
            Debug.Log("Submit Complete:" + result.Result);
        }
    }
}
