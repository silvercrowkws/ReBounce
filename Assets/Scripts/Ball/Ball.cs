using Unity.VisualScripting;
using UnityEngine;

/*public enum BallElementals
{
    Normal = 0,     // 기본
    Fire,           // 불
    Water,          // 물
    Wind,           // 바람, 공기
    Electric,       // 전기
    Earth,          // 흙, 땅?
}*/

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
            // 물리 완전 정지
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // 위치 고정 (살짝 밀리는 것 방지)
            rb.isKinematic = true;      // 재활용시 false 필요

            // 논리 이동도 정지
            direction = Vector3.zero;

            sphereCollider.enabled = false;     // 바닥에 닿으면 콜라이더 끄고

            HandleGroundHit();
            return;
        }

        // Ball이 아닌 대상과 충돌하면
        if (!collision.gameObject.CompareTag("Ball"))
        {
            // 반사 처리
            Vector3 normal = collision.contacts[0].normal;

            // 반사 공식
            direction = Vector3.Reflect(direction, normal);
        }
    }

    void HandleGroundHit()
    {
        GameManager.Instance.RegisterFirstGroundHit(transform.position);
    }

    public void ResetBall()
    {
        // 물리 상태 복구
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false;

        // 콜라이더 복구
        sphereCollider.enabled = true;

        // 이동값 초기화
        direction = Vector3.zero; 
        //isFirstGroundHit 는 static 변수라서 OnDisable에서 초기화 하면 안되고.
        // 스테이지 초기화? 내 경우에는 다음 발사 직전에 초기화 하면 될듯
    }
}