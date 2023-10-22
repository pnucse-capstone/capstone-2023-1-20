using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AutoBackup : MonoBehaviour
{
    // Start is called before the first frame update]
    static string BACKUP_FILE_PATH => Application.persistentDataPath + "/_AUTOBACKUP.pbrff";
    static string BACKUP_FILE_ARCHIVE_PATH {
        get {
            if (!Directory.Exists(Application.persistentDataPath + "/backup"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/backup");
            }
            return Application.persistentDataPath + $"/backup/{DateTime.Now}.pbrff".Replace(':', '-').Replace(' ','-');
        }
    }
    public static bool Exists()
    {
        return File.Exists(BACKUP_FILE_PATH);
    }
    public static void Archive()
    {

        string path = BACKUP_FILE_ARCHIVE_PATH;
        if (File.Exists(path)) File.Delete(path);
        if (!File.Exists(path) && File.Exists(BACKUP_FILE_PATH))
        {
            File.Move(BACKUP_FILE_PATH, path);
        }
    }
    public static void Open()
    {
        NoteEditor.PopupLock = false;
        PbrffExtracter pbrff = new PbrffExtracter(0x00);
        var data =pbrff.LoadFull(BACKUP_FILE_PATH);
        Game.ApplyOnGame(data);
        Archive();
    }
    PbrffExtracter pbrff;
    void Start()
    {

        pbrff = new PbrffExtracter(0x00);
        StartCoroutine(Backup());
    }
    IEnumerator Backup()
    {
        var code = DirtyChecker.GetDirtyCode();
        while (true)
        {
            if (pbrff.canSave && NoteEditor.isLoaded && DirtyChecker.isDirty() && code != DirtyChecker.GetDirtyCode())
            {
                if (File.Exists(BACKUP_FILE_PATH))
                {
                    File.Delete(BACKUP_FILE_PATH);
                }
                pbrff.FetchFrom(Game.pbrffdata);
                pbrff.SaveNowState(BACKUP_FILE_PATH);
                NoticeMessage.Notify("Backup complete");
                code = DirtyChecker.GetDirtyCode();
            }
            yield return new WaitForSeconds(120);
        }
    }

}
