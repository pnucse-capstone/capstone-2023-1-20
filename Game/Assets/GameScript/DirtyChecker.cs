using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
public class DirtyChecker : MonoBehaviour
{

    [SerializeField] Button btn_save;
//    [SerializeField] Button btn_load;
    [SerializeField] Button btn_saveas;
    void Start()
    {
        StartCoroutine(DirtyCheck());
    }
    IEnumerator DirtyCheck()
    {
        while (true)
        {
//            Debug.Log("dirty check");
            btn_save.interactable = NoteEditor.isLoaded && (isDirty() || NoteEditor.now_open_url == null);
            btn_saveas.interactable = NoteEditor.isLoaded;
            yield return new WaitForSeconds(1F);
        }
    }

    static int prev_dirty_code = 0;
    public static int GetDirtyCode()
    {

        int code = 0;
        if (Game.pbrffdata.video_url != null) code += Game.pbrffdata.video_url.GetHashCode();
        if (Game.pbrffdata.icon != null) code += Game.pbrffdata.icon.bytes.Length;
        if (Game.pbrffdata.audio.GetAudio() != null 
            && Game.pbrffdata.audio.GetAudio().GetPCM().Samples != null) 
            code += Game.pbrffdata.audio.GetAudio().GetPCM().Samples.Length;
        return code;
    }
    public static bool isDirty()
    {
        return prev_dirty_code != GetDirtyCode();
    }
    public static void CleanDirty()
    {
        prev_dirty_code = GetDirtyCode();
    }

    internal static void MakeDirty()
    {
        prev_dirty_code++;
    }
}
