using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using System.IO;
using System;
using System.Runtime.InteropServices;
using System.Linq;

public class TestScript : MonoBehaviour
{
    private void Start()
    {
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            var sys = FMODWrapper.GetSystem();
            Sound sound;
            var result = sys.createSound(Path.Combine(Application.dataPath, "LovelyGlassshort.wav"), MODE.OPENONLY, out sound);
            UnityEngine.Debug.Log(result);
            uint length;
            SOUND_TYPE type;
            SOUND_FORMAT format;
            int channel;
            int bits;
            float samplerate;
            sound.getDefaults(out samplerate, out _);
            sound.getLength(out length, TIMEUNIT.PCMBYTES);
            sound.getFormat(out type,out format,out channel,out bits);
            UnityEngine.Debug.Log((samplerate,type,format,channel,bits));
            UnityEngine.Debug.Log(length);
            byte[] bytes= new byte[length];

            bytes = bytes.Skip((int)(length / 2)).ToArray();

            CREATESOUNDEXINFO info = new CREATESOUNDEXINFO();
            info.cbsize = Marshal.SizeOf(info);
            info.length = (uint)bytes.Length;
            info.format = format;
            info.numchannels = channel; // number of channels
            info.defaultfrequency = (int)samplerate; // sample rate

            Sound sound2;
            result = sys.createSound(bytes, MODE.OPENMEMORY | MODE.OPENRAW, ref info, out sound2);
            ChannelGroup cg;
            sys.getMasterChannelGroup(out cg);
            sys.playSound(sound2,cg,false,out _);
        }

    }
}