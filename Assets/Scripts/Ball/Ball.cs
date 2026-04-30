using Unity.VisualScripting;
using UnityEngine;

public enum BallElementals
{
    Normal = 0,     // 기본
    Fire,           // 불            =>  화상을 남겨 지속피해를 n초동안 준다거나
    Water,          // 물            =>  피격 시 '젖음' 디버프 부여
    Earth,          // 흙, 땅?       =>  방어를 무시하는 고정피해?
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
    public BallElementals elementals = BallElementals.Normal;

    /// <summary>
    /// 공의 속도
    /// </summary>
    public float speed = 10f;

    /// <summary>
    /// 이 공의 데미지
    /// </summary>
    public float damage;

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

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;      // 빠른 속도의 물체가 콜라이더를 뚫는 것 방지

        int ballLayer = LayerMask.NameToLayer("Ball");

        Physics.IgnoreLayerCollision(ballLayer, ballLayer, true);
    }

    protected override void OnEnable()
    {
        ResetBall();
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
                CalculateDamage(damageable);
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

    public void ResetBall()
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

    /// <summary>
    /// 데미지 계산 함수
    /// </summary>
    private void CalculateDamage(IDamageable damageable)
    {
        // 충돌한 대상에게 damageable 인터페이스가 있으면 CalculateDamage 이 함수가 실행되는건데,
        // 일단 공의 속성은 쉽게 알 수 있고,
        // 충돌한 대상의 태그를 받아와?

        // 데미지 적용 함수
        damageable.TakeDamage(damage);
    }
}