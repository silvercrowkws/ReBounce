using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 몬스터의 속성
/// </summary>
public enum MonsterElementals
{
    Normal = 0,     // 기본
    Fire,           // 불
    Water,          // 물
    Land,           // 흙
    Electric,       // 전기
    Wind,           // 바람

    // 공과 몬스터의 상성 구조가
    // 불 < 물 < 흙 < 번개 < 바람 < 불
    // 불은   바람에 강하고    물에 약함
    // 물은   불에 강하고      흙에 약함
    // 흙은   물에 강하고      번개에 약함
    // 바람은 번개에 강하고    불에 약함
}

/// <summary>
/// 몬스터의 기믹여부
/// </summary>
public enum MonsterGimmicks
{
    None,       // 기믹 없음
    Heal,       // 주위 몬스터를 회복하는 몬스터
    Barrier,    // 베리어를 가져 댐감을 받는 몬스터(땅 공이나 물속성 카드의 방어율 무시 같은게 유용하도록)
    Shield,     // 특정 방향에 방패가 있어서 그 방향으로 오는 공격은 피해를 받지 않는 몬스터 
    Magnetic,   // 주위 공을 끌어당기고 튕기지 않게 하는 자석 같은 몬스터?

    // - 특정 속성에 면역인 몬스터??
    // - 보스 몬스터 여러마리 중에 하나의 기믹으로 턴마다 필드의 가장 아랫줄을 제외하고 빈 공간에 몬스터를 N마리 스폰하는거지(와 이거 개쩐다)
}

/// <summary>
/// 몬스터의 타입
/// </summary>
public enum SpawnMonsterType
{
    Normal,
    Gimmick,
    Boss,
}

public class MonsterBase : RecycleObject, IDamageable
{
    /// <summary>
    /// 이 몬스터의 속성(인스펙터에서 설정 가능)
    /// </summary>
    [SerializeField] 
    private MonsterElementals monsterElementals = MonsterElementals.Normal;

    /// <summary>
    /// 외부에서 접근할 때는 프로퍼티로만 사용
    /// </summary>
    public MonsterElementals MonsterElement => monsterElementals;

    /// <summary>
    /// 이 몬스터의 기믹
    /// </summary>
    [SerializeField]
    private MonsterGimmicks monsterGimmick = MonsterGimmicks.None;

    public MonsterGimmicks MonsterGimmick => monsterGimmick;


    /// <summary>
    /// 이 몬스터의 스폰 타입
    /// </summary>
    [SerializeField]
    private SpawnMonsterType spawnMonsterType = SpawnMonsterType.Normal;

    public SpawnMonsterType SpawnType => spawnMonsterType;


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
                //currentHP = Mathf.Clamp(value, 0, maxHP);

                // HP 깎이는 것 반올림 처리
                currentHP = Mathf.Clamp(
                Mathf.Round(value),
                0,
                maxHP);

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

    /// <summary>
    /// 상태 이상 표시용 이미지
    /// </summary>
    Image statusEffectImage;

    /// <summary>
    /// 화상 상태이상 스프라이트
    /// </summary>
    private Sprite burnSprite;

    /// <summary>
    /// 젖음 상태이상 스프라이트
    /// </summary>
    private Sprite wetSprite;

    /// <summary>
    /// 매쉬 렌더러
    /// </summary>
    private MeshRenderer meshRenderer;

    /// <summary>
    /// 턴 매니저
    /// </summary>
    //TurnManager turnManager;

    public bool IsBurning => burnStackCount > 0;

    /// <summary>
    /// 기믹 표시용 스프라이트 렌더러
    /// </summary>
    private SpriteRenderer gimmickObjectRenderer;

    /// <summary>
    /// 회복 기믹 스프라이트
    /// </summary>
    private Sprite healGimmickSprite;

    /// <summary>
    /// 배리어 기믹 스프라이트
    /// </summary>
    private Sprite barrierGimmickSprite;

    /// <summary>
    /// 배리어 기믹의 피해 감소율 (0.5 = 50% 감소)
    /// </summary>
    [SerializeField]
    private float barrierReduction = 0.5f;

