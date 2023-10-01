using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerEntity.OnPlayerDeath += GameOver;
    }
    private void OnDisable()
    {
        PlayerEntity.OnPlayerDeath -= GameOver;
    }

    void GameOver()
    {
        SceneManager.LoadScene("Menu");
        //TODO: Pause game and show summary
    }
}
