using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using UnityEngine;

namespace Async{
    public static class AsyncInput
    {
        static Thread inputThread; 
        static List<User32.VirtualKey> keys = new List<User32.VirtualKey>();
        static ushort[] prevState = new ushort[512];
        static ConcurrentStack<KeyInput> buffer = new ConcurrentStack<KeyInput>();
        static bool isPaused = true;
        static GameStopwatch timer= new GameStopwatch();
        public static void Register(User32.VirtualKey key)
        {
            keys.Add(key);
        }
        public static void Suspend()
        {
            timer.Stop();
            UnityEngine.Debug.Log("Input Thread Abort");
            inputThread.Abort();
            inputThread = null;
            Clear();
        }
        public static void Init()
        {
            if (inputThread == null)
            {
                UnityEngine.Debug.Log("Input Thread Start");
                inputThread = new Thread(new ThreadStart(KeyStateFetch));
                timer.Reset();
                inputThread.Start();
                prevState= new ushort[512];
            }
            else
            {
                Reset();
            }
        }
        public static void Pause()
        {
            UnityEngine.Debug.Log("Input Pause");
            isPaused = true;

            buffer.Clear();

        }

        public static void Resume()
        {
            UnityEngine.Debug.Log("Input Resume");
            isPaused = false;
            timer.Start();
        }
        public static void Restart()
        {
            buffer.Clear();
            timer.Restart();
            for (int i = 0; i < 256; i++)
            {
                prevState[i] = 0x00;
            }
        }
        public static void Reset()
        {
            buffer.Clear();
            timer.Reset();
            for (int i = 0; i < 256; i++)
            {
                prevState[i] = 0x00;
            }
        }
        public static void Clear() //키까지 초기화함
        {
            UnityEngine.Debug.Log("Input Reset");
            keys.Clear();
            buffer.Clear();
            timer.Reset();
            for (int i = 0; i < 256; i++)
            {
                prevState[i] = 0x00;
            }
        }
        public static void Drop()
        {
            UnityEngine.Debug.Log("Input Reset");
            buffer.Clear();
            for (int i = 0; i < 256; i++)
            {
                prevState[i] = 0x00;
            }
        }

        public static KeyInput[] GetInputs() // 프레임마다 호출, 호출간 키입력 반환
        {
            var ret = buffer.ToArray();
            buffer.Clear();
            return ret;
        }
        static void KeyStateFetch()
        {
            while (true)
            {
                if (isPaused) 
                {
                    Thread.Sleep(1);
                    continue; 
                }
                for (int i = 0; i < keys.Count; i++)
                {
                    Check(keys[i]);
                }
                Thread.Sleep(1);

                //                Thread.Yield();
            }

        }
        static void Check(User32.VirtualKey key)
        {
            float time = Game.playSpeed*(float)timer.GetTime();
            ushort state = User32.GetAsyncKeyState((int)key);
            if (prevState[(int)key] == 0x0000 && state != 0x0000)//타건
            {
                //
                buffer.Push(new KeyInput(key, KeyInput.DOWN, time));
                // 키 DOWN
            }
            else //안눌림
            {
                if (prevState[(int)key] != 0x0000 && state == 0x0000)
                {
                    //
                    buffer.Push(new KeyInput(key, KeyInput.UP, time));
                    // 키 UP
                }
            }
            prevState[(int)key] = state;

        }

        internal static void SetTime(float start_time)
        {
            timer.SetTime(start_time/ Game.playSpeed);
        }

        internal static float GetTime()
        {
            return (float)timer.GetTime()*Game.playSpeed;
        }

        public struct KeyInput
        {
            public const int DOWN = 1;
            public const int UP = 0;
            public int state;
            public float time;
            public User32.VirtualKey key;
            public KeyInput(User32.VirtualKey key, int state, float time)
            {
                this.key = key;
                this.state = state;
                this.time = time;
            }
            public override string ToString()
            {
                return $"time:{time},state:{state:X},key:{key}";
            }
        }
    }
}
public static class WinMM
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
    [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod", SetLastError = true)]
    public static extern uint TimeBeginPeriod(uint uMilliseconds);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
    [DllImport("winmm.dll", EntryPoint = "timeEndPeriod", SetLastError = true)]
    public static extern uint TimeEndPeriod(uint uMilliseconds);

    [DllImport("ntdll.dll", SetLastError = true)]
    public static extern int NtSetTimerResolution(int DesiredResolution, bool SetResolution, out int CurrentResolution);
}
public static class User32
{

