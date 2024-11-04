using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;

public class SubtitleManager : MonoBehaviour, IMakeSounds
{
    public static SubtitleManager instance;

    [SerializeField] private AudioSource audSrc;
    [SerializeField] private AudioClip typing;
    [SerializeField] private TextMeshProUGUI textUI;
    private Story currentStory;
    private bool dialogueFinished = true;


    public bool GetIfDialogueFinished()
    {
        return dialogueFinished;
    }


    private void Awake()
    {
        if(instance != null && instance != this)
        {

        }
        else if(instance == null)
        {
            instance = this;
        }
    }

    public void ActivateSubtitle(TextAsset dialogue)
    {
        if (GameManagers.instance.GetGameState() != GameManagers.GameState.Playing) { textUI.gameObject.SetActive(false); return; }
        textUI.gameObject.SetActive(true);
        EnterSubtitleText(dialogue);

    }


    IEnumerator IsEnterSubtitleTextDebounceStart;
    IEnumerator EnterSubtitleTextDebounceStart()
    {
        var debTime = 0f;
        var debRate = 0.4f;
        while (debTime < debRate)
        {
            debTime += Time.deltaTime;
            yield return null;
        }
        IsEnterSubtitleTextDebounceStart = null;
    }
    private void EnterSubtitleText(TextAsset dialogue)
    {
        if (IsEnterSubtitleTextDebounceStart != null) return;
        IsEnterSubtitleTextDebounceStart = EnterSubtitleTextDebounceStart();
        StartCoroutine(IsEnterSubtitleTextDebounceStart);
        currentStory = new Story(dialogue.text);
        ContinueText();
    }


    private void ContinueText()
    {
        if (currentStory.canContinue)
        {
            dialogueFinished = false;
            if (IsTypingStarted == null)
            {
                if (IsDeleteCountdownStarted != null) { StopCoroutine(IsDeleteCountdownStarted); IsDeleteCountdownStarted = null; }
                textUI.text = string.Empty;
                IsTypingStarted = TypingStart(currentStory.Continue());
                StartCoroutine(IsTypingStarted);
                IsDeleteCountdownStarted = DeleteCountdownStart();
            }
            else
            {
                StopCoroutine(IsTypingStarted);
                IsTypingStarted = null;
                IsTypingStarted = TypingStart(currentStory.Continue());
                IsDeleteCountdownStarted = null;
                IsDeleteCountdownStarted = DeleteCountdownStart();

                StartCoroutine(IsTypingStarted);
            }
        }
        else
        {
            dialogueFinished = true;
            DisableText();
        }
    }

    IEnumerator IsTypingStarted;
    IEnumerator TypingStart(string text)
    {
        audSrc.loop = true;
        RequestPlaySFXAudioClip(audSrc, typing);
        var typeTime = 0f;
        var typeRate = 0.001f;
        for (int i = 0; i < text.ToCharArray().Length; i++)
        {
            textUI.text += text.ToCharArray()[i];
            textUI.alignment = TextAlignmentOptions.Center;
            typeTime = 0f;
            while(typeTime < typeRate)
            {
                typeTime += Time.deltaTime * 10f;
                yield return null;
            }
        }
        IsTypingStarted = null;
        RequestStopAudioSource(audSrc);
        StartCoroutine(IsDeleteCountdownStarted);
    }

    IEnumerator IsDeleteCountdownStarted;
    IEnumerator DeleteCountdownStart()
    {
        var countTime = 0f;
        var countRate = 3.5f;
        while (countTime < countRate)
        {
            countTime += Time.deltaTime;
            yield return null;
        }
        IsDeleteCountdownStarted = null;
        ContinueText();
    }

    public void EmptyTheText()
    {
        textUI.text = string.Empty;
        dialogueFinished = true;
    }

    public void DisableText()
    {
        dialogueFinished = true;
        currentStory = null;
        textUI.text = string.Empty;
        textUI.gameObject.SetActive(false);
    }

    public void RequestPlaySFXAudioClip(AudioSource audSrc, AudioClip audClip)
    {
        SFXManager.instance.PlayAudio(audSrc, audClip);

    }

    public void RequestStopAudioSource(AudioSource audSrc)
    {
        SFXManager.instance.StopAudio(audSrc);
    }

}