    protected virtual void Awake()
    {
        //currentHP = maxHP;    => 활성화 시 처리

        if (transform.childCount > 1)
        {
            Transform child = transform.GetChild(1);        // 캔버스 위치
            hpText = child.GetChild(1).GetComponent<TextMeshProUGUI>();

            if (hpText != null)
            {
                hpText.text = currentHP.ToString();
            }

            statusEffectImage = child.GetChild(0).GetComponent<Image>();

            if(statusEffectImage != null)
            {
                statusEffectImage.sprite = null;        // 이미지 비우고
                Color color = statusEffectImage.color;
                color.a = 0f;
                statusEffectImage.color = color;        // 투명 처리
            }
        }

        burnSprite = Resources.Load<Sprite>("StatusEffect/BurnState");
        wetSprite = Resources.Load<Sprite>("StatusEffect/WetState");

        meshRenderer = GetComponent<MeshRenderer>();
        GameManager.Instance.onMaterialLoaded += ApplyMaterial;

        // 기믹 표시용 오브젝트 처리 부분
        gimmickObjectRenderer = transform.GetChild(2).GetComponent<SpriteRenderer>();

        if (gimmickObjectRenderer != null)
        {
            gimmickObjectRenderer.sprite = null;
            gimmickObjectRenderer.enabled = false;   // 기본은 꺼둠
        }

        // 기믹 스프라이트도 같은 방식으로 로드
        healGimmickSprite = Resources.Load<Sprite>("Gimmick/Gimmick_Heal");
        barrierGimmickSprite = Resources.Load<Sprite>("Gimmick/Gimmick_Barrier");
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        //TurnManager.Instance.onTurnEnd += OnTurnEnd;
        // => MonsterSpawner 에서 처리하도록 수정

        Init();     // 초기화 처리
    }

    /// <summary>
    /// 스폰 시 몬스터 정보를 초기화
    /// 속성 적용, 기믹 적용, 스폰 타입 적용 담당
    /// </summary>
    /// <param name="spawnData"></param>
    public void Initialize(MonsterSpawnData spawnData)
    {
        monsterElementals = spawnData.element;
        monsterGimmick = spawnData.gimmick;
        spawnMonsterType = spawnData.spawnType;

        // 스폰 타입에 따라 체력 배수 적용
        switch (spawnMonsterType)
        {
            case SpawnMonsterType.Normal:
                break;

            case SpawnMonsterType.Gimmick:
                maxHP *= 2;
                break;

            case SpawnMonsterType.Boss:
                maxHP *= 5;
                break;
        }

        currentHP = maxHP;

        if (hpText != null)
            hpText.text = currentHP.ToString();

        ApplyGimmickVisual();   // 기믹 표시 갱신
        ApplyMaterial();
    }

    protected virtual void Init()
    {
        maxHP = GetMonsterHPByTurn();

        // Initialize 에서 몬스터 타입에 맞게 변경하도록 수정
        /*currentHP = maxHP;

        if (hpText != null)
        {
            hpText.text = currentHP.ToString();
        }*/

        // 몬스터 속성 랜덤 결정(0 ~ MonsterElementals의 길이 만큼)
        /*int randomElements = UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(MonsterElementals)).Length);
        monsterElementals = (MonsterElementals)randomElements;*/    //=> 가중치 방식으로 변경

        // => MonsterSpawner에서 처리하도록 수정
        /*monsterElementals = GetRandomElement();
        if (GameManager.Instance.IsMaterialLoaded)
        {
            ApplyMaterial();
        }*/

        /*if (statusEffectImage != null)
        {
            statusEffectImage.sprite = null;        // 이미지 비우고
            Color color = statusEffectImage.color;
            color.a = 0f;
            statusEffectImage.color = color;        // 상태이상 이미지 투명 처리
        }*/
        StateEffectColorControl(false);
    }

    /// <summary>
    /// 턴 진행 상황에 따라 스폰되는 몬스터의 체력을 변동하는 함수
    /// 초반은 완만하게 증가하고,
    /// 후반으로 갈수록 증가량이 커져 플레이어 성장 속도를 따라가도록 설계.
    /// </summary>
    /// <returns>현재 턴의 몬스터 체력</returns>
    private float GetMonsterHPByTurn()
    {
        // 테스트용 100 고정
        //return 100f;

        /*int turn = TurnManager.Instance.turnNumber + 1;

        // 6턴까지는 +8씩 증가
        if (turn < 7)
        {
            return 20f + (turn - 1) * 8f;
        }

        // 7턴 부터는 +10씩 증가
        return 60f + (turn - 6) * 10f;*/

        // 1~6턴 +8씩 60, 7~10턴 +10씩 100, 11~15턴 +15씩 175
        // 16~20턴 +25씩 300, 21~25턴 +25씩 475, 26~30턴 +45씩 700
        // 31턴 이후부터는 +50,55,60...씩 계속 증가
        int turn = TurnManager.Instance.turnNumber + 1;

        // 1~6턴 : +8
        if (turn <= 6)
            return 20f + (turn - 1) * 8f;

        // 7~10턴 : +10
        if (turn <= 10)
            return 60f + (turn - 6) * 10f;

        // 11~15턴 : +15
        if (turn <= 15)
            return 100f + (turn - 10) * 15f;

        // 16~20턴 : +25
        if (turn <= 20)
            return 175f + (turn - 15) * 25f;

        // 21~25턴 : +35
        if (turn <= 25)
            return 300f + (turn - 20) * 35f;

        // 26~30턴 : +45
        if (turn <= 30)
            return 475f + (turn - 25) * 45f;

        // 31턴 이후
        float hp = 700f;

        // 31턴부터 증가량 : 50, 55, 60, 65...
        for (int t = 31; t <= turn; t++)
        {
            hp += 50f + (t - 31) * 5f;
        }

        return hp;
    }

    private void ApplyMaterial()
    {
        meshRenderer.sharedMaterial =
            GameManager.Instance.GetMonsterMaterial(
                monsterElementals);
    }

    /*/// <summary>
    /// 가중치 랜덤 속성 결정 함수 => MonsterSpawner로 이전
    /// </summary>
    /// <returns></returns>
    private MonsterElementals GetRandomElement()
    {
        int rand = UnityEngine.Random.Range(0, 100);

        if (rand < 50)
            return MonsterElementals.Normal;    // 50% 확률로 노말

        if (rand < 60)
            return MonsterElementals.Fire;      // 10% 확률로 불

        if (rand < 70)
            return MonsterElementals.Water;     // 10% 확률로 물

        if (rand < 80)
            return MonsterElementals.Land;      // 10% 확률로 땅

        if (rand < 90)
            return MonsterElementals.Electric;  // 10% 확률로 전기

        return MonsterElementals.Wind;          // 10% 확률로 바람 
    }*/

    /// <summary>
    /// 상태이상 이미지 컨트롤 함수
    /// </summary>
    /// <param name="tf"></param>
    private void StateEffectColorControl(bool tf)
    {
        // 비활성화 처리
        if (!tf)
        {
            if (statusEffectImage != null)
            {
                statusEffectImage.sprite = null;        // 이미지 비우고
                Color color = statusEffectImage.color;
                color.a = 0f;
                statusEffectImage.color = color;        // 투명 처리
            }
        }
        else
        {
            Color color = statusEffectImage.color;
            color.a = 1f;
            statusEffectImage.color = color;
        }
    }

    public virtual void TakeDamage(float amount)
    {
        /*CurrentHP -= amount;
        Debug.Log($"{gameObject.name}이 {amount}의 데미지. 남은 HP: {CurrentHP}");*/
        TakeDamage(amount, false, 0f);   // 배리어 정상 적용
    }

    public virtual void TakeDamage(float amount, bool ignoreBarrier, float barrierIgnorePercent = 0f)
    {
        float finalAmount = amount;

        if (!ignoreBarrier && monsterGimmick == MonsterGimmicks.Barrier)
        {
            // 와류 등으로 배리어 감소율 자체가 깎임 (0 밑으로는 안 내려가게)
            float effectiveReduction =
                Mathf.Clamp01(barrierReduction - barrierIgnorePercent);

            finalAmount = amount * (1f - effectiveReduction);
        }

        CurrentHP -= finalAmount;

        Debug.Log($"{gameObject.name}이 {finalAmount}의 데미지(원본 {amount}). 남은 HP: {CurrentHP}");
    }

