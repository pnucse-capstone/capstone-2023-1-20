using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class NoteType
{
    abstract public int getEffectId();
    public readonly string name;
    public JudgeInfo judge_info;
    public bool firstonly = true;
    public float bound_left = 10;
    public float bound_right = 1;

    abstract public bool Condition(GameState state, Note data, int input_line); // 판정 조건을 정의
    abstract public bool IsSoft { get; }
    abstract public int imageCode();
    public NoteType(string name, string default_sprite)
    {

        this.name = name;
        judge_info = new JudgeInfo();
    }
    public static int Cmp(Note x, Note y)
    {
        return Cmp(x.data, y.data);
    }

    public static int Cmp(NoteData x, NoteData y)
    {
        if (Cmp(x.time, y.time) != 0)
        {
            return Cmp(x.time, y.time);
        }
        if (Cmp(x.x, y.x) != 0)
        {
            return Cmp(x.x, y.x);
        }

        if (Cmp(x.y, y.y) != 0)
        {
            return Cmp(x.y, y.y);
        }
        if (Cmp(x.dx, y.dx) != 0)
        {
            return Cmp(x.dx, y.dx);
        }
        return Cmp(x.length, y.length);
    }

    private static int Cmp(float x, float y)
    {
        if (Mathf.Abs(x - y) < 0.001) return 0;
        return Math.Sign(x - y);
    }
    abstract public LinePath GetLinePath(GameState state, int input_line);
    abstract public Vector3 LineCenterScale(GameState state, int input_line);
    virtual public Vector3 CameraPosition(GameState state)
    {
        return Vector3.zero;
    }
    abstract public Vector3 Scale(GameState state, Note note);
    abstract public Quaternion Rotation(GameState state, Note note);
    abstract public Color NoteColor(GameState state, Note note);
    abstract public Color LongNoteColor(GameState state, Note note, float t);
    abstract public float LongNoteWidth(GameState state, Note note);
    abstract public bool Range(GameState state, Note note);
    abstract public Vector3 Position(GameState state, Note note, float offset);
    abstract public LinePath.Point[] LongNotePositions(GameState state, Note note);

    public bool InTimeBoundary(Note note)
    {
        return note.data.time - bound_left < Game.renderTime && note.data.time + note.data.length + bound_right > Game.renderTime;
    }
}

public class NormalNoteType : NoteType
{
    public override bool IsSoft => false;

    override public int getEffectId()
    {
        return 4;
    }
    public NormalNoteType() : base("Normal", "TEST")
    {
        judge_info.Add(JudgeType.perfect, new JudgeDt(-0.04F, 0.04F));
        judge_info.Add(JudgeType.good, new JudgeDt(-0.07F, 0.07F));
        judge_info.Add(JudgeType.ok, new JudgeDt(-0.11F, 0.11F));
        judge_info.Add(JudgeType.miss, new JudgeDt(-0.13F, 0.13F));
    }
    public override bool Condition(GameState state, Note note, int input_line)
    // 세가지: 1. 입력 라인 2. 게임 상태, 3. 시간 조건
    // 노트 판정 조건 >> ?? BEGIN ON END  리턴값을 이거로 주던가,
    // 이 조건을 이용해서 무언가 입력에 대한 객체를 만들거나.
    //y는 라인, x는 너비.
    {
        float left = note.data.y;
        float right = note.data.y + note.data.x;
        return left <= input_line && input_line <= right;
    }

    public override Vector3 Position(GameState state, Note note, float offset)
    {
        float distance = GetDistance(note.data.time + offset) * note.data.vx;
        var path = NotePath(state, note);
        var point = path.Get(distance);
        if (point.t >= path.maxTime) return new Vector3(990429, 990429);
        else return point.ToVector3();
    }

