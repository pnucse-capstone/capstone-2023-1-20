using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;


public class ObjPool<T> where T:class
{
    Dictionary<T, bool> usingObj = new Dictionary<T, bool>();
    public System.Func<T> newFunc;
 
    public T Get()
    {
        T result = null;
        
        foreach(var kp in usingObj)
        {
            if (kp.Value == false)
            {
                result = kp.Key;
                break;
            }
        }
        if (result == null)
        {
            result = newFunc();
        }      

        usingObj[result] = true;
        return result;
    }

    public void Put(T v)
    {
        usingObj[v] = false;
    }

}


public class TestScript : MonoBehaviour
{
    class sample
    {
        ColorAdapter color;
    }

    [MenuItem("Test/Exeperiment")]
    public static void forCheck()
    {
        var bytes = new byte[] { 1, 2, 3 ,4 };
        var hash = SHA512.Create();
        var data = hash.ComputeHash(bytes);
        var str = BitConverter.ToString(data).Replace("-","");
        Debug.Log(str.Length);
    }

    [MenuItem("Log/Show Table")]
    public static void tostrtest()
    {
        Debug.Log(JsonConvert.SerializeObject(Game.table));
        
    }
    [MenuItem("Log/Show Table First Event")]
    public static void FirstEvent()
    {
        Debug.Log(JsonConvert.SerializeObject(Game.table.getEvents((x)=>true)[0]));

    }

    [MenuItem("Test/HashCode")]
    public static void hashcodetest()
    {
        var table = new Table();
        Debug.Log(table.GetHashCode());
        table.addNote(new Note(new NoteData()));
        Debug.Log(table.GetHashCode());
    }
    [MenuItem("Test/Hash")]
    public static void hashtest()
    {
        var table = new Table();
        table.addNote(new Note(new NoteData()));
//        table.addEvent(new EventData());
        table.level = 50;
        var json = JsonUtility.ToJson(table);
        var bytes = Encoding.UTF8.GetBytes(json);        
        var md5 = MD5CryptoServiceProvider.Create();
        var hash = md5.ComputeHash(bytes);
        Debug.Log(json);
        Debug.Log(BitConverter.ToString(hash));
        
    }
    [MenuItem("Test/test1")]
    public static void test2()
    {
        var table = new Table();
        table.addNote(new Note(new NoteData(10, 1)));
        var a = TableJsonLoader.ToJSON(table);
        Debug.Log(a);

        var b = TableJsonLoader.GetTable(5,a);
        Debug.Log(JsonConvert.SerializeObject(b.getAllNotes((x)=>true)));
    }
    [MenuItem("Test/test2")]
    public static void Test()
    {

        var testJsonPath = Path.Combine(Application.persistentDataPath, "Custom", "Test.json");
        var jsonStr = File.ReadAllText(testJsonPath);
        Debug.Log("Load:" + jsonStr);
        Debug.Log("Lagacy Deserialize Test");
        var table = TableJsonLoader.GetTable(4, jsonStr);
        JsonConvert.DeserializeObject<Table>(jsonStr, new TableJsonConverterVersion4());
        var table2 = new Table();
        table.CopyTo(table2);

        Debug.Log(JsonConvert.SerializeObject(table2));
    }
}

public class DynamicContractResolver : DefaultContractResolver
{
    private readonly char _startingWithChar;

    public DynamicContractResolver(char startingWithChar)
    {
        _startingWithChar = startingWithChar;
    }

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

        // only serializer properties that start with the specified character
        properties =
            properties.Where(p => p.PropertyName.StartsWith(_startingWithChar.ToString())).ToList();

        return properties;
    }
}

public class TestJsonConverter : JsonConverter
{

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings() { Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        JToken t = JToken.FromObject(value);
        if (t.Type != JTokenType.Object)
        {
            t.WriteTo(writer);
        }
        else
        {
            JObject o = (JObject)t;
            var colors = o.Properties();
            o.WriteTo(writer);
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return new JObject();
    }

    public override bool CanConvert(Type objectType)
    {
        return true;
    }

    public override bool CanRead
    {
        get { return false; }
    }


}
public class Book
{
    public string BookName { get; set; }
    public decimal BookPrice { get; set; }
    public string AuthorName { get; set; }
    public int AuthorAge { get; set; }
    public string AuthorCountry { get; set; }
}
