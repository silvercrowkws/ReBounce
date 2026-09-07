using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine;


/// <summary>
/// 게임 상태
/// </summary>
public enum GameState
{
    Lobby = 0,
    GameStart,
    GameOver,
}

public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// 현재 게임상태
    /// </summary>
    public GameState gameState = GameState.Lobby;


    /// <summary>
    /// 현재 게임상태 변경시 알리는 프로퍼티
    /// </summary>
    public GameState GameState
    {
        get => gameState;
        set
        {
            if (gameState != value)
            {
                gameState = value;
                switch (gameState)
                {
                    case GameState.Lobby:
                        Debug.Log("로비");
                        break;
                    case GameState.GameStart:
                        Debug.Log("게임 시작");
                        onGameStart?.Invoke();
                        break;
                    case GameState.GameOver:
                        Debug.Log("게임 종료");
                        onGameEnd?.Invoke();
                        break;
                }
            }
        }
    }


    // 게임상태 델리게이트
    public Action onGameStart;
    public Action onGameEnd;

    public Vector3 firstGroundHitPos { get; private set; }
    public bool hasFirstGroundHit { get; private set; }

    /// <summary>
    /// 첫번째로 땅에 닿은 위치를 알리는 델리게이트
    /// </summary>
    public Action<Vector3> onFirstGroundHitPos;

    /// <summary>
    /// 몬스터 속성 머터리얼의 배열
    /// </summary>
    public Material[] monsterElementMaterials { get; private set; }

    /// <summary>
    /// Material 로드 완료 델리게이트
    /// </summary>
    public Action onMaterialLoaded;

    /// <summary>
    /// 머터리얼 로드 완료 여부 플래그
    /// </summary>
    public bool IsMaterialLoaded { get; private set; }

    /// <summary>
    /// 턴 매니저
    /// </summary>
    TurnManager turnManager;

    /// <summary>
    /// 게임 오버(true: 게임 오버, false : 게임 진행 중)
    /// </summary>
    private bool isGameOver = false;

    /// <summary>
    /// 게임 오버 프로퍼티
    /// </summary>
    public bool IsGameOver
    {
        get => isGameOver;
        set
        {
            if (isGameOver == value)
                return; // 값이 같으면 아무것도 안 함

            isGameOver = value;

            if (isGameOver)
            {
                GameOver();
            }
        }
    }

    /// <summary>
    /// 현재 씬 번호
    /// </summary>
    public int sceneNumber;

    private void Awake()
    {
        monsterElementMaterials =
            new Material[Enum.GetValues(typeof(MonsterElementals)).Length];
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        StartCoroutine(LoadMonsterMaterials());

        /*// 게임 시작 씬으로 이동하면 실행으로 수정
        turnManager = TurnManager.Instance;
        if (turnManager != null)
        {
            turnManager.OnTurnInitialize();        // 턴 초기화
        }
        else
        {
            Debug.LogError("턴 매니저를 못찾는다고?");
        }*/
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private IEnumerator LoadMonsterMaterials()
    {
        for (int i = 0; i < monsterElementMaterials.Length; i++)
        {
            string address =
                $"{i}_{(MonsterElementals)i}";

            AsyncOperationHandle<Material> handle =
                Addressables.LoadAssetAsync<Material>(address);

            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                monsterElementMaterials[i] = handle.Result;

                Debug.Log($"Material 로드 성공 : {address}");
            }
            else
            {
                Debug.LogError($"Material 로드 실패 : {address}");
            }
        }

        IsMaterialLoaded = true;

        onMaterialLoaded?.Invoke();

        Debug.Log("모든 Material 로드 완료");
    }

    public Material GetMonsterMaterial(MonsterElementals element)
    {
        return monsterElementMaterials[(int)element];
    }

    public void ResetRound()
    {
        hasFirstGroundHit = false;
        firstGroundHitPos = Vector3.zero;
    }

    public void RegisterFirstGroundHit(Vector3 pos)
    {
        if (hasFirstGroundHit) return;

        hasFirstGroundHit = true;
        firstGroundHitPos = pos;

        Debug.Log("첫 바닥 충돌 위치: " + firstGroundHitPos);
        onFirstGroundHitPos?.Invoke(firstGroundHitPos);
    }

    /// <summary>
    /// 게임 오버 처리 함수
    /// </summary>
    private void GameOver()
    {
        gameState = GameState.GameOver;
        Debug.LogError("게임 오버");
        // UI 처리하면서 재시작 같은 기능 추가해야 함
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        Debug.Log($"현재 씬 이름 : {scene.name}");
        Debug.Log($"현재 씬 Build Index : {scene.buildIndex}");

        switch (scene.buildIndex)
        {
            case 0:
                Debug.Log("로비 씬");
                gameState = GameState.Lobby;
                sceneNumber = scene.buildIndex;
                break;

            case 1:
                Debug.Log("게임 시작 씬");
                gameState = GameState.GameStart;

                StartCoroutine(YieldTurnManager());

                break;
        }
    }

    private IEnumerator YieldTurnManager()
    {
        turnManager = TurnManager.Instance;

        // 턴 매니저 Start 완료를 기다림
        while (!turnManager.turnManagerReady)
        {
            yield return null;
        }

        //turnManager = TurnManager.Instance;
        if (turnManager != null)
        {
            turnManager.turnManagerReady = false;
            turnManager.OnTurnInitialize();        // 턴 초기화
        }
        else
        {
            Debug.LogError("턴 매니저를 못찾는다고?");
        }
    }

    /// <summary>
    /// 씬 이동 함수
    /// </summary>
    /// <param name="number"></param>
    public void LoadScene(int number)
    {
        SceneManager.LoadScene(number);
    }
}