    private static float GetDistance(float time)
    {
        var distance = Game.state.timedistance.TimeToDistance(time) - Game.state.timedistance.TimeToDistance(Game.renderTime);
        distance *= 20;
        distance *= Game.state.properties.speed;
        distance *= Game.speed_multiplier;

        distance /= Game.playSpeed;
        return distance + Game.judgepoint_offset;
    }
    List<LinePath.Point> points = new List<LinePath.Point>();
    public override LinePath.Point[] LongNotePositions(GameState state, Note note)
    {
        var headTime = GetDistance(note.data.time + note.state.length) * note.data.vx;
        var tailTime = GetDistance(note.data.time + note.data.length) * note.data.vx;
        NotePath(state, note).Slice(headTime, tailTime, ref points);
        return points.ToArray();
    }
    Dictionary<int, LinePath.Point[]> linepathPool = new Dictionary<int, LinePath.Point[]>();
    private LinePath NotePath(GameState state, Note note) // time = 노트 시간일때 기준으로 거리
    {
        int line = (int)note.data.y;
        var origin = Vector3.right * (note.data.y - 2.5F + note.data.x / 2F) * state.properties.width;
        var offset = new Vector3(
            state.properties.position[line]
            + state.properties.notePosX[(int)note.data.y]
            , state.properties.positionY[line], 0);
        var notepos = new Vector3(note.data.posx, note.data.posy);
        Quaternion rotation = Quaternion.Euler(0, 0, Game.state.properties.rotation[line]);
        Matrix4x4 matrix = Matrix4x4.Rotate(rotation);

        LinePath path = state.properties.notePath[line];

        /*LinePath.Point 어레이 풀링 이유는 가비지*/
        LinePath next = CreateLinePathFromPool(path.points.Length);
        //여기까지

        for (int i = 0; i < path.points.Length; i++)
        {
            var p = matrix.MultiplyPoint3x4(path.points[i].ToVector3()) + offset + origin + notepos;
            next.points[i] = new LinePath.Point(path.points[i].t, p.x, p.y);
        }
        return next;
    }
    public LinePath CreateLinePathFromPool(int pointCount)
    {
        LinePath next = new LinePath();
        if (linepathPool.ContainsKey(pointCount))
        {
            next.points = linepathPool[pointCount];
        }
        else
        {
            next.points = new LinePath.Point[pointCount];
            linepathPool.Add(pointCount, next.points);

        }
        return next;
    }
    public override LinePath GetLinePath(GameState state, int input_line)
    {
        var path = state.properties.linePath[input_line];

        var offset = new Vector3(state.properties.position[input_line], state.properties.positionY[input_line], 0);
        var origin = Vector3.right * (input_line - 2.5F) * state.properties.width;

        Quaternion rotation = Quaternion.Euler(0, 0, Game.state.properties.rotation[input_line]);
        Matrix4x4 matrix = Matrix4x4.Rotate(rotation);

        LinePath next = CreateLinePathFromPool(path.points.Length);
        for (int i = 0; i < path.points.Length; i++)
        {
            var p = matrix.MultiplyPoint3x4(path.points[i].ToVector3()) + offset + origin;
            next.points[i] = new LinePath.Point(path.points[i].t, p.x, p.y);
        }
        return next;
    }
    public override Vector3 Scale(GameState state, Note note)
    {
        int line = (int)note.data.y;
        var unitLineWidth = state.properties.width * (note.data.x + 1) * state.properties.widths[(int)note.data.y];
        var x = unitLineWidth * Game.state.properties.noteWidth[line] * note.data.scalex - 0.12F;
        var y = Game.state.properties.noteHeight[line] * note.data.scaley;
        if (Game.thickmode && !Game.isEditor) y *= 2F;
        var z = y;
        var scale = new Vector3(
            Mathf.Max(0, x)
            , y
            , z);
        return scale;
    }
    public override Vector3 LineCenterScale(GameState state, int input_line)
    {
        int line = input_line;
        var unitLineWidth = state.properties.width * state.properties.widths[input_line];
        var x = unitLineWidth;
        var y = 1F;
        if (Game.thickmode && !Game.isEditor) y *= 2F;
        var z = Mathf.Max(0, unitLineWidth - 0.04F);
        var scale = new Vector3(
            Math.Max(0, x * Game.state.properties.lineCenterWidth[line] - 0.04F)
            , y * Game.state.properties.lineCenterHeight[line]
            , z * Game.state.properties.lineWidth[line]);
        return scale;
    }
    public override Quaternion Rotation(GameState state, Note note)
    {
        return Quaternion.Euler(0, 0, state.properties.rotation[(int)note.data.y]);
    }
    public override Color LongNoteColor(GameState state, Note note, float t)
    {
        Color start, end;
        if (note.data.useDefaultColor || Game.reduced)
        {
            if (GameInit.colors.presetName == "Default" && false)
            {
                NoteColorMap prev, next;
                float tt;
                GetColorState(out prev, out next, out tt);

                start = Color.Lerp(prev.GetLongStart(note.data.x, (int)note.data.y), next.GetLongStart(note.data.x, (int)note.data.y), tt);
                end = Color.Lerp(prev.GetLongEnd(note.data.x, (int)note.data.y), next.GetLongEnd(note.data.x, (int)note.data.y), tt);
            }
            else
            {
                NoteColorMap prev, next;
                prev = next = GameInit.colors;
                start = prev.GetLongStart(note.data.x, (int)note.data.y);
                end = prev.GetLongEnd(note.data.x, (int)note.data.y);
            }
        }
        else
        {
            start = (Color)note.data.lineColorStart;
            end = (Color)note.data.lineColorEnd;
        }
        Color s = start * state.properties.lineColorM + state.properties.lineColorA;
        Color e = end * state.properties.lineColorM + state.properties.lineColorA;
        if (note.state.length != 0)
        {
            float m = GameInit.colors.multiplier;
            //            if (Game.isEditor) m = 1;
            Color twinkle = Color.white * 0.03F * Mathf.Sin(note.state.length * 30);
            s += Color.white * 0.4F * m + Color.white * 0.2F * Mathf.InverseLerp(0.2F, 0, note.state.length) + twinkle;
            e += Color.white * 0.2F * m + twinkle;
        }
        else if (note.isPassed())
        {
            s *= 0.7F;
            e *= 0.7F;
        }
        return t * e + (1 - t) * s;
    }

