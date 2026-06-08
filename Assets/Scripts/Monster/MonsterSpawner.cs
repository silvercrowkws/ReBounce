using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : Singleton<MonsterSpawner>
{
    /*private readonly List<MonsterBase> activeMonsters
        = new List<MonsterBase>();*/

    public List<MonsterBase> activeMonsters
        = new List<MonsterBase>();

    // 가능한 스폰 위치
    private readonly float[] spawnXPositions =
    {
        -0.93f,
        -0.62f,
        -0.31f,
         0f,
         0.31f,
         0.62f,
         0.93f
    };

    TurnManager turnManager;
    
    private void Start()
    {
        turnManager = TurnManager.Instance;
        turnManager.onTurnEnd += OnTurnEnd;

        SpawnMonsters(); // 첫 웨이브
    }

    private void OnDestroy()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.onTurnEnd -= OnTurnEnd;
    }

    private void OnTurnEnd()
    {
        MoveMonstersDown();
        SpawnMonsters();
    }

    private void MoveMonstersDown()
    {
        Debug.Log($"{name} 아래로 이동");

        /*foreach (MonsterBase monster in activeMonsters)
        {
            if (monster == null)
                continue;

            if (!monster.gameObject.activeSelf)
                continue;

            monster.transform.position +=
                new Vector3(0f, 0f, -0.31f);
        }*/

        for (int i = activeMonsters.Count - 1; i >= 0; i--)
        {
            MonsterBase monster = activeMonsters[i];

            if (monster == null || !monster.gameObject.activeSelf)
            {
                activeMonsters.RemoveAt(i);
                continue;
            }

            Vector3 pos = monster.transform.position;
            pos.z -= 0.31f;
            monster.transform.position = pos;
        }
    }

    public void RegisterMonster(MonsterBase monster)
    {
        if (!activeMonsters.Contains(monster))
        {
            activeMonsters.Add(monster);
        }
    }

    public void UnregisterMonster(MonsterBase monster)
    {
        activeMonsters.Remove(monster);
    }

    public List<MonsterBase> GetActiveMonsters()
    {
        return activeMonsters;
    }

    /// <summary>
    /// 턴 시작 시 몬스터 스폰
    /// </summary>
    public void SpawnMonsters()
    {
        Debug.Log("몬스터 스폰");

        //int spawnCount = Random.Range(4, 7); // 4 ~ 6마리
        int spawnCount = GetSpawnCount();

        List<float> availablePositions = new List<float>(spawnXPositions);

        for (int i = 0; i < spawnCount; i++)
        {
            if (availablePositions.Count <= 0)
                break;

            int randomIndex = Random.Range(0, availablePositions.Count);

            float spawnX = availablePositions[randomIndex];

            availablePositions.RemoveAt(randomIndex);

            SpawnMonsterAt(spawnX);
        }
    }

    private void SpawnMonsterAt(float xPos)
    {
        MonsterBase monster = Factory.Instance.GetMonster_Goblin_Green();

        if (monster == null)
            return;

        Vector3 spawnPos = new Vector3(xPos, 0.033f, 1.24f);

        monster.transform.position = spawnPos;
        monster.gameObject.SetActive(true);

        RegisterMonster(monster);
    }

    /// <summary>
    /// 턴 마다 스폰되는 몬스터의 수를 조절하는 함수
    /// </summary>
    /// <returns></returns>
    private int GetSpawnCount()
    {
        // 델리게이트로 받는게 몬스터를 스폰하는 시점보다 느려서 +1 처리
        int turn = TurnManager.Instance.turnNumber + 1;

        if (turn <= 9)
            return (turn + 1) / 2;

        if (turn <= 20)
            return Random.Range(5, 8); // 5~7

        return 7;
    }
}