    static KeyCode[] vkeymap;
    static VirtualKey[] ukeymap;
    static User32()
    {
        vkeymap = new KeyCode[512];
        ukeymap = new VirtualKey[512];
        map(KeyCode.Backspace,VirtualKey.VK_BACK);
        map(KeyCode.Delete, VirtualKey.VK_DELETE);
        map(KeyCode.Tab, VirtualKey.VK_TAB);
        map(KeyCode.Clear, VirtualKey.VK_CLEAR);
        map(KeyCode.Return, VirtualKey.VK_RETURN);
        map(KeyCode.Pause,VirtualKey.VK_PAUSE);
        map(KeyCode.Escape, VirtualKey.VK_ESCAPE);
        map(KeyCode.Space, VirtualKey.VK_SPACE);

        for (var ukey = KeyCode.Keypad0; ukey <= KeyCode.Keypad9; ukey++)
        {
            ukeymap[(int)ukey] = (VirtualKey)(ukey - ((int)KeyCode.Keypad0 - (int)VirtualKey.VK_NUMPAD0));
        }
        for (var vkey = VirtualKey.VK_NUMPAD0; vkey <= VirtualKey.VK_NUMPAD9; vkey++)
        {
            vkeymap[(int)vkey] = (KeyCode)(vkey + ((int)KeyCode.Keypad0 - (int)VirtualKey.VK_NUMPAD0));
        }
        map(KeyCode.KeypadPeriod, VirtualKey.VK_DECIMAL);
        map(KeyCode.KeypadDivide, VirtualKey.VK_DIVIDE);
        map(KeyCode.KeypadMultiply, VirtualKey.VK_MULTIPLY);
        map(KeyCode.KeypadMinus, VirtualKey.VK_SUBTRACT);
        map(KeyCode.KeypadPlus, VirtualKey.VK_ADD);
        map(KeyCode.KeypadEnter, VirtualKey.VK_RETURN);
        map(KeyCode.KeypadEquals, VirtualKey.VK_OEM_PLUS);
        map(KeyCode.UpArrow, VirtualKey.VK_UP);
        map(KeyCode.DownArrow, VirtualKey.VK_DOWN);
        map(KeyCode.LeftArrow, VirtualKey.VK_LEFT);
        map(KeyCode.RightArrow, VirtualKey.VK_RIGHT);
        map(KeyCode.Insert, VirtualKey.VK_INSERT);
        map(KeyCode.Home, VirtualKey.VK_HOME);
        map(KeyCode.End, VirtualKey.VK_END);
        map(KeyCode.PageUp, VirtualKey.VK_PRIOR);
        map(KeyCode.PageDown, VirtualKey.VK_NEXT);

        for (var ukey = KeyCode.F1; ukey <= KeyCode.F15; ukey++)
        {
            ukeymap[(int)ukey] = (VirtualKey)(ukey - ((int)KeyCode.F1 - (int)VirtualKey.VK_F1));
        }
        for (var vkey = VirtualKey.VK_F1; vkey <= VirtualKey.VK_F15; vkey++)
        {
            vkeymap[(int)vkey] = (KeyCode)(vkey + ((int)KeyCode.F1 - (int)VirtualKey.VK_F1));
        }


        for (var ukey = KeyCode.A; ukey <= KeyCode.Z; ukey++)
        {
            ukeymap[(int)ukey] = (VirtualKey)(ukey - ('a'-'A'));
        }
        for (var vkey = VirtualKey.VK_A; vkey <= VirtualKey.VK_Z; vkey++)
        {
            vkeymap[(int)vkey] = (KeyCode)(vkey + ('a'-'A'));
        }


        for (var ukey = KeyCode.Alpha0; ukey <= KeyCode.Alpha9; ukey++)
        {
            ukeymap[(int)ukey] = (VirtualKey)(ukey);
        }
        for (var vkey = VirtualKey.VK_KEY_0; vkey <= VirtualKey.VK_KEY_9; vkey++)
        {
            vkeymap[(int)vkey] = (KeyCode)(vkey);
        }

        map(KeyCode.Comma, VirtualKey.VK_OEM_COMMA);
        map(KeyCode.Minus, VirtualKey.VK_OEM_MINUS);
        map(KeyCode.Period, VirtualKey.VK_OEM_PERIOD);
        map(KeyCode.Slash, VirtualKey.VK_OEM_2);
        map(KeyCode.Semicolon, VirtualKey.VK_OEM_1);
        map(KeyCode.Equals, VirtualKey.VK_OEM_PLUS);
        
        map(KeyCode.LeftBracket, VirtualKey.VK_OEM_4);
        map(KeyCode.Backslash, VirtualKey.VK_OEM_5);
        map(KeyCode.RightBracket, VirtualKey.VK_OEM_6);
        map(KeyCode.BackQuote, VirtualKey.VK_OEM_3);
        map(KeyCode.Numlock, VirtualKey.VK_NUMLOCK);
        map(KeyCode.CapsLock, VirtualKey.VK_CAPITAL);
        map(KeyCode.ScrollLock, VirtualKey.VK_SCROLL);
        map(KeyCode.RightShift, VirtualKey.VK_RSHIFT);
        map(KeyCode.LeftShift, VirtualKey.VK_LSHIFT);
        map(KeyCode.LeftControl, VirtualKey.VK_LCONTROL);
        map(KeyCode.RightControl, VirtualKey.VK_RCONTROL);
        map(KeyCode.LeftAlt, VirtualKey.VK_LMENU);
        map(KeyCode.RightAlt, VirtualKey.VK_RMENU);

        map(KeyCode.Quote, VirtualKey.VK_OEM_7);
        map(KeyCode.Help, VirtualKey.VK_HELP);
        map(KeyCode.Print, VirtualKey.VK_PRINT);

    }
    static void map(KeyCode ukey, VirtualKey vkey) 
    {
        ukeymap[(int)ukey] = vkey;
        vkeymap[(int)vkey] = ukey;
    }
    public static KeyCode ToKeyCode(VirtualKey key)
    {
        var ukey = vkeymap[(int)key];
        if(ukey == KeyCode.None)
        {
            throw new Exception($"가상 키랑 매핑되는 키코드가 없어요({key})");
        }
        return ukey;
    }
    public static VirtualKey ToVirtualKey(KeyCode key)
    {
        var vkey = ukeymap[(int)key];
        if (vkey == 0)
        {
            throw new Exception($"키코드랑 매핑되는 가상 키가 없어요({key})");
        }
        return vkey;
    }
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern ushort GetAsyncKeyState(int uType);
    public enum VirtualKey : ushort
    {
        /// <summary>
        /// This is an addendum to use on functions in which you have to pass a zero value to represent no key code
        /// </summary>
        VK_NO_KEY = 0,

