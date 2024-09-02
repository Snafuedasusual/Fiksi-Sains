using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPasswordUI : MonoBehaviour, IInteraction
{
    [SerializeField] GameObject wallPasswordUI;

    IEnumerator IsInteractionDebounce;
    IEnumerator InteractionDebounce()
    {
        var debTime = 0f;
        var debRate = 0.15f;
        while(debTime < debRate)
        {
            debTime += Time.deltaTime;
            yield return null;
        }
        IsInteractionDebounce = null;
    }

    private bool isInteracting = false;

    public void OnInteract(Transform plr)
    {
        if(isInteracting == false && IsInteractionDebounce == null)
        {
            isInteracting = true;
            IsInteractionDebounce = InteractionDebounce();
            StartCoroutine(IsInteractionDebounce);
            var spawnedUI = Instantiate(wallPasswordUI);
            InteractableUIManager.instance.ActivateInteractableUI(spawnedUI);
            GameManagers.instance.SetStateToOnUI();

            IUIObjectives iUI = spawnedUI.TryGetComponent(out iUI) ? iUI : null;
            iUI.AddListener(transform.gameObject);

            RectTransform rtUI = spawnedUI.TryGetComponent(out rtUI) ? rtUI : null;
            rtUI.localScale = Vector3.one;
        }
        else if(isInteracting == true && IsInteractionDebounce == null)
        {
            isInteracting = false;
            IsInteractionDebounce = InteractionDebounce();
            StartCoroutine(IsInteractionDebounce);
            InteractableUIManager.instance.DeactivateInteractableUI();
            GameManagers.instance.SetStateToPlaying();
        }
    }

    public void OnDetected(Transform plr)
    {
        throw new System.NotImplementedException();
    }

}
