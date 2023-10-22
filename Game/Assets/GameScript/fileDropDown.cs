using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
public class fileDropDown : MonoBehaviour
{
    [SerializeField]
    string[] format;

    Dropdown dropdown;
    string file_path;
    List<FileInfo> list;
    // Start is called before the first frame update
    void Start()
    {
        file_path = Application.persistentDataPath + "/AdofaiStorage";
        Directory.CreateDirectory(file_path);
        dropdown = gameObject.GetComponent<Dropdown>();
        dropdown.ClearOptions();
        
        list = new List<FileInfo>();
        var info = new DirectoryInfo(file_path);
        for(int i=0;i<format.Length;i++)
        {
            var fileInfo = info.GetFiles("*."+format[i]);
            foreach (var file in fileInfo)
            {
                list.Add(file);
            }
        }

        var options = new List<Dropdown.OptionData>();
        foreach (var i in list)
        {
            options.Add(new Dropdown.OptionData(i.Name));
        }
        dropdown.AddOptions(options);
    }
    public void Refresh()
    {
        Start();
    }
    public byte[] loadFile()
    {
        string name = dropdown.captionText.text;
        Debug.Log(file_path + '/' + name);
        byte[] buffer = File.ReadAllBytes(file_path + '/' +name);
        return buffer;
    }
    public AudioClip loadedClip;
    public bool isDone = false;
    public void loadAudioClip()
    {
        StartCoroutine(loadAC());
    }
    IEnumerator loadAC()
    {
        string name = dropdown.captionText.text;
        Debug.Log(name);
        WWW request = new WWW(file_path + '/' + name);
        while (!request.isDone)
        {
            yield return null;
        }
        isDone = true;
        loadedClip= request.GetAudioClip();
    }
    public string loadString()
    {
        string name = dropdown.captionText.text;
        Debug.Log(file_path + '/' + name);
        string str = File.ReadAllText(file_path + '/' + name);
        Debug.Log(str);
        return str;
    }
    public string getPath()
    {
        string name = dropdown.captionText.text;
        return file_path + '/' + name;
    }
}
