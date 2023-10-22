using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class TableJsonLoader
{
    public static Table GetTable(int version, string json)
    {
        if (version == 0) return JsonConvert.DeserializeObject<Table>(json, new TableJsonConverterVersion5());
        else if (version == 4)
        {
            return JsonConvert.DeserializeObject<Table>(json, new TableJsonConverterVersion4());
        }
        else return JsonConvert.DeserializeObject<Table>(json, new TableJsonConverterVersion5());
    }

    public static string ToJSON(Table table)
    {
        return JsonConvert.SerializeObject(table, new TableJsonConverterVersion5());
    }
}
public class TableJsonConverterVersion4 : JsonConverter<Table>
{

    public override bool CanWrite
    {
        get { return false; }
    }

    public override void WriteJson(JsonWriter writer, Table value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override Table ReadJson(JsonReader reader, Type objectType, Table existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings() { Formatting = Formatting.Indented};
        var json = serializer.Deserialize(reader);
        JObject before = JObject.FromObject(json);
        JObject after = new JObject();
        List<NoteData> notes = new List<NoteData>();
        List<EventData> events = new List<EventData>();
        foreach (var i in before.Properties())
        {
            if (i.Name == "Notes")
            {
                notes = JArray.FromObject(i.Value).Select((x) =>
                {
                    NoteData data = x.ToObject<NoteData>();
                    data.color = Utility.DefaultColor(data);
                    data.lineColorStart=  Utility.DefaultLineColorStart(data);
                    data.lineColorEnd = Utility.DefaultLineColorEnd(data);
                    return data;
                }).ToList();
            }
            else if (i.Name == "Events")
            {
                JArray convert = EventsConvert(JArray.FromObject(i.Value));
                events = convert.Select((x) => x.ToObject<EventData>()).ToList();
            }
            else if (i.Name == "code")
            {
                after.Add("format_code", i.Value);
            }
            else
            {
                after.Add(i.Name, i.Value);
            }
        }
        Table table = after.ToObject<Table>();

        table.setNotes(notes.Select((x) => new Note(x)).ToList());
        table.setEvents(events);
        table.version = 5;
        return table;
    }

    private static JArray EventsConvert(JArray events)
    {
        List<JObject> list = new List<JObject>();
        foreach (JObject before in events)
        {
            JObject after = JObject.FromObject(before);
            RenameColorProperty("note_color_additive", "additiveNoteColor", before, after);
            RenameColorProperty("note_color", "multiplyNoteColor", before, after);
            RenameColorProperty("bg_color", "bgColor", before, after);
            RenameColorsProperty("line_color_data", "lineColors", before, after);

            var use = before.Value<JArray>("events").Select((x) => x.ToObject<bool>()).ToArray();
            var dic = new Dictionary<string, bool>();
            Array.Resize(ref use, 10);
            dic.Add("speed", use[0]);
            dic.Add("bgColor", use[1]);
            dic.Add("bgColor2", use[1]);
            dic.Add("speed2", use[2]);
            dic.Add("bpm", use[3]);
            dic.Add("width", use[4]);
            dic.Add("widths", use[4]);
            dic.Add("position", use[5]);
            dic.Add("multiplayNoteColor", use[6]);
            dic.Add("additiveNoteColor", use[6]);
            dic.Add("interpole", use[7]);
            dic.Add("lineColors", use[8]);
            if(use[7])after.Add("interpole",1);
            else after.Add("interpole",0);

            if (after.ContainsKey("events"))
                after.Remove("events");
            after.Add("use", JObject.FromObject(dic));
            list.Add(after);
        }
        return JArray.FromObject(list);
    }

    private static void RenameColorProperty(string oldName, string newName, JObject before, JObject after)
    {

        if(before.Value<JArray>(oldName) == null)return;
        float[] c = before.Value<JArray>(oldName).Select((x) => x.ToObject<float>()).ToArray();
        if (after.ContainsKey(oldName))
            after.Remove(oldName);
        var obj = JToken.FromObject(new ColorAdapter { r = c[0], g = c[1], b = c[2] });
        if (after.ContainsKey(newName))
        {
            after.GetValue(newName).Replace(obj);
        }
        else
        {
            after.Add(newName, obj);
        }
    }
    private static void RenameColorsProperty(string oldName, string newName, JObject before, JObject after)
    {
        if (before.Value<JArray>(oldName) == null) return;
        float[] c = before.Value<JArray>(oldName).Select((x) => x.ToObject<float>()).ToArray();
        if (after.ContainsKey(oldName))
            after.Remove(oldName);
        List<ColorAdapter> colors = new List<ColorAdapter>();

        for (int i = 0; i < c.Length; i += 3)
        {
            colors.Add(new ColorAdapter { r = c[i + 0], g = c[i + 1], b = c[i + 2] });
        }
        var obj = JToken.FromObject(colors);

        if (after.ContainsKey(newName))
        {
            after.GetValue(newName).Replace(obj);
        }
        else
        {
            after.Add(newName, obj);
        }
    }

}

public class TableJsonConverterVersion5 : JsonConverter<Table>
{
    public override void WriteJson(JsonWriter writer, Table table, JsonSerializer serializer)
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings() { Formatting = Formatting.Indented};
        JObject t = JObject.FromObject(table);
        var data = JArray.FromObject(t.Property("NoteList").Value)
            .Select((x)=>x.ToObject<Note>().data).ToList();
        t.Property("NoteList").Value.Replace(JArray.FromObject(data));
        t.WriteTo(writer);
    }
    public override Table ReadJson(JsonReader reader, Type objectType, Table existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var json = serializer.Deserialize(reader);
        JObject t = JObject.FromObject(json);
        //data를 note로 래핑함
        var data = JArray.FromObject(t.Property("NoteList").Value)
            .Select((x) => {
                var data = x.ToObject<NoteData>();
                if (JObject.FromObject(x).Property("color") == null)
                {
                    data.color = Utility.DefaultColor(data);
                }
                if (JObject.FromObject(x).Property("lineColorStart") == null)
                {
                    data.lineColorStart= Utility.DefaultLineColorStart(data);
                    data.lineColorEnd = Utility.DefaultLineColorEnd(data);
                }
                return new Note(data); 
            }).ToList();
        t.Property("NoteList").Value.Replace(JArray.FromObject(data));
        
        var data2 = JArray.FromObject(t.Property("EventList").Value)
            .Select((x) => {
                JObject noteimage = (JObject)x;
                var obj = x["noteImage"];
                if(!(obj is JArray))
                {
                    float v = obj.ToObject<float>();
                    obj.Replace(JArray.Parse($"[{v},{v},0]"));
                }
                var data = x.ToObject<EventData>();
                return data; 
            }).ToList();
        t.Property("EventList").Value.Replace(JArray.FromObject(data2));
        
        return t.ToObject<Table>();
    }

    public override bool CanRead
    {
        get { return true; }
    }


}
