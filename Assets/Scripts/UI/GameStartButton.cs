using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartButton : MonoBehaviour
{
    Button gameStartButton;

    GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;

        gameStartButton = GetComponent<Button>();
        gameStartButton.onClick.AddListener(OnGameStartButton);
    }

    private void OnGameStartButton()
    {
        /*// 현재 씬 번호에 따라
        switch(gameManager.sceneNumber)
        {
            // 로비 씬에서는 
            case 0:
                break;

            case 1:
                break;
        }*/
        gameManager.LoadScene(1);
    }
}
