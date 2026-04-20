using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed = 10f;

    private Vector3 direction;

    private static bool isFirstGroundHit = false;
    private static Vector3 firstGroundHitPos;

    public void Init(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        Vector3 move = direction * speed * Time.deltaTime;
        transform.position += move;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 바닥 체크
        if (collision.gameObject.CompareTag("DownBrick"))
        {
            HandleGroundHit();
            return;
        }

        // 반사 처리
        Vector3 normal = collision.contacts[0].normal;

        // 반사 공식
        direction = Vector3.Reflect(direction, normal);
    }

    void HandleGroundHit()
    {
        if (!isFirstGroundHit)
        {
            isFirstGroundHit = true;
            firstGroundHitPos = transform.position;

            Debug.Log("첫 바닥 충돌 위치: " + firstGroundHitPos);
        }

        // 더 이상 안 튕기게
        direction = Vector3.zero;
    }
}