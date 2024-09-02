using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TerrorBirdHashSO", menuName = "AnimationHashes/TerrorBird")]
public class TerrorBirdHashSO : AnimationHashSO
{

    protected override void OnEnable()
    {
        animationHash.Add(Animator.StringToHash("TB_Idle"));
        animationHash.Add(Animator.StringToHash("TB_Walk"));
        animationHash.Add(Animator.StringToHash("TB_Run"));
    }
}
