using TMPro;
using UnityEngine;

/********************************************************************************************
 * 각 카드 선택지에는 등급이 있는데
 * 희귀(파랑), 영웅(보라), 전설(노랑, 주황)
 * 각각 뜰 확률이 희귀 70%, 영웅 25%, 전설 5% 가 기본인데
 * N 번째 턴이 진행됨에 따라 증가해서
 * 희귀 50%, 영웅 40%, 전설 10% 이런 식으로.
 * 
 * 우선 속성 선택지들은 그 공을 보유하고 있으면 뜨는 걸로 하고
 * 같은 속성에서는 1개만 뜨도록 해야겠네
 * 그리고 지금은 공 대미지가 10이지만 점점 올라갈텐데, % 계산할때 반올림 할지도 정해야?
 * 
 * 나중에 카드 UI에
 * 점화 Lv.2 + 화력 집중 Lv.1
 * 화상 상태 적이 받는 피해 +80%
 * 이런 식으로 누적 수치를 보여주면 좋을듯
********************************************************************************************/


/********************************************************************************************
 * 희귀 카드 풀은
 
 - 추가 탄환 : 노말 공 +1개 획득
 - 안정된 발사: 최대 발사 각도 +5°		            기본 70에 +5
 - 탄환 개조 : 모든 공 공격력 +2
 - 원소 주입 : 불 공 +1개 획득
 - 원소 주입 : 물 공 +1개 획득
 - 원소 주입 : 땅 공 +1개 획득
 - 원소 주입 : 전기 공 +1개 획득
 - 원소 주입 : 바람 공 +1개 획득
 
 * 불
 - 강한 화상 : 화상 피해 +2			                기대값 35
 - 잔불 : 화상 지속 시간 +1초
 - 점화 : 화상 상태의 대상에게 가하는 피해 +20%     화상은 지속 시간이 길지 않음
 
 * 물
 - 수분 축적 : 물 공 공격력 +5
 - 냉각 : 젖음 상태의 적이 받는 피해 +10%	        젖음은 지속 시간이 긺
 - 정화수 : 물 공 적중 시 젖음 지속시간 +5초
 
 * 땅
 - 파쇄 : 땅 공 공격력 +5
 - 압괴 : 체력이 50% 미만인 적에게 땅 공 피해 + 50%
 - 균열 : 땅 공의 추가 피해 배율 +20%
 
 * 번개
 - 증폭 회로 : 전기 공 공격력 +5
 - 과전류 : 전이 피해 +10%
 - 전압 집중 : 전기 공의 직접 피해 +20%             본체에 가하는 피해만 증가
 
 * 바람
 - 강풍 : 바람 공 공격력 +5
 - 날카로운 바람 : 관통 피해 +20%
 - 난기류 : 바람 공이 반사될 때마다 피해 +10%         얼마나 튕기는 지 보고 수치 조정 필요
********************************************************************************************/


/********************************************************************************************
 * 영웅 카드 풀은
 
 - 다중 장전 : 노말 공 +3
 - 대구경 탄환 : 노말 공 공격력 +15
 - 강화 탄환 : 모든 공 공격력 +5
 - 명사수 : 최대 발사 각도 +10                           만약 희귀에서 10°올렸으면 75 -> 85 가 되도록
 - 화염 탄환 : 불 공 +2
 - 수류 탄환 : 물 공 +2
 - 암석 탄환 : 땅 공 +2
 - 전류 탄환 : 전기 공 +2
 - 질풍 탄환 : 바람 공 +2

 * 불
 - 고열 : 화상 피해 +5
 - 타오르는 불꽃 : 화상 지속 시간 +2초
 - 화력 집중 : 화상 상태의 적이 받는 피해 + 40%

 * 물
 - 급류 : 물 공 공격력 +10
 - 빙결 : 젖음 상태 적이 받는 피해 +20%
 - 범람 : 젖음 전파 대상 +1(최대 N중첩)                  보드 내의 젖음 상태가 아닌 적에게 전파..?

 * 땅
 - 거암 : 땅 공 공격력 +10
 - 붕괴 : 체력 50% 미만 적에게 땅 공 피해 +100%
 - 분쇄 : 땅 공의 추가 피해 배율 +50%

 * 전기
 - 초전도 : 전기 공 공격력 +10
 - 낙뢰 : 전기 공의 직접 피해 +50%
 - 확장 회로 : 전이 범위 +1(중접X)

 * 바람
 - 폭풍 : 바람 공 공격력 +10
 - 칼바람 : 관통 피해 +50%
 - 상승 기류 : 관통 범위 +1(중첩X)
 ********************************************************************************************/


/********************************************************************************************
 * 전설 카드 풀은
 - 철갑탄 : 노말 공 공격력 +25
 - 분열 탄환 : 공이 적에게 적중 시 20%의 확률로 분열 or 공이 척 적중시 1회 분열

 * 불
 - 소각 : 화상 피해 100% 증가
 - 잿더미 : 화상 피해에 대상 최대 체력의 1%를 추가한다.

 * 물
 - 해일 : 범람이 모든 젖지 않은 적에게 적용된다.
 - 와류 : 젖음 상태 적에게 가하는 피해가 방어 효과를 10% 무시한다(수치 조정 필요)       실드, 방어력, 피해 감소 등이 생기면 할 만 하겠네

 * 땅
 - 지진 : 좌우 적에게 피해의 50%(최대 2중첩)
 - 압쇄 : 땅 공의 추가 피해가 적의 현재 체력의 5%를 추가로 가한다.

 * 전기
 - 뇌폭 : 전이 피해가 직접 피해와 동일해짐
 - 초고압 : 전기 공 적중 시 추가 전이 +1(중첩X)

 * 바람
 - 태풍 : 관통 거리 무제한
 - 제트기류 : 바람 공이 반사될 때마다 피해 +30%                 얼마나 튕기는 지 보고 수치 조정 필요
********************************************************************************************/

/********************************************************************************************
 * 카드 확률(사이를 보간 적용)
 
 * 1~5턴
 - 64 /35 /1

 * 6~10턴
 - 55 / 40 / 5

 * 11~15턴
 - 50 / 40 / 10

 * 16~20턴
 - 40 / 50 / 10

 * 21~25턴
 - 30 / 55 / 15

 * 26턴 이후
 - 30 / 55 / 15 고정
********************************************************************************************/
public enum CardGrade
{
    Rare,
    Epic,
    Legendary
}

public class Card : MonoBehaviour
{
    /// <summary>
    /// 영웅, 전설 카드 반짝이는 효과용 스프라이트
    /// </summary>
    [SerializeField] Transform glowSprite;

    /// <summary>
    /// 이 카드의 등급
    /// </summary>
    [SerializeField] private CardGrade cardGrade;

    private CardData cardData;

    private Vector3 originScale;
    private Color originColor;
    private SpriteRenderer glowframeSpriteRenderer;

    private SpriteRenderer frameSpriteRenderer;

    Color rareColor = new Color(0.2f, 0.55f, 1f);
    Color epicColor = new Color(0.65f, 0.3f, 1f);
    Color legendaryColor = new Color(1f, 0.75f, 0.15f);

    Color legendaryGlowColor = new Color(1f, 0.6f, 0f, 0.4f);
    Color epicGlowColor = new Color(0.45f, 0.15f, 0.85f, 0.4f);

    /// <summary>
    /// 타이틀 텍스트
    /// </summary>
    TextMeshPro titleText;

    /// <summary>
    /// 카드 효과 설명 텍스트
    /// </summary>
    TextMeshPro descriptionText;

    /// <summary>
    /// 카드의 스프라이트
    /// </summary>
    SpriteRenderer windowSprite;

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        frameSpriteRenderer = child.GetComponent<SpriteRenderer>();

        child = transform.GetChild(1);
        glowframeSpriteRenderer = child.GetComponent<SpriteRenderer>();

        child = transform.GetChild(3);
        titleText = child.GetComponent<TextMeshPro>();

        child = transform.GetChild(4);
        descriptionText = child.GetComponent<TextMeshPro>();

        child = transform.GetChild(2);
        windowSprite = child.GetChild(0).GetComponent<SpriteRenderer>();

        /*switch (cardGrade)
        {
            case CardGrade.Rare:
                frameSpriteRenderer.color = rareColor;
                titleText.text = "희귀";
                break;

            case CardGrade.Epic:
                frameSpriteRenderer.color = epicColor;
                glowframeSpriteRenderer.color = epicGlowColor;
                titleText.text = "영웅";
                break;

            case CardGrade.Legendary:
                frameSpriteRenderer.color = legendaryColor;
                glowframeSpriteRenderer.color = legendaryGlowColor;
                titleText.text = "전설";
                break;
        }*/
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void Start()
    {
        // 희귀 등급이 아니면
        if(cardGrade != CardGrade.Rare)
        {
            originScale = glowSprite.localScale;

            glowframeSpriteRenderer = glowSprite.GetComponent<SpriteRenderer>();
            originColor = glowframeSpriteRenderer.color;
        }

        // 영웅 등급은 기본적으로 1.05배 크게
        if (cardGrade == CardGrade.Epic)
        {
            glowSprite.localScale = originScale * 1.05f;
        }
    }

    private void Update()
    {
        float t = (Mathf.Sin(Time.time * 2f) + 1f) * 0.5f;

        switch (cardGrade)
        {
            // 희귀는 아무 효과 없음
            case CardGrade.Rare:
                break;

            // 영웅
            case CardGrade.Epic:
                {
                    Color c = originColor;
                    c.a = Mathf.Lerp(0f, 0.6f, t);
                    glowframeSpriteRenderer.color = c;

                    break;
                }

            // 전설
            case CardGrade.Legendary:
                {
                    glowSprite.localScale =
                        originScale * Mathf.Lerp(1f, 1.1f, t);

                    Color c = originColor;
                    c.a = Mathf.Lerp(0.2f, 0.6f, t);
                    glowframeSpriteRenderer.color = c;
                    break;
                }
        }
    }

    public void Initialize(CardData data)
    {
        cardData = data;
        cardGrade = data.grade;

        titleText.text = data.cardName;
        descriptionText.text = data.description;
        windowSprite.sprite = data.icon;

        ApplyGradeVisual();
    }

    private void ApplyGradeVisual()
    {
        switch (cardGrade)
        {
            case CardGrade.Rare:
                frameSpriteRenderer.color = rareColor;
                break;

            case CardGrade.Epic:
                frameSpriteRenderer.color = epicColor;
                glowframeSpriteRenderer.color = epicGlowColor;
                break;

            case CardGrade.Legendary:
                frameSpriteRenderer.color = legendaryColor;
                glowframeSpriteRenderer.color = legendaryGlowColor;
                break;
        }
    }
}