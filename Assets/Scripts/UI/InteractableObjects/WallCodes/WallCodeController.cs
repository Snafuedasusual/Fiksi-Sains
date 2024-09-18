using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WallCodeController : MonoBehaviour, IUIObjectives, IInitializeScript
{
    [SerializeField] private int[] codePattern;
    [SerializeField] private GameObject listener;
    [SerializeField] private EventCommsWallCodes eventComms;
    [SerializeField] private List<int> playerCodePattern;
    [SerializeField] private TextMeshProUGUI textScreen;
    [SerializeField] private TextMeshProUGUI codeClue;


    public void InitializeScript()
    {
        eventComms.SendCodeInputEvent += SendCodeInputReceiver;
    }


    public void DeInitializeScript()
    {
        eventComms.SendCodeInputEvent -= SendCodeInputReceiver;
    }


    private void InitializeCodePattern()
    {
        var minCodeKeyValue = 1;
        var maxCodeKeyValue = 10;
        codePattern = new int[6];
        playerCodePattern = new List<int>();
        for(int i = 0; i < codePattern.Length; i++)
        {
            codePattern[i] = Random.Range(minCodeKeyValue, maxCodeKeyValue);
        }
    }


    private void OnEnable()
    {
        DisableKeyCodeInput();
        InitializeCodePattern();
        InitializeScript();
        CodeClue();
    }

    private void OnDisable()
    {
        DeInitializeScript();
        listener = null;
    }
    private void OnDestroy()
    {
        DeInitializeScript();
        listener = null;
    }



    private void SendCodeInputReceiver(object sender, EventCommsWallCodes.SendCodeInputArgs e)
    {
        InputDetector(e.inputVal);
    }

    IEnumerator IsInputDebounce;
    IEnumerator InputDebounce()
    {
        var debTime = 0f;
        var debRate = 0.15f;
        while(debTime < debRate)
        {
            debTime += Time.deltaTime;
            yield return null;
        }
        IsInputDebounce = null;
    }
    private void InputDetector(int input)
    {
        if (IsInputDebounce != null) return;
        IsInputDebounce = InputDebounce();
        StartCoroutine(IsInputDebounce);
        playerCodePattern.Add(input);
        textScreen.text += "*";
        if (playerCodePattern.Count == codePattern.Length)
        {
            DisableKeyCodeInput();
            CheckKeyCodes();
        }
    }

    private void CheckKeyCodes()
    {
        for (int i = 0; i < codePattern.Length; i++)
        {
            if (codePattern[i] != playerCodePattern[i]) { playerCodePattern.Clear(); EnableKeyCodeInput(); textScreen.text = string.Empty; return; }
            if (codePattern[i] == playerCodePattern[i]) { }
        }
        Completed();
    }
    
    private void DisableKeyCodeInput()
    {
        eventComms.DisableCodeInput();
    }

    private void EnableKeyCodeInput()
    {
        eventComms.EnableCodeInput();
    }


    IEnumerator IsCodeClueStarted;
    IEnumerator CodeClueStart()
    {
        var startdelayTime = 0f;
        var startdelayRate = 0.7f;
        var numberShowTime = 0f;
        var numberShowRate = 1f;
        var partialDelayTime = 0f;
        var partialDelayRate = 0.2f;

        while(startdelayTime < startdelayRate)
        {
            startdelayTime += Time.deltaTime;
            yield return null;
        }
        for (int i = 0; i < codePattern.Length; i++)
        {
            codeClue.text = codePattern[i].ToString();
            numberShowTime = 0f;
            while(numberShowTime < numberShowRate)
            {
                numberShowTime += Time.deltaTime;
                yield return null;
            }
            codeClue.text = string.Empty;
            partialDelayTime = 0f;
            while (partialDelayTime < partialDelayRate)
            {
                partialDelayTime += Time.deltaTime;
                yield return null;
            }
            if ( i == codePattern.Length - 1) { codeClue.text = string.Empty; IsCodeClueStarted = null; EnableKeyCodeInput(); yield break; }
        }
    }
    private void CodeClue()
    {
        if (IsCodeClueStarted != null) return;
        IsCodeClueStarted = CodeClueStart();
        StartCoroutine(IsCodeClueStarted);
    }



    IEnumerator IsDoneVisualStarted;
    IEnumerator DoneVisualStart()
    {
        var debTime = 0f;
        var debRate = 1.25f;
        while(debTime < debRate)
        {
            debTime += Time.deltaTime;
            yield return null;
        }
        IsDoneVisualStarted = null;
        Destroy(transform.gameObject);
        GameManagers.instance.SetStateToPlaying();
    }
    private void DoneVisual()
    {
        if (IsDoneVisualStarted != null) return;
        IsDoneVisualStarted = DoneVisualStart();
        StartCoroutine(IsDoneVisualStarted);
        textScreen.text = "Success";
        DisableKeyCodeInput();
    }

    private void Completed()
    {
        if (!listener.TryGetComponent(out IObjectiveSection objSection)) return;
        objSection.OnDone();
        DoneVisual();
    }

    public void AddListener(GameObject listener)
    {
        this.listener = listener;
    }

}