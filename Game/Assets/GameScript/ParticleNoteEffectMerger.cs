using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleNoteEffectMerger : MonoBehaviour
{
    [SerializeField]ParticleNoteEffect perfect;
    [SerializeField] ParticleNoteEffect good;
    [SerializeField] ParticleNoteEffect ok;
    [SerializeField] ParticleNoteEffect miss;
    public void SetEffect(JudgeType effect, NoteStep step,RenderInfo info)
    {

        ParticleNoteEffect select = null;
        switch (effect)
        {
            case JudgeType.perfect:
                select = perfect;
                break;
            case JudgeType.good:
                select = good;
                break;
            case JudgeType.ok:
                select = ok;
                break;
            case JudgeType.miss:
                select = miss;
                break;
            default:
                select = miss;
                break;
        }

        switch (step)
        {
            case NoteStep.BEGIN:
                select.Begin(info);
                break;
            case NoteStep.ON:
                select.On(info);
                break;
            case NoteStep.END:
                StopAll();
                select.End(info);
                break;
        }

    }
    public void StopAll()
    {
        perfect.Stop();
        good.Stop();
        ok.Stop();
        miss.Stop();
    }
}
