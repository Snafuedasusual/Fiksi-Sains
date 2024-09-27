using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualAudioController : MonoBehaviour
{
    [SerializeField] EntityAudioClipsSO entityClipsSO;
    [SerializeField] AudioSource r_Foot;
    [SerializeField] AudioSource l_Foot;
    [SerializeField] AudioSource audSrc;

    private void PlayFootstepsAudio()
    {
        if(entityClipsSO == null) return;
        if (entityClipsSO.footStepsClips.Length <= 0) return;
        if (entityClipsSO.footStepsClips.Length == 1)
        {
            SFXManager.instance.PlayAudio(audSrc, (entityClipsSO.footStepsClips[0]));
        }
        else
        {
            var selectedAudio = entityClipsSO.footStepsClips[Random.Range(0, entityClipsSO.footStepsClips.Length)];
            SFXManager.instance.PlayAudio(audSrc, selectedAudio);
        }
    }

    private void PlayLFootstepAudio()
    {
        if (entityClipsSO == null) return;
        if (entityClipsSO.footStepsClips.Length <= 0) return;
        if (entityClipsSO.footStepsClips.Length == 1)
        {
            SFXManager.instance.PlayAudio(l_Foot, (entityClipsSO.footStepsClips[0]));
        }
        else
        {
            var selectedAudio = entityClipsSO.footStepsClips[Random.Range(0, entityClipsSO.footStepsClips.Length)];
            SFXManager.instance.PlayAudio(l_Foot, selectedAudio);
        }
    }

    private void PlayRFootstepAudio()
    {
        if (entityClipsSO == null) return;
        if (entityClipsSO.footStepsClips.Length <= 0) return;
        if (entityClipsSO.footStepsClips.Length == 1)
        {
            SFXManager.instance.PlayAudio(r_Foot, (entityClipsSO.footStepsClips[0]));
        }
        else
        {
            var selectedAudio = entityClipsSO.footStepsClips[Random.Range(0, entityClipsSO.footStepsClips.Length)];
            SFXManager.instance.PlayAudio(r_Foot, selectedAudio);
        }
    }
}