    public virtual void TakeStatusEffect(StatusEffectData effect)
    {
        switch (effect.effectType)
        {
            case StatusEffectType.Normal:
                SoundManager.Instance.PlayNormalHit();
                break;

            // 화상 상태
            case StatusEffectType.Burn:
                //StartCoroutine(BurnCoroutine(effect));

                // 젖음 상태라면 화상은 걸리지 않고 즉시 데미지만 적용
                if (isWet)
                {
                    float instantDamage = effect.value * effect.duration;

                    TakeDamage(instantDamage);

                    Debug.Log($"젖음 상태에서 화상 반응! 즉시 데미지 : {instantDamage}");

                    // 폭발 소리는 여기
                    SoundManager.Instance.PlayExplosion();

                    return;
                }

                Coroutine burn = StartCoroutine(BurnCoroutine(effect));
                burnCoroutines.Add(burn);
                break;

            // 젖음 상태
            case StatusEffectType.Wet:
                //StartCoroutine(WetCoroutine(effect));

                SoundManager.Instance.PlayWet();

                if (burnStackCount > 0)
                {
                    TakeDamage(remainBurnDamage);

                    Debug.Log($"화상 상태에서 젖음 반응! 남은 화상 데미지 폭발 : {remainBurnDamage}");

                    // 폭발 소리는 여기
                    SoundManager.Instance.PlayExplosion();

                    // 모든 화상 제거
                    foreach (Coroutine coroutine in burnCoroutines)
                    {
                        if (coroutine != null)
                        {
                            StopCoroutine(coroutine);
                        }
                    }

                    burnCoroutines.Clear();

                    burnStackCount = 0;
                    remainBurnDamage = 0f;
                }

                // 이미 젖음 상태라면 지속시간 갱신
                if (wetCoroutine != null)
                {
                    StopCoroutine(wetCoroutine);
                }

                if (CurrentHP <= 0)
                    return;

                wetCoroutine = StartCoroutine(WetCoroutine(effect));
                break;

            // 땅 속성 방어 무시 공격
            case StatusEffectType.IgnoreDefense:
                float bonusDamage = Mathf.Ceil(effect.baseDamage * effect.value);
                
                TakeDamage(bonusDamage, true);

                SoundManager.Instance.PlayIgnoreDefenseClip();

                Debug.Log($"땅 속성 기본 피해 : {bonusDamage}");
                break;

            // 번개 속성 감전
            case StatusEffectType.ChainLightning:
                ApplyChainLightning(effect);
                break;

            // 바람 속성 관통 공격
            case StatusEffectType.Pierce:
                ApplyPierce(effect);
                break;
        }
    }

    /// <summary>
    /// 화상 중첩 개수
    /// </summary>
    private int burnStackCount = 0;

    /// <summary>
    /// 현재 활성화된 화상 코루틴들
    /// </summary>
    private List<Coroutine> burnCoroutines = new List<Coroutine>();

    /// <summary>
    /// 남아있는 화상 총 누적 데미지
    /// </summary>
    private float remainBurnDamage = 0f;

    /// <summary>
    /// 화상 코루틴
    /// </summary>
    /// <param name="effect"></param>
    /// <returns></returns>
    IEnumerator BurnCoroutine(StatusEffectData effect)
    {
        burnStackCount++;       // 화상 중첩 개수 증가

        float elapsedTime = 0f;
        float tickInterval = 1f;   // 1초마다 데미지

        // 화상 총 누적 데미지 등록
        remainBurnDamage += effect.value * effect.duration;

        // 첫 화상이면 이미지 활성화
        if (burnStackCount == 1)
        {
            StateEffectColorControl(true);
            statusEffectImage.sprite = burnSprite;      // 상태 이상 이미지 변경
        }

        while (elapsedTime < effect.duration)
        {
            TakeDamage(effect.value);

            // 실제 들어간 데미지만큼 감소
            remainBurnDamage -= effect.value;

            // 화상 효과음은 여기
            SoundManager.Instance.PlayBurn();

            yield return new WaitForSeconds(tickInterval);

            elapsedTime += tickInterval;
        }

        burnStackCount--;       // while 문 종료시 화상 중첩 개수 감소

        // 현재 끝난 코루틴 정리
        burnCoroutines.RemoveAll(c => c == null);

        /*// 화상 중첩이 하나도 안남았을 때만 끄기
        if (burnStackCount <= 0)
        {
            burnStackCount = 0;
            StateEffectColorControl(false);
        }*/

        // 모든 화상 종료 시
        if (burnStackCount <= 0)
        {
            burnStackCount = 0;
            remainBurnDamage = 0f;

            // 젖음 상태가 아니라면만 끄기
            if (!isWet)
            {
                StateEffectColorControl(false);
            }
        }
    }

    /// <summary>
    /// 현재 젖음 상태 코루틴
    /// </summary>
    private Coroutine wetCoroutine;

    /// <summary>
    /// 현재 젖음 상태 여부
    /// </summary>
    private bool isWet = false;

    public bool IsWet => isWet;

    /// <summary>
    /// 젖음 코루틴
    /// </summary>
    /// <param name="effect"></param>
    /// <returns></returns>
    IEnumerator WetCoroutine(StatusEffectData effect)
    {
        isWet = true;

        // 상태 이상 이미지 표시
        StateEffectColorControl(true);
        statusEffectImage.sprite = wetSprite;

        float elapsedTime = 0f;

        while (elapsedTime < effect.duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isWet = false;
        wetCoroutine = null;

        // 젖음 종료
        StateEffectColorControl(false);
    }

    public virtual void OnDie()
    {
        Debug.Log($"{gameObject.name} 사망!");
        StopAllCoroutines();

        // 활성화된 몬스터 리스트에서 제거
        MonsterSpawner.Instance.UnregisterMonster(this);

        gameObject.SetActive(false);

        /*// 턴 매니저 구독 취소
        if (TurnManager.Instance != null)
            TurnManager.Instance.onTurnEnd -= OnTurnEnd;*/
        //=> MonsterSpawner에서 처리하도록 수정
    }

    /*private void OnTurnEnd()
    {
        transform.position += new Vector3(0f, 0f, -0.31f);
    }*/

    private void ApplyChainLightning(StatusEffectData effect)
    {
        Vector3 center = transform.position;

        //float chainRange = 0.31f;
        float chainRange = effect.duration;

        float chainDamage =
            Mathf.Ceil(effect.baseDamage * effect.value);

        // 전이를 주는 주체 자기 자신 이펙트
        Factory.Instance.GetChainLightning(
            new Vector3(
                transform.position.x,
                transform.position.y + 0.14f,
                transform.position.z)
        ).transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // 전이 효과음은 여기
        SoundManager.Instance.PlayChainLightning();

        Collider[] hits = Physics.OverlapSphere(
            center,
            chainRange,
            LayerMask.GetMask("Monster"));

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            IDamageable other = hit.GetComponent<IDamageable>();

            if (other == null)
                continue;

            Vector3 dir = hit.transform.position - center;

            dir.y = 0f;
            dir.Normalize();

            float horizontal =
                Mathf.Abs(Vector3.Dot(dir, Vector3.right));

            float vertical =
                Mathf.Abs(Vector3.Dot(dir, Vector3.forward));

            bool isCrossDirection =
                horizontal > 0.9f ||
                vertical > 0.9f;

            if (!isCrossDirection)
                continue;

            // 전이 대상 위치에 이펙트 생성
            Factory.Instance.GetChainLightning(
                new Vector3(
                    hit.transform.position.x,
                    hit.transform.position.y + 0.14f,
                    hit.transform.position.z)
            ).transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            other.TakeDamage(chainDamage);

            Debug.Log(
                $"{hit.name} 에게 번개 전이 피해 : {chainDamage}");
        }
    }

    private void ApplyPierce(StatusEffectData effect)
    {
        // 관통 범위
        float pierceRange = effect.duration;

        // 관통 피해 계산
        float pierceDamage =
            Mathf.Ceil(effect.baseDamage * effect.value);

        // 맞은 몬스터 위치에 관통 이펙트 생성
        Factory.Instance.GetWindPierce(
            new Vector3(
                transform.position.x,
                transform.position.y + 0.14f,
                transform.position.z)
        ).transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // 효과음 재생
        SoundManager.Instance.PlayPierce();

        // 공이 이동하던 방향 저장(위로 이동 중이었다면 위쪽 탐색)
        Vector3 direction;

        // 공이 위쪽으로 이동 중 블록과 충돌했다면 위 방향으로 관통
        if (effect.directionValue.z > 0f)
        {
            //direction = transform.position + Vector3.forward * pierceRange;
            direction = Vector3.forward;
        }
        // 아래로 이동 중이었다면 아래 방향으로 관통
        else
        {
            //direction = transform.position + Vector3.back * pierceRange;
            direction = Vector3.back;
        }


        // 현재 관통 가능한 칸 수 계산
        // ex)
        // 0.31f → 1칸
        // 0.62f → 2칸
        // 0.93f → 3칸
        int rangeCount =
            Mathf.RoundToInt(pierceRange / 0.31f);

        // 1칸부터 최대 관통 거리까지 순서대로 검사
        for (int i = 1; i <= rangeCount; i++)
        {
            // 현재 검사할 위치
            // i가 증가할수록 한 칸씩 더 멀리 검사
            Vector3 searchPos =
                transform.position + direction * 0.31f * i;

            Collider[] hits = Physics.OverlapSphere(
                searchPos,
                0.1f,
                LayerMask.GetMask("Monster"));

            foreach (Collider hit in hits)
            {
                // 자기 자신은 제외
                if (hit.gameObject == gameObject)
                    continue;

                IDamageable other = hit.GetComponent<IDamageable>();

                if (other == null)
                    continue;

                // 관통 대상 위치에 이펙트 생성
                Factory.Instance.GetWindPierce(
                    new Vector3(
                        hit.transform.position.x,
                        hit.transform.position.y + 0.14f,
                        hit.transform.position.z)
                ).transform.rotation = Quaternion.Euler(90f, 0f, 0f);

                // 관통 피해 적용
                other.TakeDamage(pierceDamage);

                Debug.Log(
                    $"{hit.name} 에게 관통 피해 : {pierceDamage}");

                // 현재 칸에는 한 마리만 공격
                // 다음 칸도 검사해야 하므로 for문은 계속 진행된다.
                break;
            }
        }
    }

    /// <summary>
    /// 기믹 수행의 감지 범위
    /// 상하좌우만 생각하면 0.31이면 충분한데,
    /// 대각선은 √가 들어가서 약 0.4384
    /// 넉넉히 0.45 함
    /// </summary>
    [SerializeField]
    private float healGimmickRange = 0.45f;

    /// <summary>
    /// 턴 종료 시 실행될 기믹
    /// </summary>
    public virtual void OnTurnEndGimmick()
    {
        switch (monsterGimmick)
        {
            case MonsterGimmicks.Heal:
                ApplyHealGimmick();
                break;

                // Barrier, Shield, Magnetic은 이 트리거를 쓰지 않으므로 여기 없음
        }
    }

    /// <summary>
    /// 대미지를 받기 전에 실행될 기믹
    /// </summary>
    /// <param name="incomingDamage"></param>
    /// <returns></returns>
    public virtual float OnBeforeTakeDamageGimmick(float incomingDamage)
    {
        switch (monsterGimmick)
        {
            case MonsterGimmicks.Barrier:
                // TODO: 방어율 무시 무효화 등 실제 로직
                return incomingDamage;
        }

        return incomingDamage; // 해당 기믹 아니면 원본 그대로
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="attackDirection"></param>
    /// <returns></returns>
    public virtual bool OnCheckBlockGimmick(Vector3 attackDirection)
    {
        switch (monsterGimmick)
        {
            case MonsterGimmicks.Shield:
                // TODO: 방패 방향과 attackDirection 비교
                return false;
        }

        return false; // 해당 기믹 아니면 막지 않음
    }

    /// <summary>
    /// 회복 기믹 수행 함수
    /// </summary>
    private void ApplyHealGimmick()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            healGimmickRange,
            LayerMask.GetMask("Monster"));

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            IDamageable other = hit.GetComponent<IDamageable>();

            if (other == null)
                continue;

            float lostHP = other.MaxHP - other.CurrentHP;

            if (lostHP <= 0f)
                continue;

            // 소수점 발생 시 무조건 내림
            float healAmount = Mathf.Floor(lostHP * 0.5f);

            if (healAmount <= 0f)
                continue;   // 내림 결과 0이면 회복 의미 없으니 스킵

            other.CurrentHP += healAmount;

            Debug.Log($"{gameObject.name}의 회복 기믹 → {hit.name} 이(가) {healAmount} 회복");
        }
    }

    /// <summary>
    /// 몬스터가 가진 기믹에 맞는 스프라이트를 GimmickObject에 표시하는 함수
    /// </summary>
    private void ApplyGimmickVisual()
    {
        if (gimmickObjectRenderer == null)
            return;

        switch (monsterGimmick)
        {
            case MonsterGimmicks.Heal:
                gimmickObjectRenderer.sprite = healGimmickSprite;
                gimmickObjectRenderer.enabled = true;
                break;

            case MonsterGimmicks.Barrier:
                gimmickObjectRenderer.sprite = barrierGimmickSprite;
                gimmickObjectRenderer.enabled = true;
                break;
            // Barrier, Shield, Magnetic 스프라이트는 준비되는 대로 case 추가

            default:
                gimmickObjectRenderer.sprite = null;
                gimmickObjectRenderer.enabled = false;
                break;
        }
    }
}
