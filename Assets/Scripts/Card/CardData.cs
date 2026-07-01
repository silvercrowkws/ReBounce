using UnityEngine;

public enum CardEffectType
{
    None,

    // 희귀 ----------

    ExtraBullets,               // 추가 탄환 : 노말 공 +1
    SteadyShot,                 // 안정된 발사 : 최대 발사 각도 +5°
    ModifiedBullets,            // 탄환 개조 : 모든 공 공격력 +2 => 1으로 변경
    FireInfusion,               // (불)원소 주입 : 불 공 +1개 획득
    WaterInfusion,              // (물)원소 주입 : 물 공 +1개 획득
    LandInfusion,               // (땅)원소 주입 : 땅 공 +1개 획득
    ElectricInfusion,           // (전기)원소 주입 : 전기 공 +1개 획득
    WindInfusion,               // (바람)원소 주입 : 바람 공 +1개 획득

    StrongBurn,                 // 강한 화상 : 화상 피해 +2 => 1으로 변경
    ResidualFire,               // 잔불 : 화상 지속 시간 +1초
    Ignition,                   // 점화 : 화상 상태의 대상에게 가하는 피해 +20%

    WaterAccumulation,          // 수분 축적 : 물 공 공격력 +5 => 4으로 변경
    Cooling,                    // 냉각 : 젖음 상태의 적이 받는 피해 +10% => 15으로 변경
    PurifyingWater,             // 정화수 : 물 공 적중 시 젖음 지속시간 +5초

    Shatter,                    // 파쇄 : 땅 공 공격력 +5 => 3으로 변경
    Crush,                      // 압괴 : 체력이 50% 미만인 적에게 땅 공 피해 +50% => 30으로 변경
    Crack,                      // 균열 : 땅 공의 추가 피해 배율 +20% => 15으로 변경

    AmplificationCircuit,       // 증폭 회로 : 전기 공 공격력 +5 => 3으로 변경
    Overcurrent,                // 과전류 : 전이 피해 +10%
    VoltageFocus,               // 전압 집중 : 전기 공의 직접 피해 +20% => 15으로 변경

    Gale,                       // 강풍 : 바람 공 공격력 +5 => 3으로 변경
    SharpWind,                  // 날카로운 바람 : 관통 피해 +20%
    Turbulence,                 // 난기류 : 바람 공이 반사될 때마다 피해 +10%

    // 희귀 끝 ----------

    // 영웅 ----------

    MultiLoad,                  // 다중 장전 : 노말 공 +3개 획득
    LargeCaliberBullets,        // 대구경 탄환 : 노말 공 공격력 +15 => 8으로 변경
    ReinforcedBullet,           // 강화 탄환 : 모든 공 공격력 +5 => 3으로 변경
    Sharpshooter,               // 명사수 : 최대 발사 각도 +10
    FlameBullets,               // 화염 탄환 : 불 공 +2개 획득
    AquaBullets,                // 수류 탄환 : 물 공 +2개 획득
    StoneBullets,               // 암석 탄환 : 땅 공 +2개 획득
    LightningBullets,           // 전류 탄환 : 전기 공 +2개 획득
    SwiftwindBullets,           // 질풍 탄환 : 바람 공 +2개 획득

    SearingHeat,                // 고열 : 화상 피해 +5 => 3으로 변경
    BlazingFlame,               // 타오르는 불꽃 : 화상 지속 시간 +2초
    FocusedFire,                // 화력 집중 : 화상 상태의 적이 받는 피해 + 40%

    Torrent,                    // 급류 : 물 공 공격력 +10 => 8으로 변경
    Freeze,                     // 빙결 : 젖음 상태 적이 받는 피해 +20% => 30으로 변경
    Flood,                      // 범람 : 젖음 상태 부여시 필드 내 다른 적에게 젖음 전파 +1(최대 5중첩)

    Monolith,                   // 거암 : 땅 공 공격력 +10 => 6으로 변경
    Collapse,                   // 붕괴 : 체력 50% 미만 적에게 땅 공 피해 +100% => 60으로 변경
    Pulverize,                  // 분쇄 : 땅 공의 추가 피해 배율 +50% => 30으로 변경

    Superconductivity,          // 초전도 : 전기 공 공격력 +10 => 6으로 변경
    LightningStrike,            // 낙뢰 : 전기 공의 직접 피해 +50% => 35으로 변경
    ExtendedCircuit,            // 확장 회로 : 전이 범위 +1(중접X)

    Storm,                      // 폭풍 : 바람 공 공격력 +10 => 6으로 변경
    RazorWind,                  // 칼바람 : 관통 피해 +50%
    Updraft,                    // 상승 기류 : 관통 범위 +1(중첩X)

    // 영웅 끝 ----------

    // 전설 ----------

    Incineration,               // 소각 : 화상 피해 100% 증가 => 50%으로 변경
    Ashes,                      // 잿더미 : 화상 피해에 대상 최대 체력의 1%를 추가한다.

    Tsunami,                    // 해일 : 범람이 모든 젖지 않은 적에게 적용된다.
    Vortex,                     // 와류 : 젖음 상태 적에게 가하는 피해가 방어 효과를 10% 무시한다(수치 조정 필요)

    Earthquake,                 // 지진 : 좌우 적에게 피해의 50%(최대 2중첩)
    Pulverization,              // 압쇄 : 땅 공의 추가 피해가 적의 현재 체력의 5%를 추가로 가한다.

    Thunderburst,               // 뇌폭 : 전이 피해가 직접 피해와 동일해짐(추가 배율은 유지)
    HighVoltage,                // 초고압 : 전기 공 적중 시 추가 전이 +1(중첩X)

    Typhoon,                    // 태풍 : 관통 거리 무제한
    JetStream,                  // 제트기류 : 바람 공이 반사될 때마다 피해 +30%

    // 전설 끝 ----------
}

public enum CardElementals
{
    Normal,
    Fire,
    Water,
    Land,
    Electric,
    Wind,
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
    public CardElementals elementals;

    [Header("효과")]
    public CardEffectType effectType;
    public float value1;        // 탄환 +1 같은거 처리용
    public float value2;        // 받는 피해 +20% 같은거 처리용?
    public float value3;        // 체력 50% 이하 적에게 피해 +100% 같은거 처리용?

    [Header("중복 획득 가능 여부")]
    [Tooltip("true : 중복 획득 가능\nfalse : 1회만 획득 가능")]
    public bool canDuplicate = true;
}