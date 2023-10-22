using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEffect : MonoBehaviour
{
    ParticleSystem now;
    [SerializeField] ParticleSystem[] system;
    float[] basis;
    void Start()
    {
        now = GetComponent<ParticleSystem>();
        basis = new float[system.Length];
        for(int i=0;i<system.Length; i++)
        {
            basis[i] = system[i].emission.rateOverTimeMultiplier;
        }
    }
    public void Set(float multiplier)
    {
        for(int i=0; i < system.Length; i++)
        {
            system[i].Stop();
            var em = system[i].emission;
            em.rateOverTimeMultiplier = basis[i] * multiplier;
            system[i].Play();
        }
    }
    public void Play()
    {
        now.Play();
    }
    public void Stop()
    {
        now.Stop();
    }
}
