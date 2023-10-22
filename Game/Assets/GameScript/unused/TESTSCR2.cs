using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using System.IO;

public class TESTSCR2 : MonoBehaviour
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public double x;
        public double y;
        public double z;
        public double w;
    }

    [SerializeField] AudioSource source;
    [DllImport("LuajitWrapper.dll")]
    private static extern bool Load(string url);
    [DllImport("LuajitWrapper.dll")]
    private static extern void NewTable();
    [DllImport("LuajitWrapper.dll")]
    private static extern void AddTablePair(string key,double value);
    [DllImport("LuajitWrapper.dll")]
    private static extern void SetGlobalTable(string name);
    [DllImport("LuajitWrapper.dll")]
    private static extern void SetGlobal(string name,double value);
    [DllImport("LuajitWrapper.dll")]
    private static extern Point Call(string name, int cnt_result);

    void Start()
    {
        Application.OpenURL(Application.persistentDataPath);
        Debug.Log(Application.persistentDataPath + "/test.lua");
        Load(Application.persistentDataPath + "/test.lua");
        Point a = Call("Position",3);
        Debug.Log((a.x,a.y,a.z,a.w));
//        StartCoroutine(Send());   
//        StartCoroutine(Get());
    }

    IEnumerator Send()
    {
        UnityWebRequest www = UnityWebRequest.Put("127.0.0.1:3000", new byte[] { 1, 2, 3, 4 });
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.uploadedBytes);

            // Or retrieve results as binary data
            byte[] results = www.uploadHandler.data;
            Debug.Log((results[0], results[1], results[2], results[3]));
        }
    }
    IEnumerator Get()
    {
        UnityWebRequest www = UnityWebRequest.Get("127.0.0.1:3000");
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadedBytes);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            Debug.Log((results[0], results[1], results[2], results[3]));
        }
    }
}