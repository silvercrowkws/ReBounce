using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallRecallUI : MonoBehaviour
{
    private Button recallButton;

    [Tooltip("이 시간(초) 동안 어떤 공도 Damageable을 못 맞추면 버튼 표시")]
    [SerializeField] private float stuckThreshold = 8f;

    private void Awake()
    {
        recallButton = GetComponent<Button>();

        recallButton.onClick.AddListener(OnRecallButtonClicked);
        recallButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        // 활성화된 공이 하나도 없으면 판단할 필요 없음
        bool anyBallActive = Ball.ActiveBalls.Count > 0;

        bool stuck =
            anyBallActive &&
            Ball.TimeSinceLastDamageableHit >= stuckThreshold;

        recallButton.gameObject.SetActive(stuck);
    }

    private void OnRecallButtonClicked()
    {
        // ForceRecall 도중 OnDisable로 원본 리스트가 바뀌므로 복사본으로 순회
        List<Ball> snapshot = new List<Ball>(Ball.ActiveBalls);

        foreach (Ball ball in snapshot)
        {
            if (ball == null || !ball.gameObject.activeSelf)
                continue;

            ball.ForceRecall();
        }

        recallButton.gameObject.SetActive(false);
    }
}