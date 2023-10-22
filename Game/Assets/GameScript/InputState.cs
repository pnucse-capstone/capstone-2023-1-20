using Async;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InputState
{
    public enum State { ON, OFF, DOWN, UP }
    public float time { get; }
    public bool isOn(int line);
    public bool isBegin(int line);
    public void Start();
    public void Update();
    public bool NextState(); // 다음 상태가 없을때 false 반환
    void Release();
}
public class InputStateAccurate :InputState

{
    InputState.State[] states;
    bool[] on;
    bool[] down;
    InputMap mapper;
    float _time = 0;
    public float time
    {
        get
        {
            return _time;
        }
    }

    public InputStateAccurate(InputMap mapper)
    {
        Debug.Log("Async Input");

        this.mapper = mapper;
        on = new bool[512];
        down = new bool[512];
        states = new InputState.State[512];
        inputs.Clear();
        AsyncInput.Reset();

    }


    public bool isBegin(int line)
    {
        return down[(int)mapper.mappedKey(line)];
    }

    public bool isOn(int line)
    {
        return on[(int)mapper.mappedKey(line)];
    }

    public void Start()
    {
        Debug.Log("Start");
        AsyncInput.Init();
        for (int i = 0; i < Game.lineCount; i++)
        {
            AsyncInput.Register(User32.ToVirtualKey(mapper.mappedKey(i)));
        }
        AsyncInput.Resume();

    }
    bool isCheck = false;
    Queue<AsyncInput.KeyInput> inputs = new Queue<AsyncInput.KeyInput>();
    public void Update()
    {
        isCheck = true;
        var inputs = AsyncInput.GetInputs();
        foreach (var i in inputs)
        {
//            Debug.Log("inputs:"+inputs.Length);
            this.inputs.Enqueue(i);
        }
    }

    public bool NextState()
    {
        for (int i = 0; i < 512; i++)
        {
            down[i] = false;
        }
        if (isCheck)
        {
            isCheck = false;
            _time = Game.time;
            return true;
        }
        else if (inputs.Count == 0) return false;
        var now = inputs.Dequeue();
        _time = now.time;
        var key = (int)User32.ToKeyCode(now.key);
        if (now.state == AsyncInput.KeyInput.DOWN)
        {
//            Debug.Log((key,now.state));
            on[key] = true;
            down[key] = true;
        }
        else if (now.state == AsyncInput.KeyInput.UP)
        {
//            Debug.Log((key, now.state));
            on[key] = false;
            down[key] = false;
        }
        else
        {
            down[key] = false;
        }
        return true;
    }
    public void Release()
    {
        AsyncInput.Suspend();
    }
}
public class InputStateLagacy : InputState
{
    InputState.State[] states;
    InputMap mapper;

    public float time => _time;
    float _time = 0;
    public InputStateLagacy(InputMap mapper)
    {
        Debug.Log("Legacy Input");

        this.mapper = mapper;
        states = new InputState.State[512];
    }
    public bool isOn(int line)
    {
        if (line == -1) return true;
        var state = states[(int)mapper.mappedKey(line)];
        return state == InputState.State.DOWN || state == InputState.State.ON;
    }
    public bool isBegin(int line)
    {
        if (line == -1) return true;
        var state = states[(int)mapper.mappedKey(line)];
        return state == InputState.State.DOWN;
    }

    public void Start()
    {
        for (int i = 0; i < Game.lineCount; i++)
        {
            KeyCode key = mapper.mappedKey(i);
            states[(int)key] = InputState.State.OFF;
        }

    }
    bool check = false;
    public void Update()
    {
        check = true;
    }

    public bool NextState()
    {
        if (!check) return false;
        _time = Game.renderTime ;
        for (int i = 0; i < Game.lineCount; i++)
        {
            KeyCode key = mapper.mappedKey(i);
            if (Input.GetKeyDown(key))
            {
                states[(int)key] = InputState.State.DOWN;
            }
            else if (Input.GetKey(key))
            {
                states[(int)key] = InputState.State.ON;
            }
            else if (Input.GetKeyUp(key))
            {
                states[(int)key] = InputState.State.UP;
            }
            else
            {
                states[(int)key] = InputState.State.OFF;
            }
        }
        check = false;
        return true;
    }

    public void Release()
    {

    }

}