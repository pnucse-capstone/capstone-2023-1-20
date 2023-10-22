using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;
using static UnityEngine.RectTransform;

public class EffectRender : MonoBehaviour
{
    Vector3 origin;
    static  Dictionary<int, EffectPool> Effects;
    void Start()
    {
        Effects = new Dictionary<int, EffectPool>();
        foreach(NoteType i in Game.mechanim.noteTypes)
        {
            int id = i.getEffectId();
            if (!Effects.ContainsKey(id))
            {
                var temp = ImageJournal.getEffect(EffectDataType.BUILT_IN, id);
                Effects.Add(id, new EffectPool(temp, Game.lineCount));
            }

        }
    }
    void Update()
    {

    }
    public static void makeNoteEffect(Note note,JudgeType judge,NoteStep step)
    {
        EffectPool effecter = Effects[note.Type().getEffectId()];
        effecter.Effect(judge,step,note);
    }
    public static void reset()
    {
        foreach(var i in Effects)
        {
            i.Value.Clear();
        }
    }

}
public class RenderInfo
{
    public Vector3 position;
    public bool flipX = true;
    public Vector3 size = Vector3.one;
    public Color color = new Color(1F,1F,1F);
    public Quaternion rotation = Quaternion.identity;
    public RenderInfo(Vector3 pos, Vector3 size, bool flip = true)
    {
        this.size = size;
        flipX = flip;
        position = pos;
    }
    public RenderInfo()
    {

    }
}
public enum EffectDataType {BUILT_IN, CUSTOM}
public static class ImageJournal
{
    public static Image[] loadedImages;
    public static void loadAll()
    {
        // 대충 채보파일로부터 이미지 읽어오는 코드    
    }
    public static GameObject getEffect(EffectDataType data_type, int code)
    {
        return GameObject.Instantiate(Resources.Load("Prefab/NoteEffects/BuiltInEffect"+code) as GameObject);
    }
}
public interface NoteEffect
{
    void Begin(RenderInfo info);
    void On(RenderInfo info);
    void End(RenderInfo info);
    void Stop();
}
public class EffectPool
{
    Dictionary<Note,ParticleNoteEffectMerger> dict;
    Queue<ParticleNoteEffectMerger> pool;
    int cnt;
    ParticleNoteEffectMerger Get(Note note)
    {
        Debug.Assert(dict.Count + pool.Count == cnt);
        if(!dict.ContainsKey(note))
        {
//            Debug.Log("POP:" + note.data.time);
            dict[note] = pool.Dequeue();
        }
        return dict[note];
    }
    void Release(Note note)
    {
        Debug.Assert(dict.Count + pool.Count == cnt);
        if (!dict.ContainsKey(note))
        {
            Debug.LogWarning("오브젝트 Release를 실패했습니다");
        }
        else
        {
//            Debug.Log("Push:"+note.data.time);
            pool.Enqueue(dict[note]);
            dict.Remove(note);
        }
    }
    public EffectPool(GameObject prefab, int count)
    {
        cnt = count;
        pool = new Queue<ParticleNoteEffectMerger>();
        dict = new Dictionary<Note,ParticleNoteEffectMerger>();
        for(int i = 0; i < count; i++)
        {
            var temp = GameObject.Instantiate(prefab);
            pool.Enqueue(temp.GetComponent<ParticleNoteEffectMerger>());
        }
    }
    public void Effect(JudgeType judge,NoteStep step , Note note)
    {
        var info = new RenderInfo()
        {
            position = note.Position(Game.time-note.data.time),
            color = Color.white,
            rotation = note.Rotation(),
            size = new Vector3(note.Scale().x,1,1)
        };
        switch (step)
        {
            case NoteStep.BEGIN:
                Get(note).SetEffect(judge, step, info);
                break;
            case NoteStep.ON:
                if (dict.ContainsKey(note))
                    dict[note].SetEffect(judge, step, info);
                break;
            case NoteStep.END:
                Get(note).SetEffect(judge, step, info);
                Release(note);
                break;
        }
    }
    public void Clear()
    {
        foreach(var i in dict)
        {
            i.Value.StopAll();
            pool.Enqueue(i.Value);
        }
        dict.Clear();
        
    }
}