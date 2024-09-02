using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;
    [SerializeField] GameObject gameOverScreen;

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

    public void ActivateGameOver()
    {
        if(GameManagers.instance.GetGameState() == GameManagers.GameState.Dead)
        {
            gameOverScreen.SetActive(true);
        }
    }

    public void DeactivateGameOver()
    {
        if (GameManagers.instance.GetGameState() != GameManagers.GameState.Dead)
        {
            gameOverScreen.SetActive(false);
        }
    }
}
