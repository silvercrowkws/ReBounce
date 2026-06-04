using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : Singleton<TurnManager>
{
    /// <summary>
    /// 현재 턴 진행상황 표시용 enum
    /// </summary>
    enum TurnProcessState
    {
        Idle = 0,
        Start,
        End,
    }

    /// <summary>
    /// 현재 턴 진행상황
    /// </summary>
    TurnProcessState turnState = TurnProcessState.Idle;

    /// <summary>
    /// 현재 턴 번호(몇번째 턴인지)
    /// </summary>
    public int turnNumber = 0;

    /// <summary>
    /// 턴이 진행될지 여부(true면 턴이 진행되고 false면 턴이 진행되지 않는다)
    /// </summary>
    bool isTurnEnable = true;

    /// <summary>
    /// 턴이 시작되었음을 알리는 델리게이트(int:시작된 턴 번호)
    /// </summary>
    public Action<int> onTurnStart;

    /// <summary>
    /// OnTurnInitialize가 실행되었음을 알리는 델리게이트
    /// </summary>
    public Action onTurnInitializeStart;

    /// <summary>
    /// 턴이 끝났음을 알리는 델리게이트
    /// </summary>
    public Action onTurnEnd;

    /// <summary>
    /// 턴 종료 처리 중인지 확인하는 변수
    /// </summary>
    bool isEndProcess = false;

    /// <summary>
    /// 턴 종료 웨이브
    /// </summary>
    //public int endTurnNumber = 20;

    /// <summary>
    /// 마지막 턴이 끝났음을 알리는 델리게이트(UI 갱신용)
    /// </summary>
    //public Action<int> onTurnOver;

    /// <summary>
    /// 게임 매니저
    /// </summary>
    GameManager gameManager;

    /// <summary>
    /// 활성화된 공의 개수를 카운팅하는 변수
    /// </summary>
    public int ActiveBallCount { get; private set; }

    /// <summary>
    /// 공을 발사 할 수 있는지 확인하는 변수(true : 발사 가능, false : 발사 불가능)
    /// </summary>
    bool isShotInProgress = false;

    private void Start()
    {
        gameManager = GameManager.Instance;
        turnNumber = 0;                         // OnTurnStart에서 turnNumber를 증가 시키기 때문에 0에서 시작
    }

    /// <summary>
    /// 씬이 시작될 때 초기화
    /// </summary>
    public void OnTurnInitialize()                 // 이 함수 쓸 때 n초 지나는 UI 이후에 시작시켜야 함
    {
        /*if (turnNumber == 0)
        {
            turnNumber = 1;                     // 초기화시 0부터 시작하기 때문에
        }*/
        //turnNumber = 0;                         // OnTurnStart에서 turnNumber를 증가 시키기 때문에 0에서 시작        

        //turnState = TurnProcessState.Idle;      // 턴 진행 상태 초기화
        isTurnEnable = true;                    // 턴 켜기

        Debug.Log("턴 시작 준비 완료");

        onTurnInitializeStart?.Invoke();
        OnTurnStart();                          // 턴 시작
    }


    /// <summary>
    /// 턴 시작 처리용 함수
    /// </summary>
    public void OnTurnStart()
    {
        if (isTurnEnable)                           // 턴 매니저가 작동 중이면
        {
            turnNumber++;                           // 턴 숫자 증가
            Debug.Log($"{turnNumber}턴 시작");
            //turnState = TurnProcessState.Start;     // 턴 시작 상태

            isShotInProgress = false;               // 아직 발사 안함

            //Debug.Log("onTurnStart 델리게이트 보냄");
            onTurnStart?.Invoke(turnNumber);        // 턴이 시작되었음을 알림
        }
    }

    /// <summary>
    /// 턴 종료 처리용 함수
    /// </summary>
    public void OnTurnEnd()
    {
        if (turnNumber == 0)
        {
            turnNumber = 1;     // 가끔 현재 턴이 0인 상태가 있음
            Debug.Log("턴 꼬였음");
        }
        Debug.Log("OnTurnEnd 호출");

        if (isTurnEnable)                   // 턴 매니저가 작동 중이면
        {
            isEndProcess = true;            // 종료 처리 중이라고 표시
            onTurnEnd?.Invoke();            // 턴이 종료되었다고 알림
            Debug.Log($"{turnNumber}턴 종료");

            isShotInProgress = false;        // 공 발사 불가능

            isEndProcess = false;           // 종료 처리가 끝났다고 표시
            OnTurnStart();                  // 다음 턴 시작
        }
    }

    /// <summary>
    /// 플레이어가 발사를 시작하면 처리
    /// </summary>
    public void StartShot()
    {
        ActiveBallCount = 0;
        isShotInProgress = true;
    }

    public void RegisterBall()
    {
        if (isShotInProgress)
        {
            ActiveBallCount++;
            Debug.Log($"공 누적 : {ActiveBallCount}");
        }
    }

    public void UnregisterBall()
    {
        ActiveBallCount--;

        Debug.Log($"공 제거 : {ActiveBallCount}");
        Debug.Log($"isShotInProgress : {isShotInProgress}");

        if (isShotInProgress && ActiveBallCount <= 0)
        {
            Debug.Log("턴 종료 조건 만족");

            isShotInProgress = false;
            OnTurnEnd();
        }
    }

    /*/// <summary>
    /// OnTurnStart를 사용하기 위한 public 함수
    /// </summary>
    public void OnTurnStart2()
    {
        OnTurnStart();
    }

    /// <summary>
    /// OnTurnEnd를 사용하기 위한 public 함수
    /// </summary>
    public void OnTurnEnd2()
    {
        OnTurnEnd();
    }*/
}
