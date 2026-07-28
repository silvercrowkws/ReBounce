using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 이 스크립트를 어떤 게임 오브젝트에 붙이면, 그 오브젝트에
// CanvasGroup 컴포넌트가 없을 경우 자동으로 같이 추가해줌
[RequireComponent(typeof(CanvasGroup))]
public class BallRecallUI : MonoBehaviour
{
    /// <summary>
    /// 회수 버튼의 클릭 이벤트 처리용 버튼 컴포넌트
    /// </summary>
    private Button recallButton;

    /// <summary>
    /// 버튼의 표시/클릭 가능 여부를 제어하는 캔버스 그룹
    /// </summary>
    private CanvasGroup canvasGroup;

    /// <summary>
    /// 볼 슈터
    /// </summary>
    private BallShooter ballShooter;


    [Tooltip("이 시간(초) 동안 어떤 공도 Damageable을 못 맞추면 버튼 표시")]
    [SerializeField] private float stuckThreshold = 8f;

    /// <summary>
    /// 현재 버튼이 보여지고 있는 상태인지 여부(중복 갱신 방지용)
    /// </summary>
    private bool isVisible = false;

    private void Awake()
    {
        recallButton = GetComponent<Button>();
        canvasGroup = GetComponent<CanvasGroup>();

        recallButton.onClick.AddListener(OnRecallButtonClicked);

        SetVisible(false);
    }

    private void Start()
    {
        ballShooter = FindAnyObjectByType<BallShooter>();
    }

    private void Update()
    {
        // 아직 모든 공이 발사된 상태가 아니면 판단하지 않음
        if (ballShooter.IsShooting)
        {
            if (isVisible)
                SetVisible(false);

            return;
        }

        bool anyBallActive = Ball.ActiveBalls.Count > 0;

        bool stuck =
            anyBallActive &&
            Ball.TimeSinceLastDamageableHit >= stuckThreshold;

        if (stuck != isVisible)
        {
            SetVisible(stuck);
        }
    }

    /// <summary>
    /// 캔버스 그룹을 이용해 버튼의 표시/클릭 가능 여부를 갱신하는 함수
    /// </summary>
    /// <param name="visible">true : 보이고 클릭 가능, false : 숨기고 클릭 불가</param>
    private void SetVisible(bool visible)
    {
        isVisible = visible;

        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }

    /// <summary>
    /// 회수 버튼 클릭 시, 현재 활성화된 모든 공을 강제 회수 처리
    /// </summary>
    private void OnRecallButtonClicked()
    {
        Debug.Log("회수 버튼 클릭");

        // ForceRecall 도중 OnDisable로 원본 리스트가 바뀌므로 복사본으로 순회
        List<Ball> snapshot = new List<Ball>(Ball.ActiveBalls);

        foreach (Ball ball in snapshot)
        {
            if (ball == null || !ball.gameObject.activeSelf)
                continue;

            ball.ForceRecall();
        }

        SetVisible(false);
    }
}