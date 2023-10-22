using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class SongInfoModify : MonoBehaviour
{
    // Start is called before the first frame update
    public void PannelRefresh()
    {
        gameObject.BroadcastMessage("Refresh");
//        NoteEditor.RefreshBPMS();
    }
}
