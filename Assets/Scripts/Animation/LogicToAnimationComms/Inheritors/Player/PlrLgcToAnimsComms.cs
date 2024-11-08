using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlrLgcToAnimsComms : LgcToComms, IInitializeScript
{
    [Header("ScriptReferences")]
    [SerializeField] PlayerLogic plrLogic;

    public void DeInitializeScript()
    {
        plrLogic.PlayThisMovementAnim -= PlayThisMovementAnimReceiver;
    }

    public void InitializeScript()
    {
        plrLogic.PlayThisMovementAnim += PlayThisMovementAnimReceiver;
        plrLogic.PlayThisAttackAnim += PlayThisAttackAnimReceiver;
    }


    private void OnEnable()
    {
        InitializeScript();
    }
    private void OnDisable()
    {
        InitializeScript();
    }
    private void OnDestroy()
    {
        DeInitializeScript();
    }

    private void PlayThisMovementAnimReceiver(object sender, PlayerLogic.PlayThisMovementAnimArgs e)
    {
        //Debug.Log($"{e.xAxis}, {e.yAxis}");
        PlayerSendMovementAnimEvent(new PlayerSendMovementAnimEventArgs { playThisAnim = e.playThisAnim, xAxis = e.xAxis, yAxis = e.yAxis, controller = e.controller });
    }

    private void PlayThisAttackAnimReceiver(object sender, PlayerLogic.PlayThisAttackAnimArgs e)
    {
        PlayerSendAttackAnimEvent(new PlayerAttackAnimEventArgs { controllerOverride = e.controller, playThisAnim = e.anim});
    }


}
