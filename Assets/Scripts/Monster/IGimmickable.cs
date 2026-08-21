using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGimmickable
{
    MonsterGimmicks MonsterGimmick { get; }     // 몬스터의 기믹

    /// <summary>
    /// 턴 시작 시(공 발사 전) 발동하는 기믹 (예: Summon)
    /// 해당 없는 기믹이면 아무 동작 없이 반환
    /// </summary>
    void OnTurnStartGimmick();

    /// <summary>
    /// 턴 종료 시 발동하는 기믹 (예: Heal)
    /// 해당 없는 기믹이면 아무 동작 없이 반환
    /// </summary>
    void OnTurnEndGimmick();

    /// <summary>
    /// 사망 시 발동하는 기믹 (예 : Magnetic)
    /// </summary>
    void OnDieGimmick();

    /// <summary>
    /// 피격 직전 발동하는 기믹 (예: Barrier)
    /// 데미지 값을 가공해서 반환 (해당 없으면 입력값 그대로 반환)
    /// </summary>
    float OnBeforeTakeDamageGimmick(float incomingDamage);

    /// <summary>
    /// 공격 방향 판정 시 발동하는 기믹 (예: Shield)
    /// true면 해당 공격을 완전히 무효화
    /// </summary>
    bool OnCheckBlockGimmick(Vector3 attackDirection);

    // Magnetic은 몬스터 자신이 아니라 "공" 쪽에서 감지해야 하는 구조라
    // 여기 인터페이스만으로는 부족할 수 있음 → 공 이동/충돌 코드 보고 별도 설계 필요
}
