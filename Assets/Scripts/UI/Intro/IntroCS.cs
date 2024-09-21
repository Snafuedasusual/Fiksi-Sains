using System.Collections;
using System.Collections.Generic;
using TMPro;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class IntroCS : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] textsMain = new TextMeshProUGUI[5];
    [SerializeField] TextMeshProUGUI textIntro;
    private int currentIndex = 0;
    [SerializeField] TextAsset textAssetMain;
    [SerializeField] TextAsset textAssetIntro;
    [SerializeField] RawImage blackImageFull;
    [SerializeField] RawImage blackImageHalf;
    [SerializeField] RawImage panel;
    [SerializeField] GameObject controlsUI;

    private Story storyMain;
    private Story storyIntro;

    Coroutine TypeProcess;
    IEnumerator StartTypeProcess(string text, Story story)
    {
        Debug.Log("Wrtiting");
        var typeTime = 0f;
        var typeRate = 0.75f;
        if(story == storyMain)
        {
            for (int i = 0; i < text.ToCharArray().Length; i++)
            {
                textsMain[currentIndex].text += text.ToCharArray()[i];
                textsMain[currentIndex].alignment = TextAlignmentOptions.TopLeft;
                typeTime = 0f;
                while (typeTime < typeRate)
                {
                    typeTime += Time.deltaTime * 10f;
                    yield return null;
                }
            }
        }
        else
        {
            for (int i = 0; i < text.ToCharArray().Length; i++)
            {
                textIntro.text += text.ToCharArray()[i];
                textIntro.alignment = TextAlignmentOptions.Center;
                typeTime = 0f;
                while (typeTime < typeRate)
                {
                    typeTime += Time.deltaTime * 10f;
                    yield return null;
                }
            }
        }
        TypeProcess = null;
        DelayAndFadeout = StartCoroutine(StartDelayAndFadeout(story));
    }

    Coroutine DelayAndFadeout;
    IEnumerator StartDelayAndFadeout(Story story)
    {
        Debug.Log("Fading");
        var timer = 0f;
        var maxTimerMain = 4.5f;
        var maxTimerIntro = 2.5f;
        if(story == storyMain)
        {
            if (currentIndex == textsMain.Length - 1)
            {
                currentIndex = 0;
                while (timer < maxTimerMain)
                {
                    timer += Time.deltaTime;
                    yield return null;
                }
                for (float i = 0f; i < 1f; i += Time.deltaTime)
                {
                    blackImageFull.color = new Color(blackImageFull.color.r, blackImageFull.color.g, blackImageFull.color.b, i);
                    yield return null;
                }
                EmptyTexts();
                ContinueStoryMain();
                DelayAndFadeout = null;
                blackImageFull.color = new Color(blackImageFull.color.r, blackImageFull.color.g, blackImageFull.color.b, 0f);
            }
            else if (currentIndex < textsMain.Length - 1)
            {
                currentIndex++;
                ContinueStoryMain();
                DelayAndFadeout = null;
            }
        }
        else
        {
            while (timer < maxTimerIntro)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            for (float i = 0f; i < 1f; i += Time.deltaTime)
            {
                blackImageFull.color = new Color(blackImageFull.color.r, blackImageFull.color.g, blackImageFull.color.b, i);
                yield return null;
            }
            EmptyTexts();
            blackImageFull.color = new Color(blackImageFull.color.r, blackImageFull.color.g, blackImageFull.color.b, 0f);
            ContinueStoryIntro();
            DelayAndFadeout = null;
        }
    }

    Coroutine FinalDelayAndFadeout;
    IEnumerator StartFinalDelayAndFadeOut(Story story)
    {
        Debug.Log("FinalFade");
        var timer = 0f;
        var maxTimer0 = 4.5f;
        var maxTimer1 = 2f;
        if (story == storyIntro)
        {
            for (float i = 0f; i < 1f; i += Time.deltaTime)
            {
                blackImageFull.color = new Color(blackImageFull.color.r, blackImageFull.color.g, blackImageFull.color.b, i);
                yield return null;
            }
            EmptyTexts();
            blackImageFull.color = new Color(blackImageFull.color.r, blackImageFull.color.g, blackImageFull.color.b, 0f);
            ContinueStoryMain();
            FinalDelayAndFadeout = null;
        }
        else
        {
            while (timer < maxTimer1)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            timer = 0f;
            for (float i = 0f; i < 1f; i += Time.deltaTime)
            {
                blackImageHalf.color = new Color(blackImageHalf.color.r, blackImageHalf.color.g, blackImageHalf.color.b, i);
                yield return null;
            }
            while(timer < maxTimer0)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            for (float i = 0f; i < 1f; i += Time.deltaTime)
            {
                blackImageFull.color = new Color(blackImageFull.color.r, blackImageFull.color.g, blackImageFull.color.b, i);
                yield return null;
            }
            timer = 0f;
            EmptyTexts();
            blackImageFull.color = new Color(blackImageFull.color.r, blackImageFull.color.g, blackImageFull.color.b, 0f);
            blackImageHalf.color = new Color(blackImageHalf.color.r, blackImageHalf.color.g, blackImageHalf.color.b, 0f);
            for(float i = 1f; i > 0f; i -= Time.deltaTime)
            {
                panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, i);
            }
            FinalDelayAndFadeout = null;
            ShowControls();
        }
    }


    Coroutine ControlsShow;
    IEnumerator StartControlShow()
    {
        var timer = 0f;
        var maxTimer = 8f;
        controlsUI.gameObject.SetActive(true);
        while(timer < maxTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        controlsUI.gameObject.SetActive(false);
        ControlsShow = null;
        CutsceneUIManager.instance.DeactivateCutscene();
        Destroy(gameObject);
    }

    private void ShowControls()
    {
        if (ControlsShow != null) return;
        ControlsShow = StartCoroutine(StartControlShow());
    }

    private void StartTheProcess()
    {
        storyMain = new Story(textAssetMain.text);
        storyIntro = new Story(textAssetIntro.text);
        ContinueStoryIntro();   
    }


    private void ContinueStoryIntro()
    {
        if (storyIntro.canContinue)
        {
            if (TypeProcess != null && DelayAndFadeout != null) return;
            TypeProcess = StartCoroutine(StartTypeProcess(storyIntro.Continue(), storyIntro));
        }
        else
        {
            //FinalDelayAndFadeout = StartCoroutine(StartDelayAndFadeout(storyIntro));
            ContinueStoryMain();
        }
    }

    private void ContinueStoryMain()
    {
        if (storyMain.canContinue)
        {
            if (TypeProcess != null && DelayAndFadeout != null) return;
            TypeProcess = StartCoroutine(StartTypeProcess(storyMain.Continue(), storyMain));
        }
        else
        {
            FinalDelayAndFadeout = StartCoroutine(StartFinalDelayAndFadeOut(storyMain));
        }
    }

    private void EmptyTexts()
    {
        for (int i = 0; i < textsMain.Length; i++)
        {
            textsMain[i].text = string.Empty;
        }
        textIntro.text = string.Empty;
    }

    private void OnEnable()
    {
        currentIndex = 0;
        TypeProcess = null;
        DelayAndFadeout = null;
        StartTheProcess();
    }
}
