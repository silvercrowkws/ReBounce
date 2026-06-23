using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallShooter : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform firePoint;

    private Vector2 startPos;
    private Vector2 endPos;

    private PlayerInputActions inputActions;

    private Vector2 currentPos; // 실시간 마우스/터치 위치
    LineRenderer lineRenderer; // Inspector에서 할당
    public float lineLength = 2f;     // 가이드라인 길이
    bool isPressing = false;

    Factory factory;
    GameManager gameManager;

    [Header("좌우 허용 각도")]
    public float maxAngle = 70f; // 좌우 최대 각도 (도 단위)

    /// <summary>
    /// 발사 중인지 체크하는 변수
    /// </summary>
    private bool isShooting = false;

    /// <summary>
    /// 발사 횟수
    /// </summary>
    private int shootCount;
    public int ShootCount => shootBalls.Count;

    /// <summary>
    /// 현재 발사될 공 목록
    /// 같은 속성이 여러 개 존재할 수 있음
    /// </summary>    
    public List<BallElementals> shootBalls = new List<BallElementals>();

    /// <summary>
    /// 플레이어가 한번이라도 획득하여 해금한 속성 목록
    /// 카드 등장 조건 판정에 사용
    /// </summary>
    public HashSet<BallElementals> unlockedElementals = new();

    /// <summary>
    /// 공의 보너스 대미지
    /// </summary>
    [Header("공의 보너스 대미지")]
    public float bonusDamage = 0;

    /// <summary>
    /// 공의 보너스 화상 대미지
    /// </summary>
    [Header("공의 보너스 화상 대미지")]
    public float bonusBurnDamage = 0;

    /// <summary>
    /// 공의 보너스 화상 시간
    /// </summary>
    [Header("공의 보너스 화상 시간")]
    public float bonusBurnDuration = 0;

    /// <summary>
    /// 점화 카드 보너스
    /// </summary>
    [Header("화상 상태 적에게 추가 피해 배율")]
    public float ignitionBonus = 0;

    /// <summary>
    /// 물 공의 보너스 대미지
    /// </summary>
    [Header("물 공의 보너스 대미지")]
    public float waterBonusDamage = 0;

    /// <summary>
    /// 냉각 카드 보너스
    /// </summary>
    [Header("젖음 상태 적에게 추가 피해 배율")]
    public float coolingBounsDamage = 0;

    /// <summary>
    /// 공의 보너즈 젖음 지속시간
    /// </summary>
    [Header("공의 보너스 젖음 지속시간")]
    public float bonusWetDuration = 0;

    /// <summary>
    /// 땅 공의 보너스 대미지
    /// </summary>
    [Header("땅 공의 보너스 대미지")]
    public float landBonusDamage = 0;

    /// <summary>
    /// 압괴 추가 피해 배율
    /// </summary>
    [Header("체력이 50% 미만인 적에게 땅 공 피해 +50%")]
    public float crushBonusDamage;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        gameManager = GameManager.Instance;
        gameManager.onFirstGroundHitPos += OnFirstGroundHitPos;
        factory = Factory.Instance;


        // 시작 시 기본 속성 공(Normal) 보유
        // => 테스트 용으로 턴 매니저에서 턴 시작 시 공 1개씩 지급
        //AddBall(BallElementals.Normal);

        // 기본 공 shootCount개 추가
        for (int i = 0; i < shootCount; i++)
        {
            shootBalls.Add(BallElementals.Normal);
        }

        // 첫 번째 공부터 속성 변경
        //shootBalls[0] = BallElementals.Fire;
        //shootBalls[1] = BallElementals.Electric;


        // 시작할 때는 가이드라인을 숨김
        lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer != null)
        {
            // 두께 설정
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;

            lineRenderer.enabled = false;
        }
    }

    void OnEnable()
    {
        inputActions.Actions.Enable();

        inputActions.Actions.Press.started += OnPressStarted;
        inputActions.Actions.Press.canceled += OnPressCanceled;
    }

    private void OnDisable()
    {
        inputActions.Actions.Press.started -= OnPressStarted;
        inputActions.Actions.Press.canceled -= OnPressCanceled;

        gameManager.onFirstGroundHitPos -= OnFirstGroundHitPos;

        inputActions.Actions.Disable();
    }

    private void Update()
    {
        // 누르고 있는 동안 실시간으로 가이드라인 업데이트
        if (isPressing)
        {
            DrawGuideLine();
        }
    }

    private void OnPressStarted(InputAction.CallbackContext context)
    {
        if (!CardManager.Instance.IsCardSelected)
            return;

        if (TurnManager.Instance.IsShotInProgress)
            return;

        isPressing = true;

        startPos = inputActions.Actions.Touch.ReadValue<Vector2>();

        if (lineRenderer != null) lineRenderer.enabled = true;
    }

    private void OnPressCanceled(InputAction.CallbackContext context)
    {
        isPressing = false;
        if (lineRenderer != null) lineRenderer.enabled = false;

        endPos = inputActions.Actions.Touch.ReadValue<Vector2>();
        Shoot(endPos);
    }

    /// <summary>
    /// 라인렌더러로 궤적을 미리 보여주는 함수
    /// </summary>
    void DrawGuideLine()
    {
        if (lineRenderer == null || Camera.main == null) return;

        currentPos = inputActions.Actions.Touch.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(currentPos);
        Plane plane = new Plane(Vector3.up, firePoint.position);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Vector3 dir = (hitPoint - firePoint.position).normalized;
            dir.y = 0;
            dir = ClampDirection(dir.normalized);       // 각도 제한 추가

            float yOffset = 0.05f;
            Vector3 currentOrigin = firePoint.position + (Vector3.up * yOffset);
            Vector3 currentDir = dir;

            // 💡 전체 가이드라인의 총 길이 제한
            float remainingLength = lineLength;

            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, currentOrigin);

            // 최대 3번의 선분까지 계산 (시작->벽1, 벽1->벽2, 벽2->끝)
            for (int i = 1; i <= 3; i++)
            {
                if (remainingLength <= 0) break;

                if (Physics.Raycast(currentOrigin, currentDir, out RaycastHit hit, remainingLength))
                {
                    lineRenderer.positionCount = i + 1;
                    lineRenderer.SetPosition(i, hit.point + (Vector3.up * yOffset));

                    if (hit.collider.CompareTag("SideBrick"))
                    {
                        // 💡 남은 거리에서 방금 이동한 거리를 뺍니다.
                        remainingLength -= hit.distance;

                        // 반사 벡터 계산
                        currentDir = Vector3.Reflect(currentDir, hit.normal);
                        currentDir.y = 0;
                        currentOrigin = hit.point + (Vector3.up * yOffset);
                    }
                    else
                    {
                        // 벽이 아닌 다른 오브젝트에 닿으면 거기서 종료
                        break;
                    }
                }
                else
                {
                    // 아무것도 안 닿으면 남은 길이만큼 마지막 선을 긋고 종료
                    lineRenderer.positionCount = i + 1;
                    lineRenderer.SetPosition(i, currentOrigin + currentDir * remainingLength);
                    remainingLength = 0;
                    break;
                }
            }
        }
    }

    void Shoot(Vector2 endPos)
    {
        if (!CardManager.Instance.IsCardSelected)
            return;

        if (TurnManager.Instance.IsShotInProgress)
            return;

        //Debug.Log("Camera.main: " + Camera.main);
        //Debug.Log("firePoint: " + firePoint);
        //Debug.Log("ballPrefab: " + ballPrefab);

        if (gameManager == null)
        {
            Debug.LogError("GameManager null");
            return;
        }

        if (factory == null)
        {
            Debug.LogError("Factory null");
            return;
        }

        // 발사 위치를 조정
        /*Vector3 shootStartPos = new Vector3(gameManager.firstGroundHitPos.x, gameManager.firstGroundHitPos.y, -1.3f);
        firePoint.position = shootStartPos;*/
        //Debug.Log(startPos);

        // 👉 라운드 시작 시 상태 초기화
        gameManager.ResetRound();

        Ray ray = Camera.main.ScreenPointToRay(endPos);

        Plane plane = new Plane(Vector3.up, firePoint.position);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);

            Vector3 dir = (hitPoint - firePoint.position);
            dir.y = 0;
            dir = ClampDirection(dir.normalized);       // 각도 제한 추가

            // 여기부터 공 발사 부분
            /*GameObject ballObj = Instantiate(ballPrefab, firePoint.position, Quaternion.identity);
            var ball = ballObj.GetComponent<Ball>();*/
            //Debug.Log("ball component: " + ball);


            /*Ball ball = factory.GetBall(firePoint.position, 0f);
            if (ball != null) ball.Init(dir);*/

            
            
            
            TurnManager.Instance.StartShot();       // 발사 가능하다고 하는 함수
            StartCoroutine(ShootCoroutine(dir));

            /*Debug.Log("공 발사 실행");
            factory.GetBall(firePoint.position, 0f);*/
        }
    }

    IEnumerator ShootCoroutine(Vector3 dir)
    {
        /*isShooting = true;      // 발사 시작
        for (int i = 0; i < shootCount; i++)
        {
            Ball ball = factory.GetBall(firePoint.position, 0f);
            if (ball != null) ball.Init(dir);
            yield return new WaitForSeconds(0.1f);
        }
        isShooting = false;     // 발사 종료*/

        isShooting = true;

        for (int i = 0; i < shootBalls.Count; i++)
        {
            BallElementals elemental = shootBalls[i];

            Ball ball = factory.GetBall(firePoint.position, elemental, 0f);

            if (ball != null)
            {
                ball.Init(dir);
            }

            yield return new WaitForSeconds(0.1f);
        }

        isShooting = false;
    }

    private void OnFirstGroundHitPos(Vector3 vector)
    {
        Vector3 shootStartPos = new Vector3(gameManager.firstGroundHitPos.x, gameManager.firstGroundHitPos.y, -1.3f);
        //firePoint.position = shootStartPos;

        StartCoroutine(FirePointChange(shootStartPos));
    }

    IEnumerator FirePointChange(Vector3 shootStartPos)
    {
        // 발사 중인 동안 반복
        while (isShooting)
        {
            yield return null;
        }

        // 발사가 끝났으면
        firePoint.position = shootStartPos;
    }

    /// <summary>
    /// 발사각을 제한하는 함수
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    Vector3 ClampDirection(Vector3 dir)
    {
        // 기준 방향 (예: 전방 z+ 방향)
        Vector3 forward = Vector3.forward;

        // 현재 각도 계산
        float angle = Vector3.SignedAngle(forward, dir, Vector3.up);

        // 각도 제한
        float clampedAngle = Mathf.Clamp(angle, -maxAngle, maxAngle);

        // 제한된 방향 다시 계산
        Quaternion rot = Quaternion.AngleAxis(clampedAngle, Vector3.up);
        return rot * forward;
    }

    /// <summary>
    /// 공 추가 함수
    /// 새 속성이라면 보유 속성 목록에도 등록
    public void AddBall(BallElementals elemental, int count = 1)
    {
        // 실제 발사될 공 추가
        //shootBalls.Add(elemental);

        for (int i = 0; i < count; i++)
        {
            shootBalls.Add(elemental);
        }

        // 해금 처리(HashSet 특성상 중복이면 자동 무시)
        unlockedElementals.Add(elemental);

        // 만약 FireInfusion 카드 같은 것으로 공을 추가할 때는
        // ballShooter.AddBall(BallElementals.Fire); 이런 식으로 사용
    }
}