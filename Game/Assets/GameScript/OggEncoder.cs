using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

public static class OggEncoder {
    public static string Oggecn2Path = Application.dataPath + "/StreamingAssets/oggenc2.exe";
    static Queue<(string,string)> q = new Queue<(string, string)>();
    public static async Task Encode(string input, string output)
    {
        q.Enqueue((input, output));
        while(q.Count != 0)
        {
            var pair = q.Dequeue();
            await EncodeToOgg(pair.Item1, pair.Item2);
        }
    }
    static async Task EncodeToOgg(string input, string output)
    {
        await Task.Run(() =>
        {
            UnityEngine.Debug.Log(Oggecn2Path);
            UnityEngine.Debug.Log('"' + input + '"' + " -o " + '"' + output + '"');
            Process proc = new Process();

            proc.StartInfo.FileName = Oggecn2Path;
            proc.StartInfo.Arguments = '"'+input+'"' + " -o " + '"'+output+'"';
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;

            proc.Start();
            proc.WaitForExit();
            proc.Close();
        });
    }

}
