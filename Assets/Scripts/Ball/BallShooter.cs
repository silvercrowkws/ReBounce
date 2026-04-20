using UnityEngine;
using UnityEngine.InputSystem;

public class BallShooter : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform firePoint;

    private Vector2 startPos;
    private Vector2 endPos;

    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
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
        Debug.Log("Camera.main: " + Camera.main);
        Debug.Log("firePoint: " + firePoint);
        Debug.Log("ballPrefab: " + ballPrefab);

        Ray ray = Camera.main.ScreenPointToRay(endPos);

        Plane plane = new Plane(Vector3.up, firePoint.position);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);

            Vector3 dir = (hitPoint - firePoint.position);
            dir.y = 0;

            GameObject ballObj = Instantiate(ballPrefab, firePoint.position, Quaternion.identity);
            var ball = ballObj.GetComponent<Ball>();
            Debug.Log("ball component: " + ball);

            ball.Init(dir);
        }
    }
}