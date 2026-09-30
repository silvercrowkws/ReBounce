using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialButton : MonoBehaviour
{
    Button button;

    /// <summary>
    /// 듀토리얼 패널들 모아놓은 게임 오브젝트
    /// </summary>
    [SerializeField]
    GameObject tutorialPanels;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Tutorial);
    }

    /// <summary>
    /// 듀토리얼 버튼 클릭으로 실행되는 함수
    /// </summary>
    private void Tutorial()
    {
        Debug.Log("듀토리얼 실행");
    }
}
