using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
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

    private void Awake()
    {
        monsterElementMaterials =
            new Material[Enum.GetValues(typeof(MonsterElementals)).Length];
    }

    private void Start()
    {
        StartCoroutine(LoadMonsterMaterials());

        turnManager = TurnManager.Instance;
        if (turnManager != null)
        {
            turnManager.OnTurnInitialize();        // 턴 초기화
        }
        else
        {
            Debug.LogError("턴 매니저를 못찾는다고?");
        }
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
}
