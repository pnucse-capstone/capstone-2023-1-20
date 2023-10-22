using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class ParticleNoteEffect : MonoBehaviour, NoteEffect
{
    [SerializeField]
    ParticleSystem begin_particle;
    [SerializeField]
    OnEffect on_particle;
    [SerializeField]
    ParticleSystem end_particle;
    public void Begin(RenderInfo info)
    {
        if(Game.thickmode)info.size.y *= 2;
        transform.localScale = info.size;
        transform.position = info.position;
        transform.rotation = info.rotation;
        begin_particle.Play();
        on_particle.Set(info.size.x);
        on_particle.Play();
    }
    public void On(RenderInfo info)
    {
        if (Game.thickmode) info.size.y *= 2;
        transform.localScale = info.size;
        transform.position = info.position;
        transform.rotation = info.rotation;
    }
    public void End(RenderInfo info)
    {
        if (Game.thickmode) info.size.y *= 2;
        transform.localScale = info.size;
        transform.position = info.position;
        transform.rotation = info.rotation;

        on_particle.Stop();
        end_particle.Play();
    }
    public void Stop()
    {
        on_particle.Stop();
    }
}
