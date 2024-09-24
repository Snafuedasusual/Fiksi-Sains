using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerAnimationController : AnimationController, IInitializeScript
{
    private readonly int[] animationHashes =
    {
        Animator.StringToHash("Movement_Walk"),
        Animator.StringToHash("Movement_Sprint"),
        Animator.StringToHash("Attack1")
    };

    [SerializeField] PlrEventsComms eventsComms;

    private const int UPPERBODY = 0;
    private const int LWRBODY = 1;

    [SerializeField] RuntimeAnimatorController defaultController;
    [SerializeField] RuntimeAnimatorController controllerOverride;

    public void InitializeScript()
    {
        InitializePlayerAnimations(animator.layerCount, PlayerLogic.PlrAnimations.MOVEMENT_WALK, animator);
        DefaultAnimation(UPPERBODY);
        lgcToComms.PlayerMovementAnimEvent += PlayerMovementAnimEventReceiver;
        lgcToComms.PlayerAttackAnimEvent += PlayerAttackAnimEventReceiver;
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
        PlayPlayerAnimation(e.playThisAnim, LWRBODY, false, false, 0.2f);
        var plrDir = new Vector3(e.xAxis, 0, e.yAxis);
        plrDir = transform.InverseTransformDirection(plrDir);
        animator.SetFloat("XAxis", plrDir.x);
        animator.SetFloat("YAxis", plrDir.z);
        if (e.controller != null) { controllerOverride = e.controller; animator.runtimeAnimatorController = controllerOverride; }
        else if(e.controller == null) { controllerOverride = null; animator.runtimeAnimatorController = defaultController; }
    }


    private void PlayerAttackAnimEventReceiver(object sender, LgcToComms.PlayerAttackAnimEventArgs e)
    {
        if (e.controllerOverride != null) { controllerOverride = e.controllerOverride; animator.runtimeAnimatorController = controllerOverride; }
        else { controllerOverride = null; animator.runtimeAnimatorController = defaultController; }
        PlayPlayerAnimation(e.playThisAnim, UPPERBODY, true, false, 0);
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
        if (controllerOverride == null) animator.runtimeAnimatorController = defaultController;
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


    public void PlayFootstepAudio()
    {
        eventsComms.PlayFootstepAudio();
    }

}