    public override float LongNoteWidth(GameState state, Note note)
    {
        int line = (int)note.data.y;
        var unitLineWidth = state.properties.width * (note.data.x + 1) * state.properties.widths[(int)note.data.y];
        var x = unitLineWidth * Game.state.properties.noteWidth[line] * note.data.scalex - 0.04F;
        return x;
    }
    public override Color NoteColor(GameState state, Note note)
    {
        Color c = note.data.color;

        if (note.data.useDefaultColor || Game.reduced) c = GetDefaultColor(note);
        if (!Game.reduced)
        {
            c = c * Game.state.properties.multiplyNoteColor + Game.state.properties.additiveNoteColor;
        };
        c.a = 1;
        return c;
    }
    public Color GetDefaultColor(Note note)
    {
        var colorset = GameInit.colors;
        if (Game.isEditor && false)
        {
            NoteColorMap prev, next;
            float t;
            GetColorState(out prev, out next, out t);
            var prev_color = prev.Get(note.data.x, (int)note.data.y);
            var next_color = next.Get(note.data.x, (int)note.data.y);
            return Color.Lerp(prev_color, next_color, t);
        }
        else
        {
            return colorset.Get(note.data.x, (int)note.data.y);
        }
    }

    private static void GetColorState(out NoteColorMap prev, out NoteColorMap next, out float t)
    {
        prev = GameInit.GetColorPreset(Mathf.RoundToInt(Game.state.properties.noteColorset[0]));
        next = GameInit.GetColorPreset(Mathf.RoundToInt(Game.state.properties.noteColorset[1]));
        t = Game.state.properties.noteColorset[2];
    }

    public override bool Range(GameState state, Note note)
    {
        return note.isInScreen();
    }

    public override int imageCode()
    {
        return Mathf.RoundToInt(Game.state.properties.noteImage[0]);
    }
}


public class CatchNoteType : NoteType
{
    public override bool IsSoft => true;

    override public int getEffectId()
    {
        return 4;
    }
    public CatchNoteType() : base("Normal", "TEST")
    {
        judge_info.Add(JudgeType.perfect, new JudgeDt(-0.04F, 0.04F));
    }
    public override bool Condition(GameState state, Note note, int input_line)
    {
        return note.data.y == input_line;
    }

    public override Vector3 Position(GameState state, Note note, float offset)
    {
        float distance = GetDistance(note.data.time + offset) * note.data.vx;
        return NotePath(state, note).Get(distance).ToVector3();
    }

    private static float GetDistance(float time)
    {
        var distance = Game.state.timedistance.TimeToDistance(time) - Game.state.timedistance.TimeToDistance(Game.renderTime);
        distance *= 20;
        distance *= Game.state.properties.speed;
        distance *= Game.speed_multiplier;
        return distance;
    }

