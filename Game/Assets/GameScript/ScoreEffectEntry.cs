using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreEffectEntry : MonoBehaviour
{
    [SerializeField]
    ParticleSystem particle;
    [SerializeField]
    Animator animator;
    public void Make()
    {
        gameObject.SetActive(true);
        particle.Play();
        animator.Play("", -1, 0);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
