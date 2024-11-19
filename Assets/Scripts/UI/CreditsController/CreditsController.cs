using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreditsController : MonoBehaviour, IMakeSounds
{
    [SerializeField] TextMeshProUGUI[] titleText;
    [SerializeField] TextMeshProUGUI[] creditsText0;
    [SerializeField] TextMeshProUGUI[] creditsText1;
    [SerializeField] TextMeshProUGUI[] creditsText2;
    [SerializeField] TextMeshProUGUI[] creditsText3;
    [SerializeField] TextMeshProUGUI[] thankYouText;
    private TextMeshProUGUI[][] listText = new TextMeshProUGUI[6][];
    private int currentElement = 0;

    [SerializeField] AudioClip music;

    Coroutine DelayBeforeStart;
    IEnumerator StartDelayBeforeStart()
    {
        var timer = 0f;
        var maxTimer = 1.75f;
        while(timer < maxTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        DelayBeforeStart = null;
        FadeInTexts();
    }

    Coroutine TextFadingIn;
    IEnumerator StartTextFadingInProcess(TextMeshProUGUI[] texts)
    {
        var timer = 0f;
        var maxTimer = 3f;
        if(texts.Length == 1)
        {
            for(float i = 0; i < 1; i += Time.deltaTime)
            {
                texts[0].color = new Color(texts[0].color.r, texts[0].color.g, texts[0].color.b, i);
                yield return null;
            }
        }
        else
        {
            for(float i = 0; i < 1; i += Time.deltaTime)
            {
                for(int j = 0; j < texts.Length; j++)
                {
                    texts[j].color = new Color(texts[j].color.r, texts[j].color.g, texts[j].color.b, i);
                }
                yield return null;
            }
        }
        while(timer < maxTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        TextFadingIn = null;
        TextFadingOut = StartCoroutine(StartTextFadingOutProcess(texts));
    }

    Coroutine DelayBetweenFades;
    IEnumerator StartDelayBetweenFades()
    {
        var timer = 0f;
        var maxTimer = 0.75f;
        yield return null;
    }

    Coroutine TextFadingOut;
    IEnumerator StartTextFadingOutProcess(TextMeshProUGUI[] texts)
    {
        var timer = 0f;
        var maxTimer = 1f;
        if (texts.Length == 1)
        {
            for (float i = 1; i > 0; i -= Time.deltaTime)
            {
                texts[0].color = new Color(texts[0].color.r, texts[0].color.g, texts[0].color.b, i);
                yield return null;
            }
        }
        else
        {
            for (float i = 1; i > 0; i -= Time.deltaTime)
            {
                for (int j = 0; j < texts.Length; j++)
                {
                    texts[j].color = new Color(texts[j].color.r, texts[j].color.g, texts[j].color.b, i);
                }
                yield return null;
            }
        }
        while(timer < maxTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        if(currentElement == listText.Length - 1)
        {
            GameManagers.instance.SetStateToMainMenu();
            MainMenuManager.instance.ActivateMenu();
            TextFadingOut = null;
            Destroy(gameObject);
            yield break;
        }
        else
        {
            currentElement++;
            TextFadingOut = null;
            FadeInTexts();
            yield break;
        }
    }

    private void FadeInTexts()
    {
        TextFadingIn = StartCoroutine(StartTextFadingInProcess(listText[currentElement]));
    }

    private void OnEnable()
    {
        listText[0] = titleText;
        listText[1] = creditsText0;
        listText[2] = creditsText1;
        listText[3] = creditsText2;
        listText[4] = creditsText3;
        listText[5] = thankYouText;
        TextFadingIn = null;
        TextFadingOut = null;
        RequestPlayGenericMusicAudioClip(music);
        DelayBeforeStart = StartCoroutine(StartDelayBeforeStart());
    }

    public void RequestPlaySFXAudioClip(AudioSource audSrc, AudioClip audClip)
    {
        throw new System.NotImplementedException();
    }

    public void RequestPlayGenericMusicAudioClip(AudioClip audClip)
    {
        GenericMusicManager.instance.PlayGenericMusic(audClip);
    }

    public void RequestStopGenericMusicAudioClip()
    {
        GenericMusicManager.instance.StopMusic();
    }
}
