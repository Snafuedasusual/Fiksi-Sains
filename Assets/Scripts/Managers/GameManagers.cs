using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagers : MonoBehaviour
{
    public static GameManagers instance;
    [Header("Level")]
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
                handlers[currentLevel - 1].player = plr.gameObject;
                OnPlayerDeath();
                //IsLoading = LoadingLevel(plr);
                //StartCoroutine(IsLoading);
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
        float loadTime = 0;
        float loadRate = 0.2f;

        for (int i = 0; i < sections[currentLevel - 1].transform.childCount; i++)
        {
            while (loadTime < loadRate)
            {
                loadTime += Time.deltaTime;
                yield return 0;
            }
            if (sections[currentLevel - 1].transform.GetChild(i).gameObject.activeSelf == false)
            {
                sections[currentLevel - 1].transform.GetChild(i).gameObject.SetActive(true);
            }
            loadTime = 0;
            Debug.Log(i);

        }
        IsLoading = null;
        OnPlayerDeath();
    }

    private void Start()
    {
        instance = this;
        GameObject plr = GameObject.FindGameObjectWithTag("Player");
        handlers[currentLevel - 1].player = plr;
        OnPlayerDeath();
    }
}
