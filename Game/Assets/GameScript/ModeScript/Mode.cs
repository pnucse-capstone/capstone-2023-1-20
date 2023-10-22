using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract class Mode
{
    static Mode[] modes = { new CatchMode(), new RandomizeY(), new RandomizeSpeed(), new Vertical(), new ViewBar(), new oneSided() , new hardJudge(), new testMode() ,new fullBar(),new RandomLR(),new ToClick(),new SpeedUp(),new MirrorLR(),new RemoveLong(), new SlowNote(), new FastNote(), new SpeedDown(), new InLineMode()};
    public static Mode getMode(int mode_id) 
    {
        return modes[mode_id];
    }
    public static bool isUnlock(int mode_id)
    {
        return (PlayerPrefs.GetInt("modeUnlock"+mode_id, 0)==1);
    }
    public static void unlock(int mode_id)
    {
        PlayerPrefs.SetInt("modeUnlock" + mode_id,1);
    }
    public virtual void Awake() { }
    public virtual void Start() { }
    public virtual void Update() { }
}
class CatchMode : Mode
{
    public override void Start()
    {
        List<Note> l = Game.table.getAllNotes((x) => true);
        foreach (Note i in l)
        {
//            i.data.need_input = NoteType.CONTACT;
        }
    }
}

class RandomizeY : Mode
{
    public override void Start()
    {
        List<Note> l = Game.table.getAllNotes((x) => true);
        foreach (Note i in l)
        {
            i.data.y = Random.Range(-0.9F,0.9F);
            i.data.y = Mathf.Clamp(i.data.dy + i.data.y, -0.9F, 0.9F)-i.data.dy;
        }
    }
}


class RandomizeSpeed: Mode
{
    public override void Start()
    {
        List<Note> l = Game.table.getAllNotes((x) => true);
        foreach (Note i in l)
        {
            i.data.vx = Random.Range(0.5F, 1F) *(i.data.vx > 0?1:-1);

//            if(i.data.need_input==InputType.LEFT || i.data.need_input==InputType.RIGHT)i.data.need_input = i.data.vx < 0 ? InputType.LEFT : InputType.RIGHT;
            
        }
        Game.table.setNoteWindowMode(NotesWindow.isNear);
    }
}


class Vertical : Mode
{
    public override void Start()
    {
        Camera.main.transform.Rotate(new Vector3(0, 0, 90));
//        Game.Controller = VerticalController;
    }
    void VerticalController()
    {
        Game.cursor.y += Input.GetAxis("Mouse X") * Game.mouse_sens;
        var h = Camera.main.orthographicSize;
        Game.cursor.y = Mathf.Clamp(Game.cursor.y, -h, h);
    }
}

class ViewBar : Mode
{
    public override void Start()
    {
    }
    public override void Update()
    {
        var temp= Camera.main.transform.position;
        temp.y = Mathf.Lerp(temp.y,Game.cursor.y,Time.deltaTime);
        temp.z = -10;
        Camera.main.transform.position = temp;
    }
}


class oneSided: Mode
{
    public override void Start()
    {
        List<Note> l = Game.table.getAllNotes((x) => true);
        foreach (Note i in l)
        {
            if (i.data.vx > 0) i.data.y = Mathf.Clamp(i.data.y + 0.05F, -1, 1);
            else i.data.y = i.data.y = Mathf.Clamp(i.data.y - 0.05F, -1, 1);
            i.data.vx = -Mathf.Abs(i.data.vx );
        }
        Game.oneSided = true;
        Game.isStereo = false;
        Camera.main.transform.position = new Vector3(-0.9F*Camera.main.orthographicSize,0,-10);
    }
    public override void Update()
    {
    }
}
class hardJudge : Mode
{
    public override void Start()
    {
//        NoteCondition.contactDt /= 2;
    }
    public override void Update()
    {
    }
}
class testMode : Mode
{
    public override void Awake()
    {
//        NoteCondition.contactDt = new JudgeDt(0.075F,0.075F);
    }
    public override void Update()
    {
    }
}
class fullBar : Mode
{

    public override void Update()
    {
//        Game.judge_y = 10;
    }
}
class RandomLR : Mode
{
    public override void Start()
    {
        List<Note> l = Game.table.getAllNotes((x) => true);
        for(int i=1;i<l.Count-1;i++)
        {
            if(l[i-1].data.time!= l[i].data.time && l[i + 1].data.time != l[i].data.time)
            {
                l[i].data.vx *= Random.Range(0, 2)==0?1:-1;
//                if(l[i].data.need_input == NoteType.NORMAL || l[i].data.need_input== NoteType.RIGHT) 
  //                  l[i].data.need_input = l[i].data.vx < 0 ? NoteType.NORMAL : NoteType.RIGHT;
            }
        }
    }
}
class ToClick : Mode
{
    public override void Start()
    {
        List<Note> l = Game.table.getAllNotes((x) => true);
        foreach (Note i in l)
        {
//            i.data.need_input = i.data.vx<0?NoteType.NORMAL:NoteType.RIGHT;
        }
    }
}
class SpeedUp:Mode
{
    public override void Start()
    {
        Game.playSpeed *= 1.5F;
    }

}
class MirrorLR : Mode
{
    public override void Start()
    {
        List<Note> l = Game.table.getAllNotes((x) => true);
        foreach (Note i in l)
        {
            i.data.vx = -i.data.vx;
//            if(i.data.need_input == NoteType.NORMAL || i.data.need_input == NoteType.RIGHT)i.data.need_input = i.data.vx < 0 ? NoteType.NORMAL : NoteType.RIGHT;
        }
    }
}
class RemoveLong : Mode
{
    public override void Awake()
    {
        List<Note> l = Game.table.getAllNotes((x) => true);
        foreach (Note i in l)
        {
            i.data.length = 0F;
        }
    }
}
class SpeedDown : Mode
{
    public override void Start()
    {
        Game.playSpeed /= 1.5F;
    }

}
class SlowNote : Mode
{
    public override void Start()
    {
        List<Note> l = Game.table.getAllNotes((x) => true);
        foreach (Note i in l)
        {
            i.data.vx /=2F;
        }
    }
}
class FastNote : Mode
{
    public override void Start()
    {
        List<Note> l = Game.table.getAllNotes((x) => true);
        foreach (Note i in l)
        {
            i.data.vx *= 2F;
        }
    }
}
class InLineMode: Mode
{
    public override void Start()
    {
        List<Note> l = Game.table.getAllNotes((x) => true);
        foreach (Note i in l)
        {
            i.data.y = 0F;
            i.data.dy = 0F;
        }
    }
}