    public override LinePath.Point[] LongNotePositions(GameState state, Note note)
    {
        var headTime = GetDistance(note.data.time) * note.data.vx;
        var tailTime = GetDistance(note.data.time + note.data.length) * note.data.vx;
        return NotePath(state, note).Slice(headTime, tailTime).GetPoints();
    }
    private LinePath NotePath(GameState state, Note note) // time = 노트 시간일때 기준으로 거리
    {
        int line = (int)note.data.y;
        var origin = Vector3.right * (note.data.y - 2.5F) * state.properties.width;
        var offset = new Vector3(
            state.properties.position[line]
            + state.properties.notePosX[(int)note.data.y]
            , state.properties.positionY[line], 0);
        var notepos = new Vector3(note.data.posx, note.data.posy);
        Quaternion rotation = Quaternion.Euler(0, 0, Game.state.properties.rotation[line]);
        Matrix4x4 matrix = Matrix4x4.Rotate(rotation);

        LinePath path = state.properties.notePath[line];
        LinePath next = new LinePath();
        next.points = new LinePath.Point[path.points.Length];

        for (int i = 0; i < path.points.Length; i++)
        {
            var p = matrix.MultiplyPoint3x4(path.points[i].ToVector3()) + offset + origin + notepos;
            next.points[i] = new LinePath.Point(path.points[i].t, p.x, p.y);
        }
        return next;
    }

    public override LinePath GetLinePath(GameState state, int input_line)
    {
        var path = state.properties.linePath[input_line];

        var offset = new Vector3(state.properties.position[input_line], state.properties.positionY[input_line], 0);
        var origin = Vector3.right * (input_line - 2.5F) * state.properties.width;

        Quaternion rotation = Quaternion.Euler(0, 0, Game.state.properties.rotation[input_line]);
        Matrix4x4 matrix = Matrix4x4.Rotate(rotation);

        LinePath next = new LinePath();
        next.points = new LinePath.Point[path.points.Length];
        for (int i = 0; i < path.points.Length; i++)
        {
            var p = matrix.MultiplyPoint3x4(path.points[i].ToVector3()) + offset + origin;
            next.points[i] = new LinePath.Point(path.points[i].t, p.x, p.y);
        }
        return next;
    }
    public override Vector3 Scale(GameState state, Note note)
    {
        return Vector3.one / 4;
    }
    public override Vector3 LineCenterScale(GameState state, int input_line)
    {
        int line = input_line;
        var unitLineWidth = state.properties.width * state.properties.widths[input_line];
        var x = unitLineWidth;
        var y = 1F;
        var z = Mathf.Max(0, unitLineWidth - 0.04F);
        var scale = new Vector3(
            Math.Max(0, x * Game.state.properties.lineCenterWidth[line] - 0.04F)
            , y * Game.state.properties.lineCenterHeight[line]
            , z * Game.state.properties.lineWidth[line]);
        return scale;
    }
    public override Quaternion Rotation(GameState state, Note note)
    {
        return Quaternion.Euler(0, 0, state.properties.rotation[(int)note.data.y]);
    }
    public override Color LongNoteColor(GameState state, Note note, float t)
    {

        Color s = note.data.lineColorStart;
        Color e = note.data.lineColorEnd;
        if (note.data.useDefaultColor)
        {
            s = e = Color.white;
        }
        s = s * state.properties.lineColorM + state.properties.lineColorA;
        e = e * state.properties.lineColorM + state.properties.lineColorA;
        if (note.state.length != 0)
        {
            s += Color.white * 0.2F;
        }
        else if (note.isPassed())
        {
            s *= 0.7F;
            e *= 0.7F;
        }
        return t * e + (1 - t) * s;
    }

    public override float LongNoteWidth(GameState state, Note note)
    {
        var pos = Scale(state, note);
        return pos.x;
    }
    public override Color NoteColor(GameState state, Note note)
    {
        Color c = note.data.color;
        c = c * Game.state.properties.multiplyNoteColor + Game.state.properties.additiveNoteColor;
        c.a = 1;
        return c;
    }

    public override bool Range(GameState state, Note note)
    {
        return note.isInScreen();
    }

    public override int imageCode()
    {
        return 1;
    }

}


