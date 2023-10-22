using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMeter : MonoBehaviour
{
    static ParticleSystem particle;
    static float range = 0.1F;
    private void Start()
    {
        particle = GetComponentInChildren<ParticleSystem>();
    }
    static float Calc()
    {
        float pivot = NoteEditor.snaps.SnapRound(Game.time);

        Debug.Log((pivot, Game.time));
        return Game.time- pivot;
    }
    public static void Show(float time)
    {
        var dt = Calc();
        if (Mathf.Abs(dt) < range)
        {
            Debug.Log((dt, range));
            particle.gameObject.transform.localPosition = Vector3.right * dt / range;
            particle.Emit(1);

        }
    }
}
