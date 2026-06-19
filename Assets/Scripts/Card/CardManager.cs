using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardManager : Singleton<CardManager>
{
    TurnManager turnManager;

    /// <summary>
    /// 카드 프리팹
    /// </summary>
    [SerializeField] private GameObject cardPrefab;

    /// <summary>
    /// 생성 위치(0 = 왼쪽, 1 = 가운데, 2 = 오른쪽)
    /// </summary>
    private readonly Vector3[] cardPositions =
    {
        new Vector3(-0.65f, 1f, 0f),
        new Vector3( 0.00f, 1f, 0f),
        new Vector3( 0.65f, 1f, 0f)
    };

    /// <summary>
    /// 카드 풀(만든 SO 전부 넣어야 함 => 나중에는 어드레서블로 처리할 예정)
    /// </summary>
    [SerializeField] private List<CardData> allCards;

    public List<CardData> rareCardPool = new();
    public List<CardData> epicCardPool = new();
    public List<CardData> legendaryCardPool = new();
    
    /// <summary>
    /// 이번 턴에 선택된 카드
    /// </summary>
    private Card thisTurnSelectedCard;

    /// <summary>
    /// 지금까지 선택한 카드 이력
    /// </summary>
    private List<CardData> selectedCardHistory = new();

    public IReadOnlyList<CardData> SelectedCardHistory
        => selectedCardHistory;

    /// <summary>
    /// 이번 턴에 카드가 선택되었는지?(false : 카드 선택 안함, true : 카드 선택함)
    /// </summary>
    private bool isCardSelected;
    public bool IsCardSelected => isCardSelected;

    private void Awake()
    {
        foreach (CardData card in allCards)
        {
            switch (card.grade)
            {
                case CardGrade.Rare:
                    rareCardPool.Add(card);
                    break;

                case CardGrade.Epic:
                    epicCardPool.Add(card);
                    break;

                case CardGrade.Legendary:
                    legendaryCardPool.Add(card);
                    break;
            }
        }
    }
    private void Start()
    {
        turnManager = TurnManager.Instance;
        turnManager.onTurnStart += OnGenerateCardChoices;
    }

    private void OnDestroy()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.onTurnStart -= OnGenerateCardChoices;
    }

    /// <summary>
    /// 턴 시작 시 카드 선택지 생성
    /// </summary>
    /*private void OnGenerateCardChoices(int turn)
    {
        CardData card1 = GetRandomCardGrade(turn);
        CardData card2 = GetRandomCardGrade(turn);
        CardData card3 = GetRandomCardGrade(turn);

        Debug.Log($"카드1 : {card1}");
        Debug.Log($"카드2 : {card2}");
        Debug.Log($"카드3 : {card3}");


        // 카드 등급에 맞는 카드를 카드풀에서 랜덤 선택
        SpawnCard(card1, leftCardPos);
        SpawnCard(card2, centerCardPos);
        SpawnCard(card3, rightCardPos);
    }*/

    private void OnGenerateCardChoices(int turn)
    {
        // 카드 생성 전 아직 카드가 선택되지 않았다고 변경
        isCardSelected = false;

        List<CardData> selectedCards = new();

        for (int i = 0; i < 3; i++)
        {
            CardGrade grade =
                GetRandomCardGrade(turn);

            CardData card =
                GetRandomCard(
                    grade,
                    selectedCards);

            selectedCards.Add(card);
        }

        for (int i = 0; i < selectedCards.Count; i++)
        {
            SpawnCard(
                selectedCards[i],
                cardPositions[i]);
        }
    }

    private void SpawnCard(
    CardData data,
    Vector3 position)
    {
        GameObject obj =
            Instantiate(cardPrefab,
                        position,
                        Quaternion.Euler(90f, 0f, 0f),
                        transform);

        Card card = obj.GetComponent<Card>();

        card.Initialize(data);
    }

    private CardGrade GetRandomCardGrade(int turn)
    {
        GetCardGradeProbability(
            turn,
            out float rare,
            out float epic,
            out float legendary);

        float rand = UnityEngine.Random.Range(0f, 100f);

        if (rand < rare)
            return CardGrade.Rare;

        if (rand < rare + epic)
            return CardGrade.Epic;

        return CardGrade.Legendary;
    }

    private void GetCardGradeProbability(
        int turn,
        out float rare,
        out float epic,
        out float legendary)
    {
        /*if (turn < 6)
        {
            float t = (turn - 1) / 4f;

            rare = Mathf.Lerp(70, 64, t);
            epic = Mathf.Lerp(30, 35, t);
            legendary = Mathf.Lerp(0, 1, t);
        }*/
        // 테스트용 확률 조작
        // - 테스트에서 전설 카드 풀이 부족하다고 하는 이유는
        // - 지금 카드 뜨는 조건이 해당 카드의 속성을 한번이라도 해금했어야 하는데
        // - 아직 그 부분 추가 안했고, 전설 카드는 모두 속성 카드라 그럼
        if (turn <= 5)
        {
            rare = 100;
            epic = 0;
            legendary = 0;
            return;
        }
        else if (turn < 11)
        {
            float t = (turn - 5) / 5f;          // 보간을 적용해서 8턴인 경우
                                                // 0.6. 즉 a → b 사이를 60% 만큼 보간
            rare = Mathf.Lerp(64, 55, t);       // 64 - (9 * 0.6) = 58.6% 의 확률로 희귀
            epic = Mathf.Lerp(35, 40, t);       // 35 + (5 * 0.6) = 38% 의 확률로 영웅
            legendary = Mathf.Lerp(1, 5, t);    // 1 + (4 * 0.6) = 3.4% 의 확률로 전설
        }
        else if (turn < 16)
        {
            float t = (turn - 10) / 5f;

            rare = Mathf.Lerp(55, 50, t);
            epic = Mathf.Lerp(40, 40, t);
            legendary = Mathf.Lerp(5, 10, t);
        }
        else if (turn < 21)
        {
            float t = (turn - 15) / 5f;

            rare = Mathf.Lerp(50, 40, t);
            epic = Mathf.Lerp(40, 50, t);
            legendary = 10;
        }
        else if (turn < 26)
        {
            float t = (turn - 20) / 5f;

            rare = Mathf.Lerp(40, 30, t);
            epic = Mathf.Lerp(50, 55, t);
            legendary = Mathf.Lerp(10, 15, t);
        }
        else
        {
            rare = 30;
            epic = 55;
            legendary = 15;
        }
    }

    private List<CardData> GetCardPool(CardGrade grade)
    {
        /*List<CardData> pool = new();

        // 카드의 등급에 맞는 카드만 찾음
        foreach (CardData card in allCards)
        {
            if (card.grade == grade)
                pool.Add(card);     // 해당 등급에 맞는 카드만 pool에 더하고
        }

        return pool;*/

        // => allCards에서 각 등급별로 분류(Awake에서)하는 것에서 하도록 수정
        switch (grade)
        {
            case CardGrade.Rare:
                return rareCardPool;

            case CardGrade.Epic:
                return epicCardPool;

            case CardGrade.Legendary:
                return legendaryCardPool;
        }

        return null;
    }

    private CardData GetRandomCard(
    CardGrade grade,
    List<CardData> selectedCards)
    {
        /*List<CardData> pool =
        new List<CardData>(GetCardPool(grade));*/

        // 현재 보유 속성에 해당하는 카드만 필터링
        List<CardData> pool =
            new List<CardData>(
                GetCardPool(grade).FindAll(IsAvailableCard));

        // 이미 뽑힌 카드 중복 제거
        pool.RemoveAll(card =>
            selectedCards.Contains(card));

        if (pool.Count == 0)
        {
            Debug.LogError($"{grade} 카드 풀이 부족합니다.");
            return null;
        }

        // 중복이 제외되었으니 남은 카드 중에서 랜덤 선택
        return pool[UnityEngine.Random.Range(0, pool.Count)];
    }

    /// <summary>
    /// 현재 플레이어가 보유한 속성의 카드인지 검사하는 함수
    /// (보유하지 않은 속성 카드는 등장하지 않도록)
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    private bool IsAvailableCard(CardData card)
    {
        BallShooter shooter = FindObjectOfType<BallShooter>();

        if (shooter == null)
            return true;

        // 노말 카드는 항상 등장 가능
        if (card.elementals == CardElementals.Normal)
            return true;

        return shooter.unlockedElementals.Contains(
            (BallElementals)card.elementals);
    }

    public void SelectCard(Card card)
    {
        // 이번 턴에 선택한 카드
        thisTurnSelectedCard = card;

        Debug.Log($"선택 카드 : {card.CardData.cardName}");
        //Debug.Log($"효과 : {card.CardData.effectType}");

        // 선택 카드 이력 저장
        selectedCardHistory.Add(card.CardData);

        // 카드 선택 후 카드를 선택했다고 변경
        isCardSelected = true;

        // 카드 선택 연출 시작
        StartCoroutine(CardSelectEffect(card));
    }

    /// <summary>
    /// 카드 선택 시 재생되는 연출 코루틴
    /// - 선택한 카드는 Z축 위로 0.1만큼 이동
    /// - 모든 카드는 선택과 동시에 페이드 아웃
    /// - 연출 종료 후 카드 제거
    /// </summary>
    /// <param name="selectedCard">플레이어가 선택한 카드</param>
    /// <returns></returns>
    IEnumerator CardSelectEffect(Card selectedCard)
    {
        float duration = 0.25f;     // 연출 지속 시간
        float elapsed = 0f;         // 경과 시간

        // 선택된 카드의 시작 위치
        Vector3 selectedStartPos = selectedCard.transform.position;

        // 선택된 카드는 Z축으로 0.1 만큼 이동
        Vector3 selectedEndPos = selectedStartPos + Vector3.forward * 0.1f;

        // 페이드 아웃을 위해 모든 SpriteRenderer의 원본 색상 저장을 위한 딕셔너리
        Dictionary<SpriteRenderer, Color> spriteColors = new();

        // 페이드 아웃을 위해 모든 TextMeshPro의 원본 색상 저장을 위한 딕셔너리
        Dictionary<TextMeshPro, Color> textColors = new();

        // 생성 했었던 자식으로 존재하는 현재 화면에 존재하는 카드들 가져오기
        Card[] cards = GetComponentsInChildren<Card>();

        foreach (Card card in cards)
        {
            // 카드 내부의 모든 SpriteRenderer 저장
            foreach (SpriteRenderer sr in card.GetComponentsInChildren<SpriteRenderer>())
            {
                spriteColors.Add(sr, sr.color);
            }

            // 카드 내부의 모든 TextMeshPro 저장
            foreach (TextMeshPro text in card.GetComponentsInChildren<TextMeshPro>())
            {
                textColors.Add(text, text.color);
            }
        }

        // 0.5초 동안 연출 진행
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // 0 ~ 1 보간
            float t = Mathf.Clamp01(elapsed / duration);

            // 선택 카드 상승
            selectedCard.transform.position =
                Vector3.Lerp(
                    selectedStartPos,
                    selectedEndPos,
                    t);

            // 모든 SpriteRenderer 알파값 감소
            foreach (var pair in spriteColors)
            {
                Color c = pair.Value;
                c.a = Mathf.Lerp(pair.Value.a, 0f, t);
                pair.Key.color = c;
            }

            // 모든 TextMeshPro 알파값 감소
            foreach (var pair in textColors)
            {
                Color c = pair.Value;
                c.a = Mathf.Lerp(pair.Value.a, 0f, t);
                pair.Key.color = c;
            }

            yield return null;
        }

        // 연출 종료 후 모든 카드 제거
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
