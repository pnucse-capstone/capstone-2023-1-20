using UnityEngine;
using System;
using System.IO;
using System.Linq;
using UnityEngine.Profiling;
using FMOD;
using System.Runtime.InteropServices;
using FMOD.Studio;
using static UnityEngine.Rendering.PostProcessing.HistogramMonitor;
using System.Threading;
using NAudio.Wave;
using System.Diagnostics;
using System.Runtime.Remoting.Channels;

public class AudioAdapter // 파일 바이너리 <-> 음악데이터
{
    const byte EncryptMask = 0xC8;
    byte[] encrypted_bytes;
    AudioData audio;
    public AudioData LoadAudio(byte[] audio_bytes)
    {
        byte[] bytes = (byte[])audio_bytes.Clone();
        Encrypt(ref bytes);
        encrypted_bytes = bytes;
        audio = LoadEncryptedAudio(encrypted_bytes);
        return audio;
    }
    public AudioData LoadEncryptedAudio(byte[] encrypted_bytes)
    {
        //무식한 양의 클론이...
        byte[] bytes = (byte[])encrypted_bytes.Clone();
        this.encrypted_bytes = (byte[])bytes.Clone();
        Decrypt(ref bytes);
        using (var ms = new MemoryStream(bytes))
        {
            SoundWrapper sw = AudioFileLoader.Convert(ms);
            audio = new AudioData(sw);
        }
        return audio;
    }

    void Encrypt(ref byte[] bytes)
    {
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] ^= EncryptMask;
        }
    }
    void Decrypt(ref byte[] bytes)
    {
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] ^= EncryptMask;
        }
    }
    public AudioData GetAudio()
    {
        return audio;
    }
    public byte[] GetEncryptedBytes()
    {
        return encrypted_bytes;
    }

}

static class AudioFileLoader // 메모리 - > sound
{
    public static SoundWrapper Convert(Stream ms)
    {
        Sound sound1, sound2;
        byte[] bytes = new byte[ms.Length];
        ms.Read(bytes, 0, (int)ms.Length);
        
        FMOD.System sys = FMODWrapper.GetSystem();

        CREATESOUNDEXINFO info = new CREATESOUNDEXINFO();
        info.cbsize = Marshal.SizeOf(info);
        info.length = (uint)bytes.Length;
        UnityEngine.Debug.Log("CREATE SOUND");
        sys.createSound(bytes, MODE.OPENMEMORY | MODE.CREATESAMPLE, ref info, out sound1);
        sys.createSound(bytes, MODE.OPENONLY | MODE.OPENMEMORY | MODE.CREATESAMPLE, ref info, out sound2);
        return new SoundWrapper(sound2,sound1);
    }
}
public class SoundWrapper
{
    uint length;
    SOUND_TYPE type;
    SOUND_FORMAT format;
    int channel, bits;
    float samplerate;
    uint sampleCount;
    Sound openonly,play;
    uint time;
    bool hasPCM = false;
    public SoundWrapper(Sound play)
    {
        hasPCM = false;
        play.getDefaults(out samplerate, out _);
        play.getLength(out length, TIMEUNIT.PCMBYTES);
        play.getLength(out sampleCount, TIMEUNIT.PCM);
        play.getLength(out time, TIMEUNIT.MS);
        play.getFormat(out type, out format, out channel, out bits);

        UnityEngine.Debug.Log("SoundWrapper:" + (samplerate, length, format, sampleCount, sampleCount, time, channel));
        this.play = play;
    }
    public SoundWrapper(Sound openonly , Sound play)
    {
        hasPCM = true;
        openonly.getDefaults(out samplerate, out _);
        openonly.getLength(out length, TIMEUNIT.PCMBYTES);
        openonly.getLength(out sampleCount, TIMEUNIT.PCM);
        openonly.getLength(out time, TIMEUNIT.MS);
        openonly.getFormat(out type, out format, out channel, out bits);

        UnityEngine.Debug.Log("SoundWrapper:"+(samplerate,length,format,sampleCount,sampleCount,time,channel));
        this.openonly= openonly;
        this.play = play;
    }
    public PCMSamples GetFullPCM()
    {
        PCMSamples pcm = new PCMSamples(GetFullSamples(), SampleCount, Channels, SampleRate);
        return pcm;
    }

