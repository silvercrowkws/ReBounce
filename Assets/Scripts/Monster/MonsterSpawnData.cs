using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터 스폰 시 필요한 모든 정보
/// </summary>
public class MonsterSpawnData
{
    /// <summary>
    /// 실제 스폰될 몬스터
    /// </summary>
    public MonsterBase monster;

    /// <summary>
    /// 몬스터 타입 일반 / 기믹 / 보스
    /// </summary>
    public SpawnMonsterType spawnType;

    /// <summary>
    /// 몬스터 속성
    /// </summary>
    public MonsterElementals element;

    /// <summary>
    /// 몬스터 기믹
    /// </summary>
    public MonsterGimmicks gimmick;

    /// <summary>
    /// 지정되면 이 값을 그대로 maxHP로 사용 (턴 기반 자동 계산/배수 적용을 건너뜀)
    /// </summary>
    public float? overrideMaxHP;
}
