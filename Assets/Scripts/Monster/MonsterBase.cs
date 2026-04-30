using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum MonsterElementals
{
    Normal = 0,     // 기본
    Fire,           // 불
    Water,          // 물
    Earth,          // 흙
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

    protected virtual void Awake()
    {
        //currentHP = maxHP;    => 활성화 시 처리

        if (transform.childCount > 1)
        {
            Transform child = transform.GetChild(1);
            hpText = child.GetChild(0).GetComponent<TextMeshProUGUI>();

            if (hpText != null)
            {
                hpText.text = currentHP.ToString();
            }
        }
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
    }

    public virtual void TakeDamage(float amount)
    {
        CurrentHP -= amount;
        Debug.Log($"{gameObject.name}이 {amount}의 데미지. 남은 HP: {CurrentHP}");
    }

    public virtual void OnDie()
    {
        Debug.Log($"{gameObject.name} 사망!");
        gameObject.SetActive(false);
    }
}
