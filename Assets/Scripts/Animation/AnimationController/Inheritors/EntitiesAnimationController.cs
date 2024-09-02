using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitiesAnimationController : AnimationController, IInitializeScript
{
    private readonly int[] animationHashes =
    {
        Animator.StringToHash("Idle"),
        Animator.StringToHash("Walk"),
        Animator.StringToHash("Run")
    };

    private int currentAnimState;


    public void DeInitializeScript()
    {
        lgcToComms.EnemyMovementAnimSendEvent -= EnemyMovementAnimSendEventReceiver;
    }


    public void InitializeScript()
    {
        InitializeEntityAnimations(animator.layerCount, BaseEnemyLogic.EnemyAnimations.IDLE, animator);
        DefaultAnimation(0);
        lgcToComms.EnemyMovementAnimSendEvent += EnemyMovementAnimSendEventReceiver;
    }




    private void OnEnable()
    {
        InitializeScript();
    }



    private void EnemyMovementAnimSendEventReceiver(object sender, LgcToComms.EnemyMovementAnimSendEventArgs e)
    {
        PlayEntityAnimation(e.anim, 0, false, false, 0.2f);
    }


    public override void InitializeEntityAnimations(int layer, BaseEnemyLogic.EnemyAnimations startingAnim, Animator animator)
    {
        layerLocked = new bool[layer];
        currentEnemyAnim = new BaseEnemyLogic.EnemyAnimations[layer];
        for (int i = 0; i < layer; i++)
        {
            layerLocked[i] = false;
            currentEnemyAnim[i] = startingAnim;
        }
    }

    public override BaseEnemyLogic.EnemyAnimations GetCurrentEntityAnimation(int layer)
    {
        return currentEnemyAnim[layer];
    }

    public override void SetLocked(bool isLock, int layer)
    {
        layerLocked[layer] = isLock;
    }

    public override void PlayEntityAnimation(BaseEnemyLogic.EnemyAnimations animation, int layer, bool isLock, bool bypassLock, float crossfade)
    {
        if (animation == BaseEnemyLogic.EnemyAnimations.NONE)
        {
            DefaultAnimation(layer);
            return;
        }
        if (layerLocked[layer] && !bypassLock) return;
        layerLocked[layer] = isLock;

        if (currentEnemyAnim[layer] == animation) return;

        currentEnemyAnim[layer] = animation;
        animator.CrossFade(animationHashes[(int)currentEnemyAnim[layer]], crossfade, layer);
    }

    public override void DefaultAnimation(int layer)
    {
        PlayEntityAnimation(BaseEnemyLogic.EnemyAnimations.IDLE, layer, false, false, 0.2f);
    }

}
