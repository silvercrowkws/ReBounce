using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartButton : MonoBehaviour
{
    Button gameStartButton;

    GameManager gameManager;

    CanvasGroup canvasGroup;

    TextMeshProUGUI startText;

    private void Awake()
    {
        gameManager = GameManager.Instance;

        gameStartButton = GetComponent<Button>();
        gameStartButton.onClick.AddListener(OnGameStartButton);

        canvasGroup = GetComponent<CanvasGroup>();

        startText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        //CanvasGroupControl();
    }

    private void Start()
    {
        CanvasGroupControl();
    }

    /// <summary>
    /// 현재 씬에 따라 캔버스 그룹과 텍스트를 조절하는 함수
    /// </summary>
    private void CanvasGroupControl()
    {
        Debug.LogWarning("CanvasGroupControl 실행");
        Debug.LogWarning($"현재 게임 씬 넘버 : {SceneManager.GetActiveScene().buildIndex}");

        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                startText.text = $"게임 시작";
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                break;

            case 1:
                startText.text = $"게임 재시작";
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                break;
        }
    }

    public void GameRestart()
    {
        startText.text = $"게임 재시작";
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void OnGameStartButton()
    {
        /*// 현재 씬 번호에 따라
        switch(SceneManager.GetActiveScene().buildIndex)
        {
            // 로비 씬에서는 
            case 0:
                break;

            case 1:
                break;
        }*/

        // 생각해보니 재시작일거면 그냥 다시 1번씬 불러오면 되는거 아닌가?
        gameManager.LoadScene(1);
    }
}
