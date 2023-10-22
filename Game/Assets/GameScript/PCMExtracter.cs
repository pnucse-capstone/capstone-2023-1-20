using FMOD;
using NAudio.Wave;
using NVorbis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

public class PCMExtracter
{
    public static PCMSamples GetPCM(string path)
    {
        var ext = Path.GetExtension(path);
        switch (ext)
        {
            case "ogg": return new OggToPCM().GetPCM(path); 
            case "mp3": return new Mp3ToPCM().GetPCM(path); 
            case "wav": return new WavToPCM().GetPCM(path);
            default: return null;
        }
    }
}

interface PCMStrategy
{
    PCMSamples GetPCM(string path);
}
class WavToPCM : PCMStrategy
{
    public PCMSamples GetPCM(string path)
    {

        throw new NotImplementedException();
    }
}

class OggToPCM : PCMStrategy
{
    public PCMSamples GetPCM(string path)
    {
        throw new NotImplementedException();
    }
}
class Mp3ToPCM : PCMStrategy
{
    public PCMSamples GetPCM(string path)
    {
            throw new NotImplementedException();
    }
}

abstract class PCMConverter : IDisposable
{
    public static PCMConverter Create(int format_code, Stream ms)
    {
        switch (format_code)
        {
            case 0: return new WaveConverter(ms);
//            case 1: return new MP3Converter(ms);
            case 2: return new VorbisConverter(ms);
        }
        return null;
    }
    public PCMSamples GetFullPCM()
    {
        PCMSamples pcm = new PCMSamples(GetFullSamples(), SampleCount, Channels, SampleRate);
        return pcm;
    }
    public PCMSamples GetPreviewPCM()
    {
        PCMSamples pcm = new PCMSamples(GetPreviewSamples(), SampleCount, Channels, SampleRate);
        return pcm;
    }
    public PCMSamples GetRangedPCM(double left, double right)
    {
        float[] samples = GetRangedSamples(left, right);
        int SampleCount = (int)((right - left) * SampleRate);
        PCMSamples pcm = new PCMSamples(samples, SampleCount, Channels, SampleRate);
        return pcm;
    }
    public abstract TimeSpan TotalTime { get; }
    public abstract int SampleCount { get; }
    public abstract int Channels { get; }
    public abstract int SampleRate { get; }
    public abstract float[] GetFullSamples();
    public float[] GetPreviewSamples()
    {
        return GetRangedSamples(TotalTime.TotalSeconds / 2 - 5, TotalTime.TotalSeconds / 2 + 5);
    }
    public abstract float[] GetRangedSamples(double left, double right);
    public abstract int FormatCode { get; }
    public abstract void Dispose();
}
/*
class MP3Converter : PCMConverter
{
    Mp3FileReader reader;
    public MP3Converter(Stream ms)
    {
        reader = new Mp3FileReader(ms);
    }
    public override TimeSpan TotalTime => reader.TotalTime;
    public override int Channels => reader.WaveFormat.Channels;
    public override int SampleRate => reader.WaveFormat.SampleRate;
    public override int SampleCount => (int)(reader.Length / (reader.WaveFormat.BitsPerSample / 8) / reader.WaveFormat.Channels);
    public override float[] GetFullSamples()
    {
        float[] samples = new float[SampleCount * Channels];
        reader.CurrentTime = new TimeSpan(0, 0, 0);
        reader.ToSampleProvider().Read(samples, 0, samples.Length);
        return samples;
    }
    public override float[] GetRangedSamples(double left, double right)
    {
        UnityEngine.Debug.Assert(left <= right);
        float[] samples = new float[(int)((right - left) * SampleRate * Channels)];
        reader.CurrentTime = new TimeSpan(0, 0, (int)left);
        reader.ToSampleProvider().Read(samples, 0, samples.Length);
        return samples;
    }
    public override void Dispose()
    {
        reader.Dispose();
    }
    public override int FormatCode => 1;
}
*/
class VorbisConverter : PCMConverter
{
    VorbisReader reader;
    public VorbisConverter(Stream ms)
    {
        reader = new VorbisReader(ms);
    }
    public override TimeSpan TotalTime => reader.TotalTime;
    public override int Channels => reader.Channels;
    public override int SampleRate => reader.SampleRate;
    public override int SampleCount => (int)reader.TotalSamples;
    public override float[] GetFullSamples()
    {
        float[] samples = new float[reader.TotalSamples * reader.Channels];
        reader.ReadSamples(samples, 0, samples.Length);
        return samples;
    }
    public override float[] GetRangedSamples(double left, double right)
    {
        UnityEngine.Debug.Assert(left <= right);
        float[] samples = new float[(int)((right - left) * SampleRate * Channels)];
        reader.SeekTo(new TimeSpan(0, 0, (int)left));
        reader.ReadSamples(samples, 0, samples.Length);
        return samples;
    }
    public override void Dispose()
    {
        reader.Dispose();
    }
    public override int FormatCode => 2;
}

class WaveConverter : PCMConverter
{
    WaveFileReader reader;
    public WaveConverter(Stream ms)
    {
        reader = new WaveFileReader(ms);
    }
    public override TimeSpan TotalTime => reader.TotalTime;
    public override int Channels => reader.WaveFormat.Channels;
    public override int SampleRate => reader.WaveFormat.SampleRate;
    public override int SampleCount => (int)reader.SampleCount;
    public override float[] GetFullSamples()
    {
        float[] samples = new float[SampleCount * Channels];
        reader.CurrentTime = new TimeSpan(0, 0, 0);
        reader.ToSampleProvider().Read(samples, 0, samples.Length);
        return samples;
    }
    public override float[] GetRangedSamples(double left, double right)
    {
        UnityEngine.Debug.Assert(left <= right);
        float[] samples = new float[(int)((right - left) * SampleRate * Channels)];
        reader.CurrentTime = new TimeSpan(0, 0, (int)left);
        reader.ToSampleProvider().Read(samples, 0, samples.Length);
        return samples;
    }
    public override void Dispose()
    {
        reader.Dispose();
    }
    public override int FormatCode => 0;
}