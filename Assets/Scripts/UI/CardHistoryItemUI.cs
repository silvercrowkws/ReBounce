using TMPro;
using UnityEngine;

public class CardHistoryItemUI : MonoBehaviour
{
    /// <summary>
    /// 카드 이름 + 원본 설명 (ex. "추가 탄환 : 노말 공 +1개 획득")
    /// </summary>
    [SerializeField] private TextMeshProUGUI descriptionText;

    /// <summary>
    /// 중첩 횟수 표기 (ex. "추가 탄환 x2", count가 1이어도 "추가 탄환 x1"로 표시)
    /// </summary>
    [SerializeField] private TextMeshProUGUI countText;

    /// <summary>
    /// 중첩 반영된 효과 (ex. "노말 공 +2개", count가 1이면 원본 효과 그대로)
    /// </summary>
    [SerializeField] private TextMeshProUGUI effectText;

    private void Awake()
    {
        descriptionText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        
        Transform child = transform.GetChild(1);
        countText = child.GetChild(0).GetComponent <TextMeshProUGUI>();
        effectText = child.GetChild(1).GetComponent <TextMeshProUGUI>();
    }

    public void Initialize(CardHistoryEntry entry)
    {
        descriptionText.text =
            $"{entry.data.cardName} : {entry.data.description}";

        countText.text =
            $"{entry.data.cardName} x{entry.count}";

        effectText.text =
            entry.data.GetAggregatedDescription(entry.count);
    }
}