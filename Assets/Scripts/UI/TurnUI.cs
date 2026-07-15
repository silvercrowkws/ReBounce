using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnUI : MonoBehaviour
{
    TextMeshProUGUI turnText;

    TurnManager turnManager;

    private void Awake()
    {
        turnText = GetComponent<TextMeshProUGUI>();
        turnText.text = "1 턴";
    }

    private void Start()
    {
        turnManager = TurnManager.Instance;
        turnManager.onTurnStart += OnTurnUIChange;
    }

    private void OnDisable()
    {
        turnManager.onTurnStart -= OnTurnUIChange;
    }

    private void OnTurnUIChange(int turnNumber)
    {
        turnText.text = $"{turnNumber} 턴";
    }
}
