using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상태이상 enum
/// </summary>
public enum StatusEffectType
{
    Normal = 0,
    Burn,
    Wet,
    Mud,

    IgnoreDefense,
    ChainLightning,
    Pierce,
}

/// <summary>
/// 상태이상 데이터 클래스
/// </summary>
[System.Serializable]
public class StatusEffectData
{
    public StatusEffectType effectType;

    public float duration;

    public float value;

    public float baseDamage;

    public Vector3 directionValue;        // 바람 속성 공에서
                                          // 어느 방향으로 움직이는 중인지 파악할 때 사용
}
