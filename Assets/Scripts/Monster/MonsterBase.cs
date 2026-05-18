using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MonsterElementals
{
    Normal = 0,     // 기본
    Fire,           // 불
    Water,          // 물
    Land,           // 흙
    Electric,       // 전기
    Wind,           // 바람

    // 공과 몬스터의 상성 구조가
    // 불 < 물 < 흙 < 번개 < 바람 < 불
    // 불은   바람에 강하고    물에 약함
    // 물은   불에 강하고      흙에 약함
    // 흙은   물에 강하고      번개에 약함
    // 바람은 번개에 강하고    불에 약함
}

public class MonsterBase : RecycleObject, IDamageable
{
    /// <summary>
    /// 이 몬스터의 속성(인스펙터에서 설정 가능)
    /// </summary>
    [SerializeField] 
    private MonsterElementals monsterElementals = MonsterElementals.Normal;

    /// <summary>
    /// 외부에서 접근할 때는 프로퍼티로만 사용
    /// </summary>
    public MonsterElementals MonsterElement => monsterElementals;


    [SerializeField] protected float maxHP = 100f;
    protected float currentHP;

    /// <summary>
    /// HP 텍스트
    /// </summary>
    protected TextMeshProUGUI hpText;    

    public float CurrentHP
    {
        get => currentHP;
        set
        {
            if (currentHP != value)
            {
                currentHP = Mathf.Clamp(value, 0, maxHP);

                if (hpText != null)
                {
                    hpText.text = currentHP.ToString();
                }

                if (currentHP <= 0)
                {
                    OnDie();
                }
            }
        }
    }

    public float MaxHP
    {
        get => maxHP;
    }

    /// <summary>
    /// 상태 이상 표시용 이미지
    /// </summary>
    Image statusEffectImage;

    /// <summary>
    /// 화상 상태이상 스프라이트
    /// </summary>
    private Sprite burnSprite;

    /// <summary>
    /// 젖음 상태이상 스프라이트
    /// </summary>
    private Sprite wetSprite;

    protected virtual void Awake()
    {
        //currentHP = maxHP;    => 활성화 시 처리

        if (transform.childCount > 1)
        {
            Transform child = transform.GetChild(1);        // 캔버스 위치
            hpText = child.GetChild(1).GetComponent<TextMeshProUGUI>();

            if (hpText != null)
            {
                hpText.text = currentHP.ToString();
            }

            statusEffectImage = child.GetChild(0).GetComponent<Image>();

            if(statusEffectImage != null)
            {
                statusEffectImage.sprite = null;        // 이미지 비우고
                Color color = statusEffectImage.color;
                color.a = 0f;
                statusEffectImage.color = color;        // 투명 처리
            }
        }

        burnSprite = Resources.Load<Sprite>("StatusEffect/BurnState");
        wetSprite = Resources.Load<Sprite>("StatusEffect/WetState");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Init();     // 초기화 처리
    }

    protected virtual void Init()
    {
        currentHP = maxHP;

        if (hpText != null)
            hpText.text = currentHP.ToString();

        /*if (statusEffectImage != null)
        {
            statusEffectImage.sprite = null;        // 이미지 비우고
            Color color = statusEffectImage.color;
            color.a = 0f;
            statusEffectImage.color = color;        // 투명 처리
        }*/
        StateEffectColorControl(false);
    }

    /// <summary>
    /// 상태이상 이미지 컨트롤 함수
    /// </summary>
    /// <param name="tf"></param>
    private void StateEffectColorControl(bool tf)
    {
        // 비활성화 처리
        if (!tf)
        {
            if (statusEffectImage != null)
            {
                statusEffectImage.sprite = null;        // 이미지 비우고
                Color color = statusEffectImage.color;
                color.a = 0f;
                statusEffectImage.color = color;        // 투명 처리
            }
        }
        else
        {
            Color color = statusEffectImage.color;
            color.a = 1f;
            statusEffectImage.color = color;
        }
    }

    public virtual void TakeDamage(float amount)
    {
        CurrentHP -= amount;
        Debug.Log($"{gameObject.name}이 {amount}의 데미지. 남은 HP: {CurrentHP}");
    }

    public virtual void TakeStatusEffect(StatusEffectData effect)
    {
        switch (effect.effectType)
        {
            // 화상 상태
            case StatusEffectType.Burn:
                //StartCoroutine(BurnCoroutine(effect));

                // 젖음 상태라면 화상은 걸리지 않고 즉시 데미지만 적용
                if (isWet)
                {
                    float instantDamage = effect.value * effect.duration;

                    TakeDamage(instantDamage);

                    Debug.Log($"젖음 상태에서 화상 반응! 즉시 데미지 : {instantDamage}");

                    return;
                }

                Coroutine burn = StartCoroutine(BurnCoroutine(effect));
                burnCoroutines.Add(burn);
                break;

            // 젖음 상태
            case StatusEffectType.Wet:
                //StartCoroutine(WetCoroutine(effect));

                if (burnStackCount > 0)
                {
                    TakeDamage(remainBurnDamage);

                    Debug.Log($"젖음 반응! 남은 화상 데미지 폭발 : {remainBurnDamage}");

                    // 모든 화상 제거
                    foreach (Coroutine coroutine in burnCoroutines)
                    {
                        if (coroutine != null)
                        {
                            StopCoroutine(coroutine);
                        }
                    }

                    burnCoroutines.Clear();

                    burnStackCount = 0;
                    remainBurnDamage = 0f;
                }

                // 이미 젖음 상태라면 지속시간 갱신
                if (wetCoroutine != null)
                {
                    StopCoroutine(wetCoroutine);
                }

                wetCoroutine = StartCoroutine(WetCoroutine(effect));
                break;

            // 진흙 상태
            case StatusEffectType.Mud:
                break;

            /*// 감전 상태
            case StatusEffectType.Shock:
                break;

            // 
            case StatusEffectType.Pierce:
                break;*/
        }
    }

    /// <summary>
    /// 화상 중첩 개수
    /// </summary>
    private int burnStackCount = 0;

    /// <summary>
    /// 현재 활성화된 화상 코루틴들
    /// </summary>
    private List<Coroutine> burnCoroutines = new List<Coroutine>();

    /// <summary>
    /// 남아있는 화상 총 누적 데미지
    /// </summary>
    private float remainBurnDamage = 0f;

    /// <summary>
    /// 화상 코루틴
    /// </summary>
    /// <param name="effect"></param>
    /// <returns></returns>
    IEnumerator BurnCoroutine(StatusEffectData effect)
    {
        burnStackCount++;       // 화상 중첩 개수 증가

        float elapsedTime = 0f;
        float tickInterval = 1f;   // 1초마다 데미지

        // 화상 총 누적 데미지 등록
        remainBurnDamage += effect.value * effect.duration;

        // 첫 화상이면 이미지 활성화
        if (burnStackCount == 1)
        {
            StateEffectColorControl(true);
            statusEffectImage.sprite = burnSprite;      // 상태 이상 이미지 변경
        }

        while (elapsedTime < effect.duration)
        {
            TakeDamage(effect.value);

            // 실제 들어간 데미지만큼 감소
            remainBurnDamage -= effect.value;

            yield return new WaitForSeconds(tickInterval);

            elapsedTime += tickInterval;
        }

        burnStackCount--;       // while 문 종료시 화상 중첩 개수 감소

        // 현재 끝난 코루틴 정리
        burnCoroutines.RemoveAll(c => c == null);

        /*// 화상 중첩이 하나도 안남았을 때만 끄기
        if (burnStackCount <= 0)
        {
            burnStackCount = 0;
            StateEffectColorControl(false);
        }*/

        // 모든 화상 종료 시
        if (burnStackCount <= 0)
        {
            burnStackCount = 0;
            remainBurnDamage = 0f;

            // 젖음 상태가 아니라면만 끄기
            if (!isWet)
            {
                StateEffectColorControl(false);
            }
        }
    }

    /// <summary>
    /// 현재 젖음 상태 코루틴
    /// </summary>
    private Coroutine wetCoroutine;

    /// <summary>
    /// 현재 젖음 상태 여부
    /// </summary>
    private bool isWet = false;

    /// <summary>
    /// 젖음 코루틴
    /// </summary>
    /// <param name="effect"></param>
    /// <returns></returns>
    IEnumerator WetCoroutine(StatusEffectData effect)
    {
        isWet = true;

        // 상태 이상 이미지 표시
        StateEffectColorControl(true);
        statusEffectImage.sprite = wetSprite;

        float elapsedTime = 0f;

        while (elapsedTime < effect.duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isWet = false;
        wetCoroutine = null;

        // 젖음 종료
        StateEffectColorControl(false);
    }

    public virtual void OnDie()
    {
        Debug.Log($"{gameObject.name} 사망!");
        gameObject.SetActive(false);
    }
}
