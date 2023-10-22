using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Profiling;

using UnityEngine;
using UnityEngine.Profiling;

public class LuaEngine :MonoBehaviour
{
    public static string LUA_PATH 
    { 
        get => Application.dataPath + "/script.lua";    
    }
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void DebugLog(string log);

    private static readonly DebugLog debugLog = DebugWrapper;
    private static readonly IntPtr functionPointer = Marshal.GetFunctionPointerForDelegate(debugLog);

    private static void DebugWrapper(string log)
    {
        Debug.Log("lua"+log);
    }

    [DllImport("LuajitWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void LinkDebug([MarshalAs(UnmanagedType.FunctionPtr)] IntPtr debugCal);

    public static void SetUpDebug()
    {
        DebugLog debug;
        debug = DebugWrapper;
        IntPtr ptr = Marshal.GetFunctionPointerForDelegate(debug);
        LinkDebug(ptr);
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public double x;
        public double y;
        public double z;
        public double w;
    }
    [DllImport("LuajitWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool Load(string url);
    [DllImport("LuajitWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void NewTable();
    [DllImport("LuajitWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void AddTablePair(string key, double value);
    [DllImport("LuajitWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void AddTableValue(int index, double value);
    [DllImport("LuajitWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void PushNumber(double value);

    [DllImport("LuajitWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void SetGlobalTable(string name);
    [DllImport("LuajitWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void SetGlobal(string name, double value);
    [DllImport("LuajitWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern Point Call(string name, int cnt_result);
    [DllImport("LuajitWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int CallBoolean(string name);
    [DllImport("LuajitWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern double GetGlobal(string name);

    [DllImport("LuajitWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern double GetTableMember(string name, int index);

    [DllImport("LuajitWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int getTestNumber();


    [DllImport("LuajitWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int checkStack(int sz);

    [DllImport("LuajitWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int getTop();


    [DllImport("LuajitWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int ErrorCode();

    void Awake()
    {
        SetUpDebug();
        //        AssetModificationProcessor(Application.persistentDataPath + "/test.lua");
        Debug.Log(Load(LuaEngine.LUA_PATH));
        Debug.Log(getTop());
    }
    void Start()
    {
        Update();
    }
    public int GetLineCnt()
    {
        int line = Mathf.RoundToInt((float)GetGlobal("_line_cnt"));
        Game.lineCount = line;
        return line;
    }
    public void ApplyInputKeySetting(InputMap inputmap)
    {
        Game.lineCount = GetLineCnt();
        for (int i = 1; i <= GetLineCnt(); i++)
        {
            int keycode = (int)GetTableMember("_keys", i);
            if(keycode>=65 && keycode <= 90)
            {
                keycode += (97-65);
            }
            KeyCode key = (KeyCode)keycode;
            Debug.Log("Map:"+key);
            inputmap.Map(key,i-1);
        }
    }
    void Update()
    {

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        NewTable();
        AddTablePair("x", pos.x);
        AddTablePair("y", pos.y);
        SetGlobalTable("_mouse");
    }
    public void SetData(Note note, GameState state)
    {
        NewTable();
        AddTablePair("x",note.data.x);  
        AddTablePair("y", note.data.y);
        AddTablePair("z", note.data.z);
        SetGlobalTable("_note");

        NewTable();
        for(int i=0;i<6; i++)
        {
            AddTableValue(i+1, state.properties.position[i]);
        }
        SetGlobalTable("_position");

        SetGlobal("_speed",state.properties.speed);
        SetGlobal("_width", state.properties.width);
        SetGlobal("_speed", state.properties.speed);
        SetGlobal("_vx", note.data.vx);

        SetGlobal("_input_line", 0);
    }
    public void SetInputLine(int line)
    {
        SetGlobal("_input_line", line);
    }
    public void SetDistance(float distance)
    {
        SetGlobal("_distance", distance);
    }
    static readonly ProfilerMarker positionMarker= new ProfilerMarker("Position");
    public Vector3 Position(Note note, GameState state, float dist)
    {

        positionMarker.Begin();
        SetData(note,state);
        SetGlobal("_distance", dist);
        var p = Call("Position",3);
        positionMarker.End();
        return new Vector3((float)p.x, (float)p.y, (float)p.z);
    }

    public Vector3 LinePosition(Note note, GameState state, float dist)
    {

        SetData(note, state);
        SetGlobal("_distance", dist);
        var p = Call("LinePosition", 3);
        return new Vector3((float)p.x, (float)p.y, (float)p.z);
    }
    public bool Condition(Note note, GameState state, int input_line)
    {
        SetData(note, state);
        SetGlobal("_input_line", input_line);
        var p = CallBoolean("Condition");
        return p ==1;
    }

    public Vector3 Scale(Note note, GameState state, float dist)
    {
        SetData(note, state);
        SetGlobal("_distance", dist);
        var p = Call("Scale", 3);
        return new Vector3((float)p.x, (float)p.y, (float)p.z);
    }

    public Vector3 LineScale(Note note, GameState state, float dist)
    {
        SetData(note, state);
        SetGlobal("_distance", dist);
        var p = Call("LineScale", 3);
        return new Vector3((float)p.x, (float)p.y, (float)p.z);
    }
    public bool IsSoft(Note note, GameState state)
    {
        SetData(note, state);
        var p = CallBoolean("IsSoft");
        return p == 1;
    }

    public Vector3 CameraPosition(GameState state)
    {
        var p = Call("CameraPosition", 3);
        return new Vector3((float)p.x, (float)p.y, (float)p.z);
    }
}