    public TimeSpan TotalTime
    {
        get
        {

            return TimeSpan.FromMilliseconds(time);
        }
    }
    public int Channels => channel;
    public int SampleRate => (int)samplerate;
    public int SampleCount => (int)sampleCount;
    public float[] GetFullSamples()
    {
        if (hasPCM)
        {
            float[] samples = new float[SampleCount * Channels];
            byte[] bytes = new byte[length];
            ulong read = 0;

            read = ReadData(bytes, length, openonly);

            UnityEngine.Debug.Assert(read != 0);
            BytesToSamples(samples, bytes);
            return samples;
        }
        else
        {
            return null;
        }
    }

    private void BytesToSamples(float[] samples, byte[] bytes)
    {
        if (format == SOUND_FORMAT.PCM16)
        {
            for (int i = 0; i < SampleCount * Channels; i++)
            {
                short sampleValue = (short)(bytes[i * 2] | (bytes[i * 2 + 1] << 8)); // 두 바이트로부터 16비트 샘플 값을 읽음
                float sampleFloatValue = sampleValue / 32768f; // 부호 있는 16비트 값을 -1.0에서 1.0 사이의 부동 소수점 값으로 정규화
                samples[i] = sampleFloatValue;
            }

        }
        else if (format == SOUND_FORMAT.PCMFLOAT)
        {
            Buffer.BlockCopy(bytes, 0, samples, 0, bytes.Length); // byte 배열을 float 배열로 복사
        }
        else if (format == SOUND_FORMAT.PCM8)
        {
            for (int i = 0; i < SampleCount * Channels; i++)
            {
                sbyte sampleValue = (sbyte)bytes[i]; // 부호 있는 8비트 샘플 값을 읽음
                float sampleFloatValue = sampleValue / 128f; // 부호 있는 8비트 값을 -1.0에서 1.0 사이의 부동 소수점 값으로 정규화
                samples[i] = sampleFloatValue;
            }
        }
        else if (format == SOUND_FORMAT.PCM24)
        {

            for (int i = 0; i < sampleCount * Channels; i++)
            {
                int sampleValue = bytes[i * 3] | (bytes[i * 3 + 1] << 8) | (bytes[i * 3 + 2] << 16); // 세 바이트로부터 24비트 샘플 값을 읽음
                if (sampleValue >= 8388608) // 음수 처리
                    sampleValue -= 16777216;
                float sampleFloatValue = sampleValue / 8388608f; // 부호 있는 24비트 값을 -1.0에서 1.0 사이의 부동 소수점 값으로 정규화
                samples[i] = sampleFloatValue;
            }
        }
        else if (format == SOUND_FORMAT.PCM32)
        {

            for (int i = 0; i < SampleCount * Channels; i++)
            {
                int sampleValue = BitConverter.ToInt32(bytes, i * 4); // 4바이트로부터 32비트 샘플 값을 읽음
                float sampleFloatValue = sampleValue / 2147483648f; // 부호 있는 32비트 값을 -1.0에서 1.0 사이의 부동 소수점 값으로 정규화
                samples[i] = sampleFloatValue;
            }
        }
        else
        {
            UnityEngine.Debug.LogError("PCM CONVERSION ERROR:" + format);
        }
    }

    public float[] GetRangedSamples(double left, double right)
    {
        float[] fullsamples = GetFullSamples();
        int sliceStart = SampleCount * Channels / 2;
        int sliceLength = 10 * SampleRate * Channels;
        float[] samples = new float[sliceLength];
        Array.Copy(fullsamples, sliceStart, samples, 0, sliceLength);
        return samples;
    }

    public uint ReadData(byte[] buffer, uint length, Sound _fmodSound)
    {
        if (length > buffer.Length)
        {
            throw new ArgumentOutOfRangeException("length", string.Format("Cannot read {0} bytes into buffer with length {1}", length, buffer.Length));
        }

        uint read;

        var result = _fmodSound.readData(buffer, out read);
        UnityEngine.Debug.Log("TEST" + result);
        UnityEngine.Debug.Log("TESTRead" + (read, length));
        return read;
    }

    public Sound GetSound()
    {
        return play;
    }

    public void Release()
    {
        play.release();
        if (hasPCM)
        {
            openonly.release();
        }
    }
}
public class AudioData
{
    PCMSamples pcm;
    SoundWrapper sound;
    public float length { get => pcm.length; }

    public AudioData(PCMSamples pcm)
    {
        this.pcm = pcm;
        sound = new SoundWrapper(pcm.CreateFMODSound());
    }
    public AudioData(SoundWrapper sound)
    {
        this.sound = sound;
        pcm = this.sound.GetFullPCM();
        UnityEngine.Debug.Log("PCM:"+pcm);
    }
    public Sound GetSound()
    {
        return sound.GetSound();
    }
    public PCMSamples GetPCM()
    {
        return pcm;
    }

    internal void Release()
    {
        sound.Release();
    }
}
public class PCMSamples
{
    public float[] Samples;
    public int SampleCount;
    public int Channels;
    public int SampleRate;

    public float length { get => (float)SampleCount/SampleRate; }
    /*
    public void Reverse()
    {
        for(int i=0;i<Samples.Length/2; i++)
        {
            var temp = Samples[i];
            Samples[i]=Samples[Samples.Length - 1 - i];
            Samples[Samples.Length - 1 - i] = temp;
        }
    }
    public void SetClip(AudioClip clip)
    {
        clip.SetData(Samples, 0);
    }
    */
    public void SaveWave(string path) // 해당 PCM을 해당 경로에 wav로 저장
    {

        var wavf = new WaveFormat(SampleRate,Channels);
        using (var writer = new WaveFileWriter(path, wavf))
        {
            writer.WriteSamples(Samples, 0, Samples.Length);
        }
    }
    public PCMSamples(float[] samples, int sampleCount, int channelCount, int sampleRate)
    {
        Samples = samples;
        SampleCount = sampleCount;
        Channels = channelCount;
        SampleRate = sampleRate;
    }
    public Sound CreateFMODSound()
    {
        // create FMOD system and initialize it
        byte[] byteData = new byte[Samples.Length * 2]; // 변환된 바이트 배열
        for (int i = 0; i < Samples.Length; i++)
        {
            short sampleValue = (short)(Samples[i] * 32767f);
                byteData[i * 2] = (byte)(sampleValue & 0xff);
                byteData[i * 2 + 1] = (byte)(sampleValue >> 8);
        }


        CREATESOUNDEXINFO exInfo = new CREATESOUNDEXINFO();
        exInfo.cbsize = Marshal.SizeOf(exInfo);
        exInfo.length = (uint)byteData.Length;
        exInfo.format = SOUND_FORMAT.PCM16;
        exInfo.numchannels = Channels; // number of channels
        exInfo.defaultfrequency = SampleRate; // sample rate

        Sound sound;
        var result = FMODWrapper.GetSystem().createSound(byteData, MODE.OPENMEMORY | MODE.OPENRAW | MODE.CREATESAMPLE, ref exInfo, out sound);
        UnityEngine.Debug.Log(result);
        return sound;
    }
    public PCMSamples(PCMSamples pcm)
    {
        Samples = (float[])pcm.Samples.Clone();
        SampleCount = pcm.SampleCount;
        Channels = pcm.Channels;
        SampleRate = pcm.SampleRate;
    }
    public PCMSamples DefaultSlice()
    {
        var begin = Samples.Length/Channels/ 2 - SampleRate * 5*Channels;
        float[] samples  = Samples.Skip(begin).Take(SampleRate * 10*Channels).ToArray();
        var pcm = new PCMSamples(samples,SampleCount,Channels,SampleRate);
        return pcm;
    }
}