        /// <summary>
        /// Left mouse button
        /// </summary>
        VK_LBUTTON = 0x01,

        /// <summary>
        /// Right mouse button
        /// </summary>
        VK_RBUTTON = 0x02,

        /// <summary>
        /// Control-break processing
        /// </summary>
        VK_CANCEL = 0x03,

        /// <summary>
        /// Middle mouse button (three-button mouse)
        /// </summary>
        /// <remarks>NOT contiguous with L and R buttons</remarks>
        VK_MBUTTON = 0x04,

        /// <summary>
        /// X1 mouse button
        /// </summary>
        /// <remarks>NOT contiguous with L and R buttons</remarks>
        VK_XBUTTON1 = 0x05,

        /// <summary>
        /// X2 mouse button
        /// </summary>
        /// <remarks>NOT contiguous with L and R buttons</remarks>
        VK_XBUTTON2 = 0x06,

        /* 0x07 : unassigned */

        /// <summary>
        /// BACKSPACE key
        /// </summary>
        VK_BACK = 0x08,

        /// <summary>
        /// TAB key
        /// </summary>
        VK_TAB = 0x09,

        /* 0x0A - 0x0B : reserved */

        /// <summary>
        /// CLEAR key
        /// </summary>
        VK_CLEAR = 0x0C,

        /// <summary>
        /// RETURN key
        /// </summary>
        VK_RETURN = 0x0D,

        /* 0x0E - 0x0F : unassigned */

        /// <summary>
        /// SHIFT key
        /// </summary>
        VK_SHIFT = 0x10,

        /// <summary>
        /// CONTROL key
        /// </summary>
        VK_CONTROL = 0x11,

        /// <summary>
        /// ALT key
        /// </summary>
        VK_MENU = 0x12,

        /// <summary>
        /// PAUSE key
        /// </summary>
        VK_PAUSE = 0x13,

        /// <summary>
        /// CAPS LOCK key
        /// </summary>
        VK_CAPITAL = 0x14,

        /// <summary>
        /// IME Kana mode
        /// </summary>
        VK_KANA = 0x15,

        /// <summary>
        /// IME Hanguel mode (maintained for compatibility; use <see cref="VK_HANGUL"/>)
        /// </summary>
        [Obsolete("Use VK_HANGUL instead")]
        VK_HANGEUL = 0x15,

        /// <summary>
        /// IME Hangul mode
        /// </summary>
        VK_HANGUL = 0x15,

        /* 0x16 : unassigned */

        /// <summary>
        /// IME Junja mode
        /// </summary>
        VK_JUNJA = 0x17,

        /// <summary>
        /// IME final mode
        /// </summary>
        VK_FINAL = 0x18,

        /// <summary>
        /// IME Hanja mode
        /// </summary>
        VK_HANJA = 0x19,

        /// <summary>
        /// IME Kanji mode
        /// </summary>
        VK_KANJI = 0x19,

        /* 0x1A : unassigned */

        /// <summary>
        /// ESC key
        /// </summary>
        VK_ESCAPE = 0x1B,

        /// <summary>
        /// IME convert
        /// </summary>
        VK_CONVERT = 0x1C,

        /// <summary>
        /// IME nonconvert
        /// </summary>
        VK_NONCONVERT = 0x1D,

        /// <summary>
        /// IME accept
        /// </summary>
        VK_ACCEPT = 0x1E,

        /// <summary>
        /// IME mode change request
        /// </summary>
        VK_MODECHANGE = 0x1F,

        /// <summary>
        /// SPACEBAR
        /// </summary>
        VK_SPACE = 0x20,

        /// <summary>
        /// PAGE UP key
        /// </summary>
        VK_PRIOR = 0x21,

        /// <summary>
        /// PAGE DOWN key
        /// </summary>
        VK_NEXT = 0x22,

        /// <summary>
        /// END key
        /// </summary>
        VK_END = 0x23,

        /// <summary>
        /// HOME key
        /// </summary>
        VK_HOME = 0x24,

        /// <summary>
        /// LEFT ARROW key
        /// </summary>
        VK_LEFT = 0x25,

        /// <summary>
        /// UP ARROW key
        /// </summary>
        VK_UP = 0x26,

        /// <summary>
        /// RIGHT ARROW key
        /// </summary>
        VK_RIGHT = 0x27,

        /// <summary>
        /// DOWN ARROW key
        /// </summary>
        VK_DOWN = 0x28,

        /// <summary>
        /// SELECT key
        /// </summary>
        VK_SELECT = 0x29,

        /// <summary>
        /// PRINT key
        /// </summary>
        VK_PRINT = 0x2A,

        /// <summary>
        /// EXECUTE key
        /// </summary>
        VK_EXECUTE = 0x2B,

        /// <summary>
        /// PRINT SCREEN key
        /// </summary>
        VK_SNAPSHOT = 0x2C,

        /// <summary>
        /// INS key
        /// </summary>
        VK_INSERT = 0x2D,

        /// <summary>
        /// DEL key
        /// </summary>
        VK_DELETE = 0x2E,

        /// <summary>
        /// HELP key
        /// </summary>
        VK_HELP = 0x2F,

        /// <summary>
        /// 0 key
        /// </summary>
        VK_KEY_0 = 0x30,

        /// <summary>
        /// 1 key
        /// </summary>
        VK_KEY_1 = 0x31,

        /// <summary>
        /// 2 key
        /// </summary>
        VK_KEY_2 = 0x32,

        /// <summary>
        /// 3 key
        /// </summary>
        VK_KEY_3 = 0x33,

        /// <summary>
        /// 4 key
        /// </summary>
        VK_KEY_4 = 0x34,

        /// <summary>
        /// 5 key
        /// </summary>
        VK_KEY_5 = 0x35,

        /// <summary>
        /// 6 key
        /// </summary>
        VK_KEY_6 = 0x36,

        /// <summary>
        /// 7 key
        /// </summary>
        VK_KEY_7 = 0x37,

        /// <summary>
        /// 8 key
        /// </summary>
        VK_KEY_8 = 0x38,

        /// <summary>
        /// 9 key
        /// </summary>
        VK_KEY_9 = 0x39,

        /* 0x3A - 0x40 : unassigned */

        /// <summary>
        /// A key
        /// </summary>
        VK_A = 0x41,

        /// <summary>
        /// B key
        /// </summary>
        VK_B = 0x42,

        /// <summary>
        /// C key
        /// </summary>
        VK_C = 0x43,

        /// <summary>
        /// D key
        /// </summary>
        VK_D = 0x44,

        /// <summary>
        /// E key
        /// </summary>
        VK_E = 0x45,

        /// <summary>
        /// F key
        /// </summary>
        VK_F = 0x46,

        /// <summary>
        /// G key
        /// </summary>
        VK_G = 0x47,

        /// <summary>
        /// H key
        /// </summary>
        VK_H = 0x48,

        /// <summary>
        /// I key
        /// </summary>
        VK_I = 0x49,

        /// <summary>
        /// J key
        /// </summary>
        VK_J = 0x4A,

        /// <summary>
        /// K key
        /// </summary>
        VK_K = 0x4B,

        /// <summary>
        /// L key
        /// </summary>
        VK_L = 0x4C,

        /// <summary>
        /// M key
        /// </summary>
        VK_M = 0x4D,

        /// <summary>
        /// N key
        /// </summary>
        VK_N = 0x4E,

        /// <summary>
        /// O key
        /// </summary>
        VK_O = 0x4F,

        /// <summary>
        /// P key
        /// </summary>
        VK_P = 0x50,

        /// <summary>
        /// Q key
        /// </summary>
        VK_Q = 0x51,

        /// <summary>
        /// R key
        /// </summary>
        VK_R = 0x52,

        /// <summary>
        /// S key
        /// </summary>
        VK_S = 0x53,

        /// <summary>
        /// T key
        /// </summary>
        VK_T = 0x54,

        /// <summary>
        /// U key
        /// </summary>
        VK_U = 0x55,

        /// <summary>
        /// V key
        /// </summary>
        VK_V = 0x56,

        /// <summary>
        /// W key
        /// </summary>
        VK_W = 0x57,

        /// <summary>
        /// X key
        /// </summary>
        VK_X = 0x58,

        /// <summary>
        /// Y key
        /// </summary>
        VK_Y = 0x59,

        /// <summary>
        /// Z key
        /// </summary>
        VK_Z = 0x5A,

        /// <summary>
        /// Left Windows key (Natural keyboard)
        /// </summary>
        VK_LWIN = 0x5B,

        /// <summary>
        /// Right Windows key (Natural keyboard)
        /// </summary>
        VK_RWIN = 0x5C,

        /// <summary>
        /// Applications key (Natural keyboard)
        /// </summary>
        VK_APPS = 0x5D,

        /* 0x5E : reserved */

        /// <summary>
        /// Computer Sleep key
        /// </summary>
        VK_SLEEP = 0x5F,

        /// <summary>
        /// Numeric keypad 0 key
        /// </summary>
        VK_NUMPAD0 = 0x60,

        /// <summary>
        /// Numeric keypad 1 key
        /// </summary>
        VK_NUMPAD1 = 0x61,

        /// <summary>
        /// Numeric keypad 2 key
        /// </summary>
        VK_NUMPAD2 = 0x62,

        /// <summary>
        /// Numeric keypad 3 key
        /// </summary>
        VK_NUMPAD3 = 0x63,

        /// <summary>
        /// Numeric keypad 4 key
        /// </summary>
        VK_NUMPAD4 = 0x64,

        /// <summary>
        /// Numeric keypad 5 key
        /// </summary>
        VK_NUMPAD5 = 0x65,

        /// <summary>
        /// Numeric keypad 6 key
        /// </summary>
        VK_NUMPAD6 = 0x66,

        /// <summary>
        /// Numeric keypad 7 key
        /// </summary>
        VK_NUMPAD7 = 0x67,

        /// <summary>
        /// Numeric keypad 8 key
        /// </summary>
        VK_NUMPAD8 = 0x68,

        /// <summary>
        /// Numeric keypad 9 key
        /// </summary>
        VK_NUMPAD9 = 0x69,

        /// <summary>
        /// Multiply key
        /// </summary>
        VK_MULTIPLY = 0x6A,

        /// <summary>
        /// Add key
        /// </summary>
        VK_ADD = 0x6B,

        /// <summary>
        /// Separator key
        /// </summary>
        VK_SEPARATOR = 0x6C,

        /// <summary>
        /// Subtract key
        /// </summary>
        VK_SUBTRACT = 0x6D,

        /// <summary>
        /// Decimal key
        /// </summary>
        VK_DECIMAL = 0x6E,

        /// <summary>
        /// Divide key
        /// </summary>
        VK_DIVIDE = 0x6F,

        /// <summary>
        /// F1 Key
        /// </summary>
        VK_F1 = 0x70,

        /// <summary>
        /// F2 Key
        /// </summary>
        VK_F2 = 0x71,

        /// <summary>
        /// F3 Key
        /// </summary>
        VK_F3 = 0x72,

        /// <summary>
        /// F4 Key
        /// </summary>
        VK_F4 = 0x73,

        /// <summary>
        /// F5 Key
        /// </summary>
        VK_F5 = 0x74,

        /// <summary>
        /// F6 Key
        /// </summary>
        VK_F6 = 0x75,

        /// <summary>
        /// F7 Key
        /// </summary>
        VK_F7 = 0x76,

        /// <summary>
        /// F8 Key
        /// </summary>
        VK_F8 = 0x77,

        /// <summary>
        /// F9 Key
        /// </summary>
        VK_F9 = 0x78,

        /// <summary>
        /// F10 Key
        /// </summary>
        VK_F10 = 0x79,

        /// <summary>
        /// F11 Key
        /// </summary>
        VK_F11 = 0x7A,

        /// <summary>
        /// F12 Key
        /// </summary>
        VK_F12 = 0x7B,

        /// <summary>
        /// F13 Key
        /// </summary>
        VK_F13 = 0x7C,

        /// <summary>
        /// F14 Key
        /// </summary>
        VK_F14 = 0x7D,

        /// <summary>
        /// F15 Key
        /// </summary>
        VK_F15 = 0x7E,

        /// <summary>
        /// F16 Key
        /// </summary>
        VK_F16 = 0x7F,

        /// <summary>
        /// F17 Key
        /// </summary>
        VK_F17 = 0x80,

        /// <summary>
        /// F18 Key
        /// </summary>
        VK_F18 = 0x81,

        /// <summary>
        /// F19 Key
        /// </summary>
        VK_F19 = 0x82,

        /// <summary>
        /// F20 Key
        /// </summary>
        VK_F20 = 0x83,

        /// <summary>
        /// F21 Key
        /// </summary>
        VK_F21 = 0x84,

        /// <summary>
        /// F22 Key
        /// </summary>
        VK_F22 = 0x85,

        /// <summary>
        /// F23 Key
        /// </summary>
        VK_F23 = 0x86,

        /// <summary>
        /// F24 Key
        /// </summary>
        VK_F24 = 0x87,

        /* 0x88 - 0x8F : unassigned */

        /// <summary>
        /// NUM LOCK key
        /// </summary>
        VK_NUMLOCK = 0x90,

        /// <summary>
        /// SCROLL LOCK key
        /// </summary>
        VK_SCROLL = 0x91,

        /* 0x92 - 0x96 : OEM specific */

        /// <summary>
        /// '=' key on numpad (NEC PC-9800 kbd definitions)
        /// </summary>
        VK_OEM_NEC_EQUAL = 0x92,

        /// <summary>
        /// 'Dictionary' key (Fujitsu/OASYS kbd definitions)
        /// </summary>
        VK_OEM_FJ_JISHO = 0x92,

        /// <summary>
        /// 'Unregister word' key (Fujitsu/OASYS kbd definitions)
        /// </summary>
        VK_OEM_FJ_MASSHOU = 0x93,

        /// <summary>
        /// 'Register word' key (Fujitsu/OASYS kbd definitions)
        /// </summary>
        VK_OEM_FJ_TOUROKU = 0x94,

        /// <summary>
        /// 'Left OYAYUBI' key (Fujitsu/OASYS kbd definitions)
        /// </summary>
        VK_OEM_FJ_LOYA = 0x95,

        /// <summary>
        /// 'Right OYAYUBI' key (Fujitsu/OASYS kbd definitions)
        /// </summary>
        VK_OEM_FJ_ROYA = 0x96,

        /* 0x97 - 0x9F : unassigned */

        /// <summary>
        /// Left SHIFT key
        /// </summary>
        /// <remarks>Used only as parameters to <see cref="GetAsyncKeyState" /> and  <see cref="GetKeyState" />. * No other API or message will distinguish left and right keys in this way.</remarks>
        VK_LSHIFT = 0xA0,

        /// <summary>
        /// Right SHIFT key
        /// </summary>
        VK_RSHIFT = 0xA1,

        /// <summary>
        /// Left CONTROL key
        /// </summary>
        VK_LCONTROL = 0xA2,

        /// <summary>
        /// Right CONTROL key
        /// </summary>
        VK_RCONTROL = 0xA3,

        /// <summary>
        /// Left MENU key
        /// </summary>
        VK_LMENU = 0xA4,

        /// <summary>
        /// Right MENU key
        /// </summary>
        VK_RMENU = 0xA5,

        /// <summary>
        /// Browser Back key
        /// </summary>
        VK_BROWSER_BACK = 0xA6,

        /// <summary>
        /// Browser Forward  key
        /// </summary>
        VK_BROWSER_FORWARD = 0xA7,

        /// <summary>
        /// Browser Refresh  key
        /// </summary>
        VK_BROWSER_REFRESH = 0xA8,

        /// <summary>
        /// Browser Stop  key
        /// </summary>
        VK_BROWSER_STOP = 0xA9,

        /// <summary>
        /// Browser Search  key
        /// </summary>
        VK_BROWSER_SEARCH = 0xAA,

        /// <summary>
        /// Browser Favorites  key
        /// </summary>
        VK_BROWSER_FAVORITES = 0xAB,

        /// <summary>
        /// Browser Start and Home key
        /// </summary>
        VK_BROWSER_HOME = 0xAC,

        /// <summary>
        /// Volume Mute key
        /// </summary>
        VK_VOLUME_MUTE = 0xAD,

        /// <summary>
        /// Volume Down key
        /// </summary>
        VK_VOLUME_DOWN = 0xAE,

        /// <summary>
        /// Volume Up key
        /// </summary>
        VK_VOLUME_UP = 0xAF,

        /// <summary>
        /// Next Track key
        /// </summary>
        VK_MEDIA_NEXT_TRACK = 0xB0,

        /// <summary>
        /// Previous Track key
        /// </summary>
        VK_MEDIA_PREV_TRACK = 0xB1,

        /// <summary>
        /// Stop Media key
        /// </summary>
        VK_MEDIA_STOP = 0xB2,

        /// <summary>
        /// Play/Pause Media key
        /// </summary>
        VK_MEDIA_PLAY_PAUSE = 0xB3,

        /// <summary>
        /// Start Mail key
        /// </summary>
        VK_LAUNCH_MAIL = 0xB4,

        /// <summary>
        /// Select Media key
        /// </summary>
        VK_LAUNCH_MEDIA_SELECT = 0xB5,

        /// <summary>
        /// Start Application 1 key
        /// </summary>
        VK_LAUNCH_APP1 = 0xB6,

        /// <summary>
        /// Start Application 2 key
        /// </summary>
        VK_LAUNCH_APP2 = 0xB7,

        /* 0xB8 - 0xB9 : reserved */

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
        /// <remarks>For the US standard keyboard, the ';:' key</remarks>
        VK_OEM_1 = 0xBA,

        /// <summary>
        /// For any country/region, the '+' key.
        /// </summary>
        VK_OEM_PLUS = 0xBB,

        /// <summary>
        /// For any country/region, the ',' key.
        /// </summary>
        VK_OEM_COMMA = 0xBC,

        /// <summary>
        /// For any country/region, the '-' key.
        /// </summary>
        VK_OEM_MINUS = 0xBD,

        /// <summary>
        /// For any country/region, the '.' key.
        /// </summary>
        VK_OEM_PERIOD = 0xBE,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
        /// <remarks>For the US standard keyboard, the '/?' key</remarks>
        VK_OEM_2 = 0xBF,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
        /// <remarks>For the US standard keyboard, the '`~' key</remarks>
        VK_OEM_3 = 0xC0,

        /* 0xC1 - 0xD7 : reserved */
        /* 0xD8 - 0xDA : unassigned */

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
        /// <remarks>For the US standard keyboard, the '[{' key</remarks>
        VK_OEM_4 = 0xDB,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
        /// <remarks>For the US standard keyboard, the '\|' key</remarks>
        VK_OEM_5 = 0xDC,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
        /// <remarks>For the US standard keyboard, the ']}' key</remarks>
        VK_OEM_6 = 0xDD,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
        /// <remarks>For the US standard keyboard, the 'single-quote/double-quote' (''"') key</remarks>
        VK_OEM_7 = 0xDE,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
        VK_OEM_8 = 0xDF,

        /* 0xE0 : reserved */

        /* 0xE1 : OEM specific */

        /// <summary>
        /// OEM specific
        /// </summary>
        /// <remarks>'AX' key on Japanese AX kbd</remarks>
        VK_OEM_AX = 0xE1,

        /// <summary>
        /// Either the angle bracket ("<![CDATA[<>]]>") key or the backslash ("\|") key on the RT 102-key keyboard
        /// </summary>
        VK_OEM_102 = 0xE2,

        /* 0xE3 - 0xE4 : OEM specific */

        /// <summary>
        /// OEM specific
        /// </summary>
        /// <remarks>Help key on ICO</remarks>
        VK_ICO_HELP = 0xE3,

        /// <summary>
        /// OEM specific
        /// </summary>
        /// <remarks>00 key on ICO</remarks>
        VK_ICO_00 = 0xE4,

        /// <summary>
        /// IME PROCESS key
        /// </summary>
        VK_PROCESSKEY = 0xE5,

        /* 0xE6 : OEM specific */

        /// <summary>
        /// OEM specific
        /// </summary>
        /// <remarks>Clear key on ICO</remarks>
        VK_ICO_CLEAR = 0xE6,

        /// <summary>
        /// Used to pass Unicode characters as if they were keystrokes. The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods.
        /// </summary>
        /// <remarks>For more information, see Remark in <see cref="KEYBDINPUT"/>, <see cref="SendInput(int, INPUT*, int)"/>, <see cref="WindowMessage.WM_KEYDOWN"/>, and <see cref="WindowMessage.WM_KEYUP"/></remarks>
        VK_PACKET = 0xE7,

        /* 0xE8 : unassigned */

        /* 0xE9 - 0xF5 : OEM specific */

        /// <summary>
        /// Nokia/Ericsson definition
        /// </summary>
        VK_OEM_RESET = 0xE9,

        /// <summary>
        /// Nokia/Ericsson definition
        /// </summary>
        VK_OEM_JUMP = 0xEA,

        /// <summary>
        /// Nokia/Ericsson definition
        /// </summary>
        VK_OEM_PA1 = 0xEB,

        /// <summary>
        /// Nokia/Ericsson definition
        /// </summary>
        VK_OEM_PA2 = 0xEC,

        /// <summary>
        /// Nokia/Ericsson definition
        /// </summary>
        VK_OEM_PA3 = 0xED,

        /// <summary>
        /// Nokia/Ericsson definition
        /// </summary>
        VK_OEM_WSCTRL = 0xEE,

        /// <summary>
        /// Nokia/Ericsson definition
        /// </summary>
        VK_OEM_CUSEL = 0xEF,

        /// <summary>
        /// Nokia/Ericsson definition
        /// </summary>
        VK_OEM_ATTN = 0xF0,

        /// <summary>
        /// Nokia/Ericsson definition
        /// </summary>
        VK_OEM_FINISH = 0xF1,

        /// <summary>
        /// Nokia/Ericsson definition
        /// </summary>
        VK_OEM_COPY = 0xF2,

        /// <summary>
        /// Nokia/Ericsson definition
        /// </summary>
        VK_OEM_AUTO = 0xF3,

        /// <summary>
        /// Nokia/Ericsson definition
        /// </summary>
        VK_OEM_ENLW = 0xF4,

        /// <summary>
        /// Nokia/Ericsson definition
        /// </summary>
        VK_OEM_BACKTAB = 0xF5,

        /// <summary>
        /// Attn key
        /// </summary>
        VK_ATTN = 0xF6,

        /// <summary>
        /// CrSel key
        /// </summary>
        VK_CRSEL = 0xF7,

        /// <summary>
        /// ExSel key
        /// </summary>
        VK_EXSEL = 0xF8,

        /// <summary>
        /// Erase EOF key
        /// </summary>
        VK_EREOF = 0xF9,

        /// <summary>
        /// Play key
        /// </summary>
        VK_PLAY = 0xFA,

        /// <summary>
        /// Zoom key
        /// </summary>
        VK_ZOOM = 0xFB,

        /// <summary>
        /// Reserved constant by Windows headers definition
        /// </summary>
        VK_NONAME = 0xFC,

        /// <summary>
        /// PA1 key
        /// </summary>
        VK_PA1 = 0xFD,

        /// <summary>
        /// Clear key
        /// </summary>
        VK_OEM_CLEAR = 0xFE,

        /* 0xFF : reserved */
    }
}
