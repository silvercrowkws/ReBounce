using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementalChartUI : MonoBehaviour
{
    [SerializeField]
    Image elementalChart;

    Button aaaButton;

    private void Awake()
    {
        aaaButton = GetComponent<Button>();
        aaaButton.onClick.AddListener(AAA);
    }

    private void AAA()
    {
        // 버튼으로 토글
        elementalChart.gameObject.SetActive(
           !elementalChart.gameObject.activeSelf
       );
    }
}
