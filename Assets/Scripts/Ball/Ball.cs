using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum BallElementals
{
    Normal = 0,     // 기본
    Fire,           // 불            =>  화상을 남겨 지속피해를 n초동안 준다거나
    Water,          // 물            =>  피격 시 '젖음' 디버프 부여
    Land,           // 흙, 땅?       =>  방어를 무시하는 고정피해?
    Electric,       // 번개          =>  주변 블록으로 데미지의 일부가 전이된다거나
    Wind,           // 바람          =>  추가 타격이나 뒤의 몬스터도 맞는 관통 공격?

    // '젖음' 디버프 효과는
    // - 불과 만나면 남은 화상 데미지를 한번에 주고 n초 갱신?
    // - 흙과 만나면 진흙 상태로 변해서 몬스터의 방어율 감소?
    // - 번개와 만나면 전이 범위 증가?
    // - 바람과 만나면 주변 몬스터들에게 '젖음' 확산?
}

public class Ball : RecycleObject
{
    /// <summary>
    /// 이 공의 속성(인스펙터에서 설정 가능)
    /// </summary>
    public BallElementals ballElementals = BallElementals.Normal;

    /// <summary>
    /// 공의 속도
    /// </summary>
    public float speed = 10f;

    /// <summary>
    /// 이 공의 대미지
    /// </summary>
    public float damage = 10f;
    public float Damage => damage;

    /// <summary>
    /// 이 공의 기본 대미지
    /// </summary>
    [SerializeField]
    private float baseDamage = 10f;

    private Vector3 direction;

    /*private static bool isFirstGroundHit = false;
    private static Vector3 firstGroundHitPos;*/

    /// <summary>
    /// 콜라이더
    /// </summary>
    SphereCollider sphereCollider;

    /// <summary>
    /// 리지드바디
    /// </summary>
    Rigidbody rb;

    /// <summary>
    /// 매쉬 렌더러
    /// </summary>
    MeshRenderer meshRenderer;

    /// <summary>
    /// 무속성 공
    /// </summary>
    Color whiteBall = new Color(1f, 1f, 1f, 1f);
    
    /// <summary>
    /// 불속성 공
    /// </summary>
    Color fireBall = new Color(1f, 0.1f, 0.1f, 1f);

    /// <summary>
    /// 물속성 공
    /// </summary>
    Color waterBall = new Color(0.1f, 0.1f, 1f, 1f);
    
    /// <summary>
    /// 땅속성 공
    /// </summary>
    Color landBall = new Color(0.45f, 0.3f, 0.15f, 1f);
    
    /// <summary>
    /// 전기속성 공
    /// </summary>
    Color electricBall = new Color(1f, 0.95f, 0.2f, 1f);
    
    /// <summary>
    /// 바람속성 공
    /// </summary>
    Color windcBall = new Color(0.25f, 0.95f, 0.7f, 1f);

    /// <summary>
    /// 턴 매니저
    /// </summary>
    TurnManager turnManager;

    /// <summary>
    /// 볼 슈터
    /// </summary>
    BallShooter ballShooter;

    /// <summary>
    /// 기본 화상 대미지
    /// </summary>
    public float baseBurnDamage = 5f;

    /// <summary>
    /// 기본 화상 시간
    /// </summary>
    public float baseBurnDuration = 5f;

    /// <summary>
    /// 기본 젖음 지속시간
    /// </summary>
    public float baseWetDuration = 10f;

    /// <summary>
    /// 기본 전이 피해 배율
    /// </summary>
    public float baseChainValue = 0.3f;
    
    /// <summary>
    /// 기본 관통 피해 배율
    /// </summary>
    public float basePierceValue = 1f;

    /// <summary>
    /// 현재 공이 반사된 횟수
    /// (첫 충돌부터 배율 적용되지 않도록 -1)
    /// </summary>
    private int bounceCount = -1;

    /// <summary>
    /// 전기 공의 기본 전이 범위
    /// </summary>
    private float baseChainRange = 0.31f;

    /// <summary>
    /// 바람 공의 기본 전이 범위
    /// </summary>
    private float basePierceRange = 0.31f;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;      // 빠른 속도의 물체가 콜라이더를 뚫는 것 방지

        int ballLayer = LayerMask.NameToLayer("Ball");

        Physics.IgnoreLayerCollision(ballLayer, ballLayer, true);

        meshRenderer = GetComponent<MeshRenderer>();

        turnManager = TurnManager.Instance;

        ballShooter = FindAnyObjectByType<BallShooter>();
    }

    protected override void OnEnable()
    {
        ResetBall();
        ResetBallElementals();

        turnManager.RegisterBall();
    }

    /*protected override void OnDisable()
    {
        turnManager.UnregisterBall();
    }*/

    public void Init(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void FixedUpdate()
    {
        Move();
    }    


    void Move()
    {
        //Vector3 move = direction * speed * Time.deltaTime;
        //transform.position += move;

        //rb.velocity = direction * speed;

        if (!rb.isKinematic)
        {
            rb.velocity = direction * speed;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 바닥 체크
        if (collision.gameObject.CompareTag("DownBrick"))
        {
            //Debug.Log("바닥 충돌");
            // 물리 완전 정지
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // 위치 고정 (살짝 밀리는 것 방지)
            rb.isKinematic = true;      // 재활용시 false 필요

            // 논리 이동도 정지
            direction = Vector3.zero;

            sphereCollider.enabled = false;     // 바닥에 닿으면 콜라이더 끄고

            HandleGroundHit();
            
            turnManager.UnregisterBall();       // 땅에 닿은 공 카운팅에서 빼기
            gameObject.SetActive(false);
            return;
        }

        // Ball이 아닌 대상과 충돌하면
        if (!collision.gameObject.CompareTag("Ball"))
        {
            // 반사 횟수++
            bounceCount++;

            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

            // 충돌한 대상에게 damageable 인터페이스가 있으면
            if (damageable != null)
            {
                // 데미지 적용 함수
                CalculateDamage(damageable, damageable.MonsterElement);
                //damageable.TakeDamage(damage);
                // 나중에 속성 상성 이런 것 별로 기능하도록 수정 필요
            }
            
            // 반사 처리
            Vector3 normal = collision.contacts[0].normal;

            // 반사 공식
            direction = Vector3.Reflect(direction, normal);

            // 살짝 밀어내기
            transform.position += normal * 0.02f;
        }
    }

    void HandleGroundHit()
    {
        GameManager.Instance.RegisterFirstGroundHit(transform.position);
    }

    private void ResetBall()
    {
        // 물리 상태 복구
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 콜라이더 복구
        sphereCollider.enabled = true;

        // 이동값 초기화
        direction = Vector3.zero;
        //isFirstGroundHit 는 static 변수라서 OnDisable에서 초기화 하면 안되고.
        // 스테이지 초기화? 내 경우에는 다음 발사 직전에 초기화 하면 될듯

        // 공의 대미지 설정
        damage = baseDamage + ballShooter.allBonusDamage;
        

        // 반사 횟수 초기화
        bounceCount = -1;
        /*// 각 속성에 따른 추가 대미지 있을 경우 적용
        switch (ballElementals)
        {
            case BallElementals.Fire:
                break;

            case BallElementals.Water:
                damage += ballShooter.waterBonusDamage;
                Debug.Log($"물 공의 최종 공격력 : {damage}");
                break;

            case BallElementals.Land:
                //damage += ballShooter.landBonusDamage;
                break;

            case BallElementals.Electric:
                //damage += ballShooter.electricBonusDamage;
                break;

            case BallElementals.Wind:
                //damage += ballShooter.windBonusDamage;
                break;
        }*/
    }

    public void SetElemental(BallElementals elemental)
    {
        ballElementals = elemental;
        ResetBallElementals();
    }

    private void ResetBallElementals()
    {
        // 각 속성에 따른 추가 대미지 있을 경우 함께 적용
        switch (ballElementals)
        {
            case BallElementals.Normal:
                meshRenderer.material.color = whiteBall;
                break;

            case BallElementals.Fire:
                meshRenderer.material.color = fireBall;
                break;

            case BallElementals.Water:
                meshRenderer.material.color = waterBall;
                damage += ballShooter.waterBonusDamage;
                break;
            
            case BallElementals.Land:
                meshRenderer.material.color = landBall;
                damage += ballShooter.landBonusDamage;
                break;
            
            case BallElementals.Electric:
                meshRenderer.material.color = electricBall;
                damage += ballShooter.electricBonusDamage;
                break;
            
            case BallElementals.Wind:
                meshRenderer.material.color = windcBall;
                damage += ballShooter.windBonusDamage;
                break;
        }
    }

    /// <summary>
    /// 데미지 계산 함수
    /// </summary>
    /// <param name="damageable">IDamageable 인터페이스</param>
    private void CalculateDamage(IDamageable damageable, MonsterElementals monsterElement)
    {
        // 충돌한 대상에게 damageable 인터페이스가 있으면 CalculateDamage 이 함수가 실행되는건데,
        // 일단 공의 속성은 쉽게 알 수 있고,
        // 충돌한 대상의 태그를 받아와? => 태그 대신 인터페이스로 처리해서 몬스터의 속성을 받아왔어.

        // 기본 데미지 * 상성 테이블
        /*float multipliedDamage = 
            damage * elementalTable[(int)ballElementals, (int)monsterElement];*/

        float finalDamage =
        damage * elementalTable[(int)ballElementals,
                                (int)monsterElement];

        MonsterBase monster = damageable as MonsterBase;

        // 화상 상태 적 추가 피해
        if (monster != null && monster.IsBurning)
        {
            finalDamage *=
                1f + ballShooter.ignitedBonusDamage;
        }

        // 젖음 상태 적 추가 피해
        if (monster.IsWet)
        {
            finalDamage *=
                1f + ballShooter.wetBounsDamage;
        }

        // 균열: 땅 공의 추가 피해 배율 + 20%
        if(monster!= null && ballElementals == BallElementals.Land)
        {
            // 압괴 적용?
            if(monster.CurrentHP <= monster.MaxHP * 0.5f)
            {
                finalDamage *=
                    1f + ballShooter.landHalfHPBonusDamage + ballShooter.landExtraBonusDamage;
            }
            // 균열만 적용?
            else
            {
                finalDamage *=
                    1f + ballShooter.landExtraBonusDamage;
            }
        }

        // 전기 공의 직접 피해
        if(monster != null && ballElementals == BallElementals.Electric)
        {
            finalDamage *=
                1f + ballShooter.electricDirectBonusDamage;
        }

        if(monster != null && ballElementals == BallElementals.Normal)
        {
            finalDamage += ballShooter.normalBonusDamage;
        }
        

        // 효과 적용 함수 실행
        ApplyElementalEffect(damageable);

        // 배리어 관련 판정
        bool ignoreBarrier = (ballElementals == BallElementals.Land);   // 땅 속성은 배리어 완전 무시
        float barrierIgnorePercent = 0f;

        // 와류 : 젖음 상태의 적에게 가하는 피해만 배리어 일부 무시
        if (monster != null && monster.IsWet)
        {
            barrierIgnorePercent = ballShooter.vortex;
        }


        // 최종적으로 계산된 데미지 적용
        damageable.TakeDamage(finalDamage, ignoreBarrier, barrierIgnorePercent);

        if(ballElementals == BallElementals.Land)
        {
            ApplyEarthquake(damageable, finalDamage);
        }
    }

    /// <summary>
    /// 공과 몬스터 사이의 상성 테이블
    /// </summary>
    private static readonly float[,] elementalTable =
    {
        //대상:       Normal  Fire    Water   Land    Elec    Wind
        /*Nomal*/     {1f,    1f,     1f,     1f,     1f,     1f,},
        /*Fire*/      {1f,    1f,     0.5f,   1f,     1f,     1.5f,},
        /*Water*/     {1f,    1.5f,   1f,     0.5f,   1f,     1f,},
        /*Land*/      {1f,    1f,     1.5f,   1f,     0.5f,   1f,},
        /*Elec*/      {1f,    1f,     1f,     1.5f,   1f,     0.5f,},
        /*Wind*/      {1f,    0.5f,   1f,     1f,     1.5f,   1f,},
    };

    /// <summary>
    /// Ball의 상태이상, 특수 공격 효과 함수
    /// </summary>
    private void ApplyElementalEffect(IDamageable target)
    {
        //Normal = 0,     // 기본
        //Fire,           // 불            =>  화상을 남겨 지속피해를 n초동안 준다거나
        //Water,          // 물            =>  피격 시 '젖음' 디버프 부여
        //Land,           // 흙, 땅?       =>  방어를 무시하는 고정피해?
        //Electric,       // 번개          =>  주변 블록으로 데미지의 일부가 전이된다거나
        //Wind,           // 바람          =>  추가 타격이나 뒤의 몬스터도 맞는 관통 공격?

        // '젖음' 디버프 효과는
        // - 불과 만나면 남은 화상 데미지를 한번에 주고 n초 갱신?
        // - 흙과 만나면 진흙 상태로 변해서 몬스터의 방어율 감소?
        // - 번개와 만나면 전이 범위 증가?
        // - 바람과 만나면 주변 몬스터들에게 '젖음' 확산?

        switch (ballElementals)
        {
            case BallElementals.Normal:
                ApplyNormal(target);
                break;

            case BallElementals.Fire:
                // 화상 디버프 부여
                ApplyBurn(target);
                break;

            case BallElementals.Water:
                // 젖음 디버프 부여
                ApplyWet(target);
                //ApplyWet(target);
                break;

            case BallElementals.Land:
                // 방어를 무시하는 고정 피해
                ApplyLand(target);
                break;

            case BallElementals.Electric:
                // 주변 블록으로 데미지 일부 전이
                ApplyElectric(target);
                break;

            case BallElementals.Wind:
                // 추가 타격 및 뒤의 몬스터도 맞는 관통 공격
                ApplyPierce(target);
                break;
        }
    }

    private void ApplyNormal(IDamageable target)
    {
        StatusEffectData normal = new StatusEffectData
        {
            effectType = StatusEffectType.Normal,
            duration = 0f,
            value = 0f
        };

        target.TakeStatusEffect(normal);
    }

    private void ApplyBurn(IDamageable target)
    {
        float burnDamage =
        (baseBurnDamage + ballShooter.bonusBurnDamage) *
        (1 + ballShooter.incinerationBonus);

        // 잿더미 : 최대 체력 비례 피해 추가(소수점 올림처리)
        burnDamage +=
            Mathf.Ceil(target.MaxHP * ballShooter.ashesBonus);

        StatusEffectData burn = new StatusEffectData
        {
            effectType = StatusEffectType.Burn,
            //duration = 5f,
            //value = 5f
            duration = baseBurnDuration + ballShooter.bonusBurnDuration,
            /*value = (1 + ballShooter.incinerationBonus) 
            * (baseBurnDamage + ballShooter.bonusBurnDamage),*/
            value = burnDamage,
        };

        target.TakeStatusEffect(burn);
    }

    private void ApplyWet(IDamageable target)
    {
        StatusEffectData wet = new StatusEffectData
        {
            effectType = StatusEffectType.Wet,
            //duration = 10f,
            duration = baseWetDuration + ballShooter.bonusWetDuration,
            value = 0f
        };

        target.TakeStatusEffect(wet);

        // 범람
        SpreadWet(target, wet);
    }

    private void ApplyLand(IDamageable target)
    {
        float value = 1f;

        // 압쇄 : 현재 체력의 n%만큼 추가 피해를 배율로 환산
        if (ballShooter.pulverizationBonus > 0f)
        {
            value +=
            (target.CurrentHP * ballShooter.pulverizationBonus) / damage;
        }

        // 적의 방어력을 일부 무시하는 공격
        StatusEffectData ignoreDefense = new StatusEffectData
        {
            effectType = StatusEffectType.IgnoreDefense,
            duration = 0f,
            //value = 1f,             // 추가 피해 정도
            value = value,             // 추가 피해 정도
            baseDamage = damage     // 원본 대미지
        };

        target.TakeStatusEffect(ignoreDefense);
    }

    private void ApplyElectric(IDamageable target)
    {
        /*MonoBehaviour targetObject = target as MonoBehaviour;

        if (targetObject == null)
            return;

        Vector3 center = targetObject.transform.position;

        // 전이 범위
        float chainRange = 0.31f;

        // 전이 피해량 (원본 데미지의 30%)
        float chainDamage = Mathf.Ceil(damage * 0.3f);

        // 몬스터 레이어만 탐색
        Collider[] hits = Physics.OverlapSphere(
            center,
            chainRange,
            LayerMask.GetMask("Monster"));

        foreach (Collider hit in hits)
        {
            // 자기 자신 제외
            if (hit.gameObject == targetObject.gameObject)
                continue;

            IDamageable other = hit.GetComponent<IDamageable>();

            if (other == null)
                continue;

            // 방향 벡터
            Vector3 dir = hit.transform.position - center;

            // Y축 제거 (2D 평면 느낌으로)
            dir.y = 0f;

            dir.Normalize();

            // 상하좌우 판정
            float horizontal = Mathf.Abs(Vector3.Dot(dir, Vector3.right));
            float vertical = Mathf.Abs(Vector3.Dot(dir, Vector3.forward));

            // 축 방향에 충분히 가까운 경우만 허용
            bool isCrossDirection =
                horizontal > 0.9f ||
                vertical > 0.9f;

            if (!isCrossDirection)
                continue;

            // 전이 데미지 적용
            other.TakeDamage(chainDamage);
            //other.TakeStatusEffect(chainLightning);

            Debug.Log($"{hit.name} 에게 번개 전이 피해 : {chainDamage}");
        }*/
        // =>  다른 함수들 처럼 MonsterBase 에서 처리하도록 수정
        
        if(ballShooter.thunderburst != false)
        {
            baseChainValue = 1f;
        }

        StatusEffectData chainLightning = new StatusEffectData
        {
            effectType = StatusEffectType.ChainLightning,
            //duration = 0f,    // 지속시간 대신 전이 범위로 재활용
            duration = baseChainRange * (1 + ballShooter.chainRangeBonus),
            //value = 0.3f,   // 전이 대미지
            value = baseChainValue + ballShooter.chainBonusDamage,   // 전이 대미지
            baseDamage = damage
        };

        // 초고압 상태면 전이 2회 하도록
        if (ballShooter.highVoltage)
        {
            StartCoroutine(HighVoltageCoroutine(target, chainLightning));
        }
        else
        {
            target.TakeStatusEffect(chainLightning);
        }
    }

    private void ApplyPierce(IDamageable target)
    {
        float pierceValue =
        basePierceValue + ballShooter.pierceBonusDamage;

        // 반사 배율 적용 시
        if (bounceCount > 0)
        {
            pierceValue +=
                bounceCount * ballShooter.bouncePierceBonusDamage;
        }

        float pierceRange = 0;
        if (ballShooter.typhoon)
        {
            pierceRange = basePierceRange * 8;
        }
        else
        {
            pierceRange = basePierceRange * (1 + ballShooter.pierceRangeBonus);
        }

        StatusEffectData pierce = new StatusEffectData
        {
            effectType = StatusEffectType.Pierce,
            //duration = 0f,    관통 범위로 재활용
            //duration = basePierceRange * (1 + ballShooter.pierceRangeBonus),
            duration = pierceRange,
            //value = 1f,   // 100% 관통 대미지
            value = pierceValue,
            baseDamage = damage,

            // 위로 날아가는지 여부만 전달
            directionValue = direction
        };

        target.TakeStatusEffect(pierce);
    }

    /// <summary>
    /// 범람 카드 효과 : 다른 몬스터에게 젖음 전파
    /// </summary>
    private void SpreadWet(IDamageable target, StatusEffectData wet)
    {
        // 범람 카드가 없으면 종료
        if (ballShooter.wetSpreadCount <= 0)
            return;

        MonsterBase centerMonster = target as MonsterBase;

        if (centerMonster == null)
            return;

        // 활성화된 몬스터 리스트 복사
        List<MonsterBase> monsters =
            new List<MonsterBase>(MonsterSpawner.Instance.GetActiveMonsters());

        // 맞은 몬스터 제외
        monsters.Remove(centerMonster);

        // 비활성 또는 null 제거
        monsters.RemoveAll(monster =>
            monster == null || !monster.gameObject.activeSelf);

        /*int spreadCount =
            Mathf.Min(ballShooter.wetSpreadCount, monsters.Count);*/

        int spreadCount;

        // 해일이 있다면 모든 몬스터에게 전파
        if (ballShooter.tsunami)
        {
            spreadCount = monsters.Count;
        }
        // 없다면 기존 범람 개수만큼만 전파
        else
        {
            spreadCount =
                Mathf.Min(ballShooter.wetSpreadCount, monsters.Count);
        }

        for (int i = 0; i < spreadCount; i++)
        {
            // 랜덤 대상 선택
            int randomIndex = Random.Range(0, monsters.Count);

            MonsterBase monster = monsters[randomIndex];

            monster.TakeStatusEffect(wet);

            // 같은 몬스터가 또 선택되지 않도록 제거
            monsters.RemoveAt(randomIndex);
        }
    }

    /// <summary>
    /// 지진 카드 효과 : 좌우 적에게 피해의 50%(최대 2중첩)
    /// </summary>
    /// <param name="target"></param>
    /// <param name="damage"></param>
    private void ApplyEarthquake(
    IDamageable target,
    float damage)
    {
        // 지진 카드가 없으면 종료
        if (ballShooter.earthquake <= 0f)
            return;

        MonsterBase centerMonster = target as MonsterBase;

        if (centerMonster == null)
            return;

        float earthquakeDamage =
            Mathf.Ceil(damage * ballShooter.earthquake);

        Vector3 center = centerMonster.transform.position;

        Collider[] hits = Physics.OverlapSphere(
            center,
            0.35f,
            LayerMask.GetMask("Monster"));

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == centerMonster.gameObject)
                continue;

            Vector3 dir =
                hit.transform.position - center;

            dir.y = 0f;
            dir.Normalize();

            // 좌우만 허용
            if (Mathf.Abs(Vector3.Dot(dir, Vector3.right)) < 0.9f)
                continue;

            IDamageable other =
                hit.GetComponent<IDamageable>();

            if (other == null)
                continue;

            other.TakeDamage(earthquakeDamage, true);   // 땅 속성이므로 배리어 완전 무시
        }
    }

    /// <summary>
    /// 초고압 : 0.2초 간격으로 전이를 2회 발생
    /// </summary>
    private IEnumerator HighVoltageCoroutine(IDamageable target, StatusEffectData chainLightning)
    {
        target.TakeStatusEffect(chainLightning);

        yield return new WaitForSeconds(0.2f);

        // 첫 번째 전이로 죽었을 수도 있으므로 체크
        if (target != null)
        {
            target.TakeStatusEffect(chainLightning);
        }
    }
}