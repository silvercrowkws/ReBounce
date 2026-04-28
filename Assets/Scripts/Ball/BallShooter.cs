using System;
using System.Collections;
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
    public float maxAngle = 75f; // 좌우 최대 각도 (도 단위)

    /// <summary>
    /// 발사 중인지 체크하는 변수
    /// </summary>
    private bool isShooting = false;

    /// <summary>
    /// 발사 횟수
    /// </summary>
    public int shootCount;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        gameManager = GameManager.Instance;
        gameManager.onFirstGroundHitPos += OnFirstGroundHitPos;
        factory = Factory.Instance;

        // 시작할 때는 가이드라인을 숨김
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null) lineRenderer.enabled = false;
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
            StartCoroutine(ShootCoroutine(dir));

            /*Debug.Log("공 발사 실행");
            factory.GetBall(firePoint.position, 0f);*/
        }
    }

    IEnumerator ShootCoroutine(Vector3 dir)
    {
        isShooting = true;      // 발사 시작
        for (int i = 0; i < shootCount; i++)
        {
            Ball ball = factory.GetBall(firePoint.position, 0f);
            if (ball != null) ball.Init(dir);
            yield return new WaitForSeconds(0.1f);
        }
        isShooting = false;     // 발사 종료
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
}