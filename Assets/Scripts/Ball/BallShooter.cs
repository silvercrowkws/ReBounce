using UnityEngine;
using UnityEngine.InputSystem;

public class BallShooter : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform firePoint;

    private Vector2 startPos;
    private Vector2 endPos;

    private PlayerInputActions inputActions;

    Factory factory;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        factory = Factory.Instance;
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

        inputActions.Actions.Disable();
    }

    private void OnPressStarted(InputAction.CallbackContext context)
    {
        startPos = inputActions.Actions.Touch.ReadValue<Vector2>();
    }

    private void OnPressCanceled(InputAction.CallbackContext context)
    {
        endPos = inputActions.Actions.Touch.ReadValue<Vector2>();
        Shoot();
    }

    void Shoot()
    {
        //Debug.Log("Camera.main: " + Camera.main);
        //Debug.Log("firePoint: " + firePoint);
        //Debug.Log("ballPrefab: " + ballPrefab);

        if (GameManager.Instance == null)
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
        Vector3 startPos = new Vector3(GameManager.Instance.firstGroundHitPos.x, GameManager.Instance.firstGroundHitPos.y, -1.3f);
        firePoint.position = startPos;
        //Debug.Log(startPos);

        // 👉 라운드 시작 시 상태 초기화
        GameManager.Instance.ResetRound();

        Ray ray = Camera.main.ScreenPointToRay(endPos);

        Plane plane = new Plane(Vector3.up, firePoint.position);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);

            Vector3 dir = (hitPoint - firePoint.position);
            dir.y = 0;

            // 여기부터 공 발사 부분
            /*GameObject ballObj = Instantiate(ballPrefab, firePoint.position, Quaternion.identity);
            var ball = ballObj.GetComponent<Ball>();*/
            //Debug.Log("ball component: " + ball);

            Ball ball = factory.GetBall(firePoint.position, 0f);

            if (ball == null)
            {
                Debug.LogError("Factory returned null Ball");
                return;
            }

            ball.Init(dir);

            /*Debug.Log("공 발사 실행");
            factory.GetBall(firePoint.position, 0f);*/
        }
    }
}