using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementalChartUI : MonoBehaviour
{
    public Sprite elementalChartSprite;
    public Sprite gmmickSprite;
    public Sprite bossGmmickSprite;

    Button aaaButton;

    private void Awake()
    {
        aaaButton = GetComponent<Button>();
        aaaButton.onClick.AddListener(AAA);
    }

    private void AAA()
    {
        
    }
}
