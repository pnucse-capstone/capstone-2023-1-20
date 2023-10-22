using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorSound : MonoBehaviour
{
    static bool useHitsound = true;
    static bool useBGM = true;
    [SerializeField] Toggle toggle_hit;
    [SerializeField]Toggle toggle_bgm;
    public void SetHitsound(bool value)
    {
        prev_note = NoteEditor.tableNow.searchNote(Game.time);
        useHitsound = value;
    }
    public void SetBGM(bool value)
    {
        useBGM = value;
        MusicPlayer.Mute(!useBGM);
    }
    // Start is called before the first frame update
    Note prev_note;
    void Start()
    {
        toggle_bgm.isOn = useBGM;
        MusicPlayer.Mute(!useBGM);
        toggle_hit.isOn = useHitsound;
        prev_note = NoteEditor.tableNow.searchNote(Game.time);
    }
    float prev_time= 0;
    List<Note> prev_list = new List<Note>();
    Dictionary<Note, FMOD.Channel> chn = new Dictionary<Note, FMOD.Channel>();
    List<Note> select = new List<Note>();
    bool LineTest(float a, float b, float c, float d)
    {
        return !(b < c || d < a);
    }
    void Update()
    {   
        var list = NoteEditor.tableNow.getAllNotes((x)=> LineTest(Mathf.Min(prev_time, Game.time), Mathf.Max(prev_time, Game.time),x.data.time,x.data.time+x.data.length));
        
        if (useHitsound)
        {
            if (list.Count>6)
            {
                list.Clear();
            }
            foreach (var i in prev_list)
            {
                if (!list.Contains(i))
                {
                    var info =KeySoundPallete.GetSoundInfo(i.data.key);
                    chn.Remove(i);
                }
            }
            foreach (var i in list)
            {
                if (!prev_list.Contains(i))
                {
                    var ch = KeySoundPallete.FMODPlay(i.data.key);
                    chn.Add(i, ch);
                }
                
                select.Add(i);

            }
            prev_list.Clear();
            foreach(var i in select)
            {
                prev_list.Add(i);
            }
            select.Clear();
        }
        // 둘다 있음: prev_list 등록
        // 있다 사라짐: stop하고 삭제
        // 없다 생김: prev_list 등록, 소리 재생

        prev_time = Game.time;
//        source.mute = !useBGM;
    }
}
