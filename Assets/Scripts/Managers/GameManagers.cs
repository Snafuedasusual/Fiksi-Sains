using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagers : MonoBehaviour
{
    public static GameManagers instance;
    [Header("Level")]
    [SerializeField] Transform loadingSpot;
    [SerializeField] GameObject[] sections;
    [SerializeField] BaseHandler[] handlers;
    [SerializeField] int currentLevel;


    void OnPlayerDeath()
    {
        handlers[currentLevel - 1].Restart();
    }

    public void OnLevelChange(Transform plr)
    {
        if(currentLevel < sections.Length)
        {
            currentLevel++;
           if(IsLoading == null)
           {
                sections[currentLevel - 2].gameObject.SetActive(false);
                IsLoading = LoadingLevel(plr);
                StartCoroutine(IsLoading);
           }
        }
        else
        {
            Debug.Log("End Of The Line!");
        }
    }

    IEnumerator IsLoading;
    IEnumerator LoadingLevel(Transform plr)
    {
        plr.transform.position = loadingSpot.position;
        plr.transform.GetComponent<PlayerLogic>().plrState = PlayerLogic.PlayerStates.Hiding;
        var loadTime = 0f;
        var loadRate = 0.07f;

        for(int i = 0; i < sections[currentLevel-1].transform.childCount; i++)
        {
            while (loadTime < loadRate)
            {
                loadTime += Time.deltaTime;
                yield return 0;
            }
            loadTime = 0f;
            sections[currentLevel - 1].transform.GetChild(i).transform.gameObject.SetActive(true);
            if (sections[currentLevel - 1].transform.GetChild(i).childCount > 0)
            {
                sections[currentLevel - 1].transform.GetChild(i).transform.gameObject.SetActive(true);
                for(int j = 0; j < sections[currentLevel - 1].transform.GetChild(i).childCount; j++)
                {
                    while (loadTime < loadRate)
                    {
                        loadTime += Time.deltaTime;
                        yield return 0;
                    }
                    loadTime = 0f;
                    sections[currentLevel - 1].transform.GetChild(i).GetChild(j).gameObject.SetActive(true);
                }
            }
        }
        plr.transform.GetComponent<PlayerLogic>().plrState = PlayerLogic.PlayerStates.Idle;
        handlers[currentLevel - 1].player = plr.gameObject;
        IsLoading = null;
        OnPlayerDeath();
    }

    private void Start()
    {
        instance = this;
        GameObject plr = GameObject.FindGameObjectWithTag("Player");
        handlers[currentLevel - 1].player = plr;
        //StartCoroutine(LoadingLevel(plr.transform));
    }

}
