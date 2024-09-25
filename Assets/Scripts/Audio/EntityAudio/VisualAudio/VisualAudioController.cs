using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualAudioController : MonoBehaviour
{
    [SerializeField] EntityAudioClipsSO entityClipsSO;
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
}
