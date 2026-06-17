using System;
using System.Collections;
using System.Collections.Generic;
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
                        Quaternion.Euler(90f, 0f, 0f));

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
        if (turn < 6)
        {
            float t = (turn - 1) / 4f;

            rare = Mathf.Lerp(70, 64, t);
            epic = Mathf.Lerp(30, 35, t);
            legendary = Mathf.Lerp(0, 1, t);
        }
        /*// 테스트용 레어 100%
        if (turn <= 5)
        {
            rare = 100;
            epic = 0;
            legendary = 0;
            return;
        }*/
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
        /*List<CardData> pool = GetCardPool(grade);

        // 위에서 찾은 pool에서 이미 뽑힌 카드는 제거
        pool.RemoveAll(card =>
            selectedCards.Contains(card));

        if (pool.Count == 0)
        {
            Debug.LogError($"{grade} 카드 풀이 부족합니다.");
            return null;
        }

        // 중복이 제외되었으니 남은 카드 중에서 랜덤 선택
        int rand = UnityEngine.Random.Range(0, pool.Count);

        return pool[rand];*/

        List<CardData> pool =
        new List<CardData>(GetCardPool(grade));

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
}
