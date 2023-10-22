using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.Profiling;

public class ZipUtility
{
    string url;
    public ZipUtility(string url = "")
    {
        SetPath(url);
    }
    public void SetPath(string url)
    {
        this.url = url;
    }
    public string FindEntry(string prefix)
    {
        using (ZipArchive zip = ZipFile.Open(url, ZipArchiveMode.Read))
        {
            foreach(var i in zip.Entries)
            {
                if (i.Name.StartsWith(prefix))
                {
                    return i.Name;
                }
            }
        }
        return null;
    }
    public byte[] ReadEntry(string fileName)
    {
        byte[] buffer;
        using (ZipArchive zip = ZipFile.Open(url, ZipArchiveMode.Read))
        {
            var entry = zip.GetEntry(fileName);
            using (var stream = entry.Open())
            {
                buffer = new byte[entry.Length];
                stream.Read(buffer, 0, buffer.Length);
            }
        }
        return buffer;
    }
    public void WriteEntry(string fileName, string dataPath)
    {
        WriteEntry(fileName, File.ReadAllBytes(dataPath));
    }
    public void WriteEntry(string fileName, byte[] data)
    {
        string ext = Path.GetExtension(fileName);
        using (ZipArchive zip = ZipFile.Open(url, ZipArchiveMode.Update))
        {
            UnityEngine.Debug.Log((fileName,zip.Entries.Count));
            foreach (var i in zip.Entries)
            {
                if (i.Name.StartsWith(fileName))
                {
                    i.Delete();
                    break;
                }
            }
            var entry = zip.GetEntry(fileName);
            if (entry == null) entry = zip.CreateEntry(fileName);
            using (var stream = zip.GetEntry(fileName).Open())
            {
                //      stream.SetLength(0);
                stream.Write(data, 0, data.Length);
            }
        }
    }
}