using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    MonsterElementals MonsterElement { get; }   // 몬스터의 속성 추가

    // 현재 HP와 최대 HP를 조회할 수 있는 프로퍼티
    float CurrentHP { get; set; }
    float MaxHP { get; }

    // 데미지를 받는 메서드
    void TakeDamage(float amount);

    /// <summary>
    /// 배리어 감소율을 제어할 수 있는 데미지 처리
    /// </summary>
    /// <param name="amount">원본 데미지</param>
    /// <param name="ignoreBarrier">true면 배리어를 완전히 무시 (땅 속성 공)</param>
    /// <param name="barrierIgnorePercent">배리어 감소율 중 추가로 무시할 비율 (와류)</param>
    void TakeDamage(float amount, bool ignoreBarrier, float barrierIgnorePercent = 0f, MonsterElementals ballElement = MonsterElementals.Normal);

    void TakeStatusEffect(StatusEffectData effect);

    // 사망 시 실행될 로직
    void OnDie();
}
