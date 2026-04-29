using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
