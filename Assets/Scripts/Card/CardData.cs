using UnityEngine;

public enum CardEffectType
{
    None,

    AddNormalBall,              // 노말 공 +1
    IncreaseMaxShootAngle,      // 최대 발사 각도 +5°
    IncreaseAllBallDamage,      // 모든 공 공격력 +2
    AddFireBall,
    AddWaterBall,
    AddLandBall,
    AddElectricBall,
    AddWindBall,

    IncreaseBallDamage,

    IncreaseBurnDamage,
    IncreaseBurnDuration,
    IncreaseBurnTargetDamage,

    IncreaseWetDamage,
    IncreaseWetDuration,

    IncreaseLandBonusDamage,

    IncreaseChainDamage,
    IncreaseChainRange,

    IncreasePierceDamage,
    IncreasePierceCount,
}

[CreateAssetMenu(fileName = "Card_", menuName = "Card/Card Data")]
public class CardData : ScriptableObject
{
    [Header("기본 정보")]
    public string cardName;
    [TextArea(2, 5)]
    public string description;

    public Sprite icon;
    public CardGrade grade;

    [Header("효과")]
    public CardEffectType effectType;
    public float value1;        // 탄환 +1 같은거 처리용
    public float value2;        // 받는 피해 +20% 같은거 처리용?
    public float value3;        // 체력 50% 이하 적에게 피해 +100% 같은거 처리용?
}