using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnterPlr : StateMachineBehaviour
{
    public PlayerLogic.PlrAnimations nextAnim;
    public float crossfade;
    public bool lockLayer;
    public bool bypassLock;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Coroutine IsStarted;
        IsStarted = GameManagers.instance.StartCoroutine(WaitTillExit());
        IEnumerator WaitTillExit()
        {
            Debug.Log(stateInfo.length);
            var waitTime = 0f;
            var waitRate = (stateInfo.length) - crossfade;
            while(waitTime < waitRate)
            {
                if(GameManagers.instance.GetGameState() == GameManagers.GameState.Playing || GameManagers.instance.GetGameState() == GameManagers.GameState.OnMenu)
                {
                    waitTime += Time.deltaTime;
                }
                else
                {

                }
                yield return null;
            }
            AnimationController target = animator.GetComponent<AnimationController>();
            target.SetLocked(false, layerIndex);
            target.PlayPlayerAnimation(nextAnim, layerIndex, lockLayer, bypassLock, crossfade);
        }
    }
}
