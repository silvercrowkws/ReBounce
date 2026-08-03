using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UIElements;
using UnityEngine.UI;
using static UnityEditor.Progress;
using static UnityEngine.EventSystems.EventTrigger;

public class SelectedCardHistoryUI : MonoBehaviour
{
    Button historyButton;

    [SerializeField] ScrollRect scrollView;

    [SerializeField] private CanvasGroup scrollViewCanvasGroup;

    private bool isHistoryVisible;

    /// <summary>
    /// 이력 아이템들이 실제로 생성될 부모(Content, GridLayoutGroup이 붙어있는 오브젝트)
    /// </summary>
    [SerializeField] private Transform contentParent;

    /// <summary>
    /// 이력 아이템 프리팹(CardHistoryItemUI가 붙어있어야 함)
    /// </summary>
    [SerializeField] private CardHistoryItemUI itemPrefab;

    private void Awake()
    {
        historyButton = GetComponent<Button>();
        historyButton.onClick.AddListener(ToggleHistory);
    }

    private void Start()
    {
        scrollViewCanvasGroup = scrollView.GetComponent<CanvasGroup>();

        SetHistoryVisible(false);
    }

    private void ToggleHistory()
    {
        SetHistoryVisible(!isHistoryVisible);
    }

    private void SetHistoryVisible(bool visible)
    {
        isHistoryVisible = visible;

        scrollViewCanvasGroup.alpha = visible ? 1f : 0f;
        scrollViewCanvasGroup.interactable = visible;
        scrollViewCanvasGroup.blocksRaycasts = visible;

        CardManager.Instance.IsHistoryUIOpen = visible;

        // 열릴 때마다 최신 이력으로 다시 채움
        if (visible)
        RefreshHistoryList();
    }

    private void RefreshHistoryList()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    
        foreach (CardHistoryEntry entry in CardManager.Instance.GetGroupedHistory())
        {
            CardHistoryItemUI item = Instantiate(itemPrefab, contentParent);
            item.Initialize(entry);
        }
    }
}
