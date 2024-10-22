using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjIndicatorManager : MonoBehaviour
{
    public static ObjIndicatorManager instance;
    private void Awake()
    {
        if (instance != this && instance != null) return;
        if (instance == null) instance = this;
    }

    [SerializeField] Transform targetObj;
    [SerializeField] GameObject arrow;

    RectTransform arrowLogicRect;
    [SerializeField] RectTransform arrowVisRect;


    float defaultDeltaSize = 45f;
    float onScreenDeltaSize = 30f;

    public void SetTargetObj(Transform targetObj)
    {
        arrowLogicRect = arrow.GetComponent<RectTransform>();
        for(int i = 0; i < arrow.transform.childCount; i++)
        {
            if (arrow.transform.GetChild(i).TryGetComponent(out RectTransform rectTrans)) { arrowVisRect = rectTrans; break; }
        }

        IObjectiveSection objSection = targetObj.TryGetComponent(out IObjectiveSection objectif) ? objectif : null;
        if (objSection == null) return;
        if (objSection.CanHaveIndicator() == IObjectiveSection.HasIndicator.Yes)
        {
            this.targetObj = targetObj;
            arrow.SetActive(true);
            ArrowActive = StartCoroutine(StartArrowActive());
        }
        else
        {
            NullifyTargetObj();
        }
        
    }

    public void NullifyTargetObj()
    {
        targetObj = null;
        arrow.SetActive(false);
        if(ArrowActive != null) { StopCoroutine(ArrowActive); ArrowActive = null; }
    }

    Coroutine ArrowActive;
    IEnumerator StartArrowActive()
    {
        while (targetObj != null )
        {
            var player = GameManagers.instance.GetPlayer().transform;
            var cameraTransform = Camera.main.transform;
            if (player == null) { ArrowActive = null; NullifyTargetObj(); yield break; }

            Vector3 worldPos = targetObj.TransformPoint(Vector3.zero);
            var direction = targetObj.position - cameraTransform.position;


            float angle = (Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg - 90) % 360;

            arrowLogicRect.eulerAngles = Vector3.forward * angle;

            var borderSize = 50f;
            Vector3 targetToScreen = Camera.main.WorldToScreenPoint(targetObj.position);
            bool isOffScreen = targetToScreen.x <= (Screen.width - Screen.width) + borderSize || targetToScreen.x >= Screen.width - borderSize || targetToScreen.y <= (Screen.height - Screen.height) + borderSize || targetToScreen.y >= Screen.height - borderSize;


            if (isOffScreen == true)
            {
                arrowVisRect.sizeDelta = new Vector2(defaultDeltaSize, defaultDeltaSize);
                Vector3 cappedTargetToScreen = targetToScreen;
                if (cappedTargetToScreen.x <= borderSize) cappedTargetToScreen.x = borderSize;
                if (cappedTargetToScreen.x >= Screen.width - borderSize) cappedTargetToScreen.x = Screen.width - borderSize;
                if (cappedTargetToScreen.y <= borderSize) cappedTargetToScreen.y = borderSize;
                if (cappedTargetToScreen.y >= Screen.height - borderSize) cappedTargetToScreen.y = Screen.height - borderSize;

                arrowLogicRect.position = cappedTargetToScreen;
            }
            else
            {
                arrowVisRect.sizeDelta = new Vector2(onScreenDeltaSize, onScreenDeltaSize);
                arrowLogicRect.position = targetToScreen;
            }

            yield return null;
        }
    }
}
