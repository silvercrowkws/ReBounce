using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Monster_Goblin_Green : RecycleObject, IDamageable
{
    [SerializeField] private float maxHP = 100f;
    private float currentHP;
    
    public float CurrentHP
    {
        get => currentHP;
        set
        {
            if (currentHP != value)
            {
                currentHP = Mathf.Clamp(value, 0, maxHP);
                //Debug.Log($"현재 체력 변경 {currentHP}");
                hpText.text = currentHP.ToString();

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
    /// HP 텍스트
    /// </summary>
    TextMeshProUGUI hpText;

    private void Awake()
    {
        currentHP = maxHP;

        if(transform.childCount > 1)
        {
            Transform child = transform.GetChild(1);
            hpText = child.GetChild(0).GetComponent<TextMeshProUGUI>();

            hpText.text = currentHP.ToString();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //StartCoroutine(LifeOver(lifeTime));
    }

    // 인터페이스 메서드 구현
    public void TakeDamage(float amount)
    {
        CurrentHP -= amount;
        Debug.Log($"{gameObject.name}이 {amount}의 데미지를 입음. 남은 HP: {CurrentHP}");

        /*if (currentHP <= 0)
        {
            OnDie();
        }*/
    }

    public void OnDie()
    {
        Debug.Log($"{gameObject.name} 사망!");
        gameObject.SetActive(false); // 또는 오브젝트 풀로 반환
    }
}
