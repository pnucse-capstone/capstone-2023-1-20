using System.Linq;
using UnityEngine;

public interface MODI
{

    public enum Mode
    {
        None = 0, Random = 1, Mirror = 2, FourKeys = 3, SixKeys = 4, FourButtons = 5, HardJudge = 6, Backmask = 7, SuperRandom = 8, OneButton = 9, FourLines = 10,
        NoGimmick = 11
    }
    public void Modify(Table table);
    public string name
    {
        get;
    }
    public Mode code
    {
        get;
    }
}
public class MODINone : MODI
{
    public MODI.Mode code => MODI.Mode.None;
    public string name => "None";


    public void Modify(Table table)
    {
    }
}
public class MODIRandom : MODI
{

    public MODI.Mode code => MODI.Mode.Random;
    public string name => "Random";

    public void Modify(Table table)
    {
        new RandomizerDefault(0, 6).Convert(table);
        table.ApplySpecialMode((x) => { });
        table.RemoveOverlapped();
    }
}
public class MODISuperRandom : MODI
{

    public MODI.Mode code => MODI.Mode.SuperRandom;
    public string name => "Super Random";

    public void Modify(Table table)
    {
        table.getAllNotes(x => true).ForEach(x => x.data.x = Random.Range(0, 6));
        new RandomizerDefault(0, 6).Convert(table);
        table.ApplySpecialMode((x) => { });
        table.RemoveOverlapped();
    }
}
public class MODIMirror : MODI
{
    public MODI.Mode code => MODI.Mode.Mirror;
    public string name => "Mirror";

    public void Modify(Table table)
    {
        table.Apply((x) =>
        {
            x.data.y = 5 - x.data.y - x.data.x;
        });
    }
}

public class MODIHardJudge : MODI
{
    public MODI.Mode code => MODI.Mode.HardJudge;
    public string name => "Hard Judgement";

    public void Modify(Table table)
    {
        var judge_info = new JudgeInfo();
        Debug.Log("Hard Judgement");
        judge_info.Add(JudgeType.perfect, new JudgeDt(-0.02F, 0.02F));
        judge_info.Add(JudgeType.good, new JudgeDt(-0.035F, 0.035F));
        judge_info.Add(JudgeType.ok, new JudgeDt(-0.075F, 0.075F));
        judge_info.Add(JudgeType.miss, new JudgeDt(-0.2F, 0.2F));
        Game.mechanim.noteType(0).judge_info = judge_info;
    }
}
public class MODI4keys : MODI
{
    public MODI.Mode code => MODI.Mode.FourKeys;
    public string name => "Classic 4 keys";

    public void Modify(Table table)
    {
        table.Apply((x) => x.data.x = 0);
        new RandomizerDefault(1, 5).Convert(table);
        table.ApplySpecialMode(FirstEvent);
        table.RemoveOverlapped();

    }
    void FirstEvent(EventData e)
    {
        e.Use("widths", true);
        e.widths[0] = 0;
        e.widths[5] = 0;
    }
}

public class MODINoGimmick : MODI
{
    public MODI.Mode code => MODI.Mode.NoGimmick;
    public string name => "No Gimmick";

    public void Modify(Table table)
    {
        table.Apply(Clear);
        Game.state.timedistance = new TimeDistance(1);
    }
    public void Clear(EventData e)
    {
        e.Reset();
    }
}

public class MODI4lines : MODI
{
    public MODI.Mode code => MODI.Mode.FourLines;
    public string name => "4 keys";

    public void Modify(Table table)
    {

        table.Apply((x) => x.data.x = Mathf.Clamp(x.data.x, 0, 3));
        new RandomizerDefault(1, 5).Convert(table);
        table.ApplySpecialMode(FirstEvent);
        table.RemoveOverlapped();

    }
    void FirstEvent(EventData e)
    {
        e.Use("widths", true);
        e.widths[0] = 0;
        e.widths[5] = 0;
    }
}
public class MODI6keys : MODI
{
    public MODI.Mode code => MODI.Mode.SixKeys;
    public string name => "Classic 6 keys";

    public void Modify(Table table)
    {
        table.Apply((x) => x.data.x = 0);
        new RandomizerDefault(0, 6).Convert(table);
        table.ApplySpecialMode((x) => { });
        table.RemoveOverlapped();

    }
}
public class MODI4tunes : MODI
{
    public MODI.Mode code => MODI.Mode.FourButtons;
    public string name => "4+2 keys";

    public void Modify(Table table)
    {
        table.Apply((x) => x.data.x = 0);
        var l1 = new RandomizerDefault(0, 6).Convert(table.getAllNotes((x) => x.data.length != 0));
        var l2 = new RandomizerDefault(1, 5).Convert(table.getAllNotes((x) => x.data.length == 0));

        table.setNotes(l1.Concat(l2).Select(x => new Note(x)).ToList());
        new RandomizerDefault(1, 5).Convert(table.getAllNotes((x) => x.data.y >= 1 && x.data.y <= 4));

        table.getAllNotes((x) => x.data.y == 5 || x.data.y == 0).ForEach(x => x.data.time -= 0.002F);

        table.ApplySpecialMode(FirstEvent);
        table.Apply((x) =>
        {
            x.data.useDefaultColor = false;
            if (x.data.y == 0 || x.data.y == 5)
            {
                x.data.lineColorEnd = x.data.lineColorStart = x.data.color = Color.green;
            }
            else if (x.data.y == 1 || x.data.y == 3)
            {
                x.data.lineColorEnd = x.data.lineColorStart = x.data.color = Color.white;
            }
            else
            {
                x.data.lineColorEnd = x.data.lineColorStart = x.data.color = Color.cyan;

            }
        });
        table.RemoveOverlapped();
    }
    void FirstEvent(EventData e)
    {
        e.Use("position", true);
        e.position[0] = 1.5F;
        e.position[5] = -1.5F;
        e.Use("widths", true);
        e.widths[0] = 2;
        e.widths[5] = 2;
        e.Use("lineLength", true);
        e.lineLength[0] = 0;
        e.lineLength[5] = 0;
    }
}

public class MODIBackmask : MODI
{
    public MODI.Mode code => MODI.Mode.Backmask;
    public string name => "Reverse Play";

    public void Modify(Table table)
    {
        Game.reverse = true;
        //pcm.length + Game.table.offset + Game.table.length_offset
        var songlength = Game.pbrffdata.audio.GetAudio().GetPCM().length
            + table.offset;
        table.Apply((x) =>
        {
            x.data.time = songlength - x.data.time - x.data.length;
        });
        table.Apply((x) =>
        {
            x.time = songlength - x.time;
            x.Use("skip", false);
        });

    }
}

public class MODIOneButton : MODI
{

    public MODI.Mode code => MODI.Mode.OneButton;
    public string name => "Any Line";

    public void Modify(Table table)
    {
        Game.mechanim = GameMech.Create(GameMech.Type.ONEBUTTON);
        table.ApplySpecialMode((x) => { });
        table.RemoveOverlapped();
        table.Apply((e) =>
        {
            e.Use("lineLength", true);
            for (int i = 0; i < e.lineLength.Length; i++)
            {
                e.lineLength[i] = 0;
            }
        });
    }
}