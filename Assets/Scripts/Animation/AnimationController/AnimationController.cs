using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] protected Animator animator;

    [Header("ScriptReferences")]
    [SerializeField] protected LgcToComms lgcToComms;

    [Header("ScriptableObjects")]
    [SerializeField] protected AnimationHashSO animationHash;

    [SerializeField] protected PlayerLogic.PlrAnimations[] currentPlrAnim;
    [SerializeField] protected BaseEnemyLogic.EnemyAnimations[] currentEnemyAnim;
    [SerializeField] protected bool[] layerLocked;


    private protected int currentAnimation;


    private void Start()
    {
        
    }

    public virtual void InitializePlayerAnimations(int layer, PlayerLogic.PlrAnimations startingAnim, Animator animator)
    {

    }

    public virtual void InitializeEntityAnimations(int layer, BaseEnemyLogic.EnemyAnimations startingAnim, Animator animator)
    {

    }

    public virtual PlayerLogic.PlrAnimations GetCurrentPlayerAnimation(int layer)
    {
        return currentPlrAnim[layer];
    }

    public virtual BaseEnemyLogic.EnemyAnimations GetCurrentEntityAnimation(int layer)
    {
        return BaseEnemyLogic.EnemyAnimations.NONE;
    }

    public virtual void SetLocked(bool isLock, int layer)
    {

    }

    public virtual void PlayPlayerAnimation(PlayerLogic.PlrAnimations animation, int layer, bool isLock, bool bypassLock, float crossfade)
    {

    }

    public virtual void PlayEntityAnimation(BaseEnemyLogic.EnemyAnimations animation, int layer, bool isLock, bool bypassLock, float crossfade)
    {

    }

    public virtual void DefaultAnimation(int layer)
    {

    }

}
