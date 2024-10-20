using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnterEntitiy : StateMachineBehaviour
{
    public BaseEnemyLogic.EnemyAnimations nextAnim;
    public float crossfade;
    public bool lockLayer;
    public bool bypassLock;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Coroutine IsStarted;
        IsStarted = GameManagers.instance.StartCoroutine(WaitTillExit());
        IEnumerator WaitTillExit()
        {
            if (animator == null) { IsStarted = null; yield break; }
            Debug.Log(stateInfo.length);
            var waitTime = 0f;
            var waitRate = (stateInfo.length) - crossfade;
            while (waitTime < waitRate)
            {
                if(animator == null) { IsStarted = null; yield break; }
                if (GameManagers.instance.GetGameState() == GameManagers.GameState.Playing || GameManagers.instance.GetGameState() == GameManagers.GameState.OnMenu)
                {
                    waitTime += Time.deltaTime;
                }
                else
                {

                }
                yield return null;
            }
            if(animator == null) { IsStarted = null; yield break; }
            AnimationController target = animator.TryGetComponent(out AnimationController animCtrl) ? animCtrl : null;
            if (target == null) { IsStarted = null; yield break; }
            target.SetLocked(false, layerIndex);
            target.PlayEntityAnimation(nextAnim, layerIndex, lockLayer, bypassLock, crossfade);
        }
    }
}
