using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End_CScenes : MonoBehaviour, IScriptedEvents, IMakeSounds, IInitializeScript
{
    [SerializeField] SectionEventComms sectionEventComms;
    [SerializeField] private IScriptedEvents.Triggered isTriggered;

    [SerializeField] AudioSource audSrc;
    [SerializeField] AudioClip audClip;

    [Header("Components")]
    [SerializeField] GameObject human;
    [SerializeField] GameObject tntcl;
    [SerializeField] GameObject hole;
    [SerializeField] Transform humanPos;
    [SerializeField] Transform tntclPos;
    [SerializeField] Vector3 humanAngle;
    [SerializeField] Vector3 tntclAngle;
    [SerializeField] GameObject cVC;
    [SerializeField] ParticleSystem partclSys;


    private PlayerLogic playerLogic;


    public void InitializeScript()
    {
        sectionEventComms.ScriptedEventPlayer += ScriptedEventPlayerReceiver;
    }

    public void DeInitializeScript()
    {
        sectionEventComms.ScriptedEventPlayer -= ScriptedEventPlayerReceiver;
    }

    void OnEnable()
    {
        InitializeScript();
    }

    private void OnDisable()
    {
        DeInitializeScript();
    }

    private void ScriptedEventPlayerReceiver(object sender, SectionEventComms.ScriptedEventPlayerArgs e)
    {
        if (!e.obj.TryGetComponent(out PlayerLogic plrLgc)) return;
        playerLogic = plrLgc;
        Trigger();
    }

    public void ResetTrigger()
    {
        isTriggered = IScriptedEvents.Triggered.NotTriggered;
    }


    Coroutine IsCountdownToDisable;
    IEnumerator StartCountdownToDisable()
    {
        var count = 0f;
        var maxCount = 7.5f;
        while(count < maxCount)
        {
            count += Time.deltaTime;
            yield return null;
        }
        IsCountdownToDisable = null;
        human.SetActive(false);
        tntcl.SetActive(false);
    }
    public void Trigger()
    {
        if (isTriggered == IScriptedEvents.Triggered.HasTriggered) return;
        if (IsCountdownToDisable != null) return;
        if (playerLogic != null) playerLogic.NullifyState();
        IsCountdownToDisable = StartCoroutine(StartCountdownToDisable());
        partclSys.time = 0f;
        partclSys.Play();
        SetOriginalPos();
        cVC.gameObject.SetActive(true);
        human.SetActive(true);
        tntcl.SetActive(true);
        hole.SetActive(true);
        RequestPlaySFXAudioClip(audSrc, audClip);
    }

    void SetOriginalPos()
    {
        human.transform.position = humanPos.position;
        human.transform.eulerAngles = humanAngle;
        tntcl.transform.position = tntclPos.position;
        tntcl.transform.eulerAngles = tntclAngle;
    }

    public void RequestPlaySFXAudioClip(AudioSource audSrc, AudioClip audClip)
    {
        SFXManager.instance.PlayAudio(audSrc, audClip);
    }
}
