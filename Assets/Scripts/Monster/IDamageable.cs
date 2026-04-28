using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    // 현재 HP와 최대 HP를 조회할 수 있는 프로퍼티
    float CurrentHP { get; set; }
    float MaxHP { get; }

    // 데미지를 받는 메서드
    void TakeDamage(float amount);

    // 사망 시 실행될 로직
    void OnDie();
}
