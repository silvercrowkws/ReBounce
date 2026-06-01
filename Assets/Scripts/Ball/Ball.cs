using System;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.XR;

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
    /// 이 공의 데미지
    /// </summary>
    public float damage = 10;

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

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;      // 빠른 속도의 물체가 콜라이더를 뚫는 것 방지

        int ballLayer = LayerMask.NameToLayer("Ball");

        Physics.IgnoreLayerCollision(ballLayer, ballLayer, true);

        meshRenderer = GetComponent<MeshRenderer>();
    }

    protected override void OnEnable()
    {
        ResetBall();
        //ResetBallElementals();
    }

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
            
            gameObject.SetActive(false);
            return;
        }

        // Ball이 아닌 대상과 충돌하면
        if (!collision.gameObject.CompareTag("Ball"))
        {
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
    }

    public void SetElemental(BallElementals elemental)
    {
        ballElementals = elemental;
        ResetBallElementals();
    }

    private void ResetBallElementals()
    {
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
                break;
            
            case BallElementals.Land:
                meshRenderer.material.color = landBall;
                break;
            
            case BallElementals.Electric:
                meshRenderer.material.color = electricBall;
                break;
            
            case BallElementals.Wind:
                meshRenderer.material.color = windcBall;
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
        float multipliedDamage =  damage * elementalTable[(int)ballElementals, (int)monsterElement];

        // 효과 적용 함수 실행
        ApplyElementalEffect(damageable);

        // 최종적으로 계산된 데미지 적용
        damageable.TakeDamage(multipliedDamage);
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
        StatusEffectData burn = new StatusEffectData
        {
            effectType = StatusEffectType.Burn,
            duration = 5f,
            value = 5f
        };

        target.TakeStatusEffect(burn);
    }

    private void ApplyWet(IDamageable target)
    {
        StatusEffectData wet = new StatusEffectData
        {
            effectType = StatusEffectType.Wet,
            duration = 10f,
            value = 0f
        };

        target.TakeStatusEffect(wet);
    }

    private void ApplyLand(IDamageable target)
    {
        // 적의 방어력을 일부 무시하는 공격
        StatusEffectData ignoreDefense = new StatusEffectData
        {
            effectType = StatusEffectType.IgnoreDefense,
            duration = 0f,
            value = 1f,             // 추가 피해 정도
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

        StatusEffectData chainLightning = new StatusEffectData
        {
            effectType = StatusEffectType.ChainLightning,
            duration = 0f,
            value = 0.3f,   // 전이 대미지
            baseDamage = damage
        };

        target.TakeStatusEffect(chainLightning);
    }

    private void ApplyPierce(IDamageable target)
    {
        StatusEffectData pierce = new StatusEffectData
        {
            effectType = StatusEffectType.Pierce,
            duration = 0f,
            value = 1f,   // 100% 관통 대미지
            baseDamage = damage,

            // 위로 날아가는지 여부만 전달
            directionValue = direction
        };

        target.TakeStatusEffect(pierce);
    }
}