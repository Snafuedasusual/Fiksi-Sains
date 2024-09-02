using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerAnimationController : AnimationController, IInitializeScript
{
    private readonly int[] animationHashes =
    {
        Animator.StringToHash("Movement_Walk"),
        Animator.StringToHash("Movement_Sprint")
    };

    private const int UPPERBODY = 0;


    public void InitializeScript()
    {
        InitializePlayerAnimations(animator.layerCount, PlayerLogic.PlrAnimations.MOVEMENT_WALK, animator);
        DefaultAnimation(UPPERBODY);
        lgcToComms.PlayerMovementAnimEvent += PlayerMovementAnimEventReceiver;
    }

    public void DeInitializeScript()
    {
        lgcToComms.PlayerMovementAnimEvent -= PlayerMovementAnimEventReceiver;
    }

    private void OnEnable()
    {
        InitializeScript();
    }
    private void OnDisable()
    {
        DeInitializeScript();
    }
    private void OnDestroy()
    {
        DeInitializeScript() ;
    }

    private void PlayerMovementAnimEventReceiver(object sender, LgcToComms.PlayerSendMovementAnimEventArgs e)
    {
        PlayPlayerAnimation(e.playThisAnim, UPPERBODY, false, false, 0.2f);
        animator.SetFloat("XAxis", e.xAxis);
        animator.SetFloat("YAxis", e.yAxis);
    }


    public override void InitializePlayerAnimations(int layer, PlayerLogic.PlrAnimations startingAnim, Animator animator)
    {
        layerLocked = new bool[layer];
        currentPlrAnim = new PlayerLogic.PlrAnimations[layer];
        for(int i = 0; i < layer; i++)
        {
            layerLocked[i] = false;
            currentPlrAnim[i] = startingAnim;
        }
        
    }

    public override PlayerLogic.PlrAnimations GetCurrentPlayerAnimation(int layer)
    {
        return currentPlrAnim[layer];
    }

    public override void SetLocked(bool isLock, int layer)
    {
        layerLocked[layer] = isLock;
    }

    public override void DefaultAnimation(int layer)
    {
        PlayPlayerAnimation(PlayerLogic.PlrAnimations.MOVEMENT_WALK, layer, false, false, 0.2f);
    }

    public override void PlayPlayerAnimation(PlayerLogic.PlrAnimations animation, int layer, bool isLock, bool bypassLock, float crossfade)
    {

        if(animation == PlayerLogic.PlrAnimations.NONE)
        {
            DefaultAnimation(layer);
            return;
        }
        if (layerLocked[layer] && !bypassLock) return;
        layerLocked[layer] = isLock;

        if (currentPlrAnim[layer] == animation) return;

        currentPlrAnim[layer] = animation;
        animator.CrossFade(animationHashes[(int)currentPlrAnim[layer]], crossfade, layer);
    }

}
