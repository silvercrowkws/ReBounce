using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : Singleton<MonsterSpawner>
{
    /*private readonly List<MonsterBase> activeMonsters
        = new List<MonsterBase>();*/

    public List<MonsterBase> activeMonsters
        = new List<MonsterBase>();

    /// <summary>
    /// 가능한 X 스폰 위치
    /// </summary>
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

        bool isGameOverCheck = false;

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

            if (pos.z <= -1.24f)
            {
                isGameOverCheck = true;
            }
        }

        if(isGameOverCheck)
        {
            Debug.Log("게임 오버 체크 확인");
            GameManager.Instance.IsGameOver = true;
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
    /// 턴 시작 시 몬스터 스폰 함수
    /// </summary>
    public void SpawnMonsters()
    {
        Debug.Log("몬스터 스폰");

        int turn = TurnManager.Instance.turnNumber + 1;     // 델리게이트로 받는게 몬스터를 스폰하는 시점보다 느려서 +1 처리

        //int spawnCount = Random.Range(4, 7); // 4 ~ 6마리

        // 이번 턴의 총 몬스터 스폰 수
        int spawnCount = GetSpawnCount();

        // 보스 / 기믹 / 일반 몬스터 수 계산
        int bossCount = GetBossCount(turn);
        int gimmickCount = GetGimmickCount(turn, spawnCount - bossCount);       // 기믹 몬스터 스폰 숫자 결정
        int normalCount = spawnCount - bossCount - gimmickCount;                // 일반 몬스터 스폰 숫자 결정

        List<float> availablePositions = new List<float>(spawnXPositions);

        for (int i = 0; i < spawnCount; i++)
        {
            if (availablePositions.Count <= 0)
                break;

            int randomIndex = Random.Range(0, availablePositions.Count);

            float spawnX = availablePositions[randomIndex];

            availablePositions.RemoveAt(randomIndex);

            //SpawnMonsterAt(spawnX);

            SpawnMonsterType spawnType = SpawnMonsterType.Normal;

            if (bossCount > 0)
            {
                spawnType = SpawnMonsterType.Boss;
                bossCount--;
            }
            else if (gimmickCount > 0)
            {
                spawnType = SpawnMonsterType.Gimmick;
                gimmickCount--;
            }

            SpawnMonsterAt(spawnX, spawnType);
        }
    }

    /// <summary>
    /// 팩토리에서 몬스터를 스폰하는 함수
    /// </summary>
    /// <param name="xPos">스폰될 X 위치</param>
    /// /// <param name="spawnType">스폰할 몬스터 종류</param>
    private void SpawnMonsterAt(float xPos, SpawnMonsterType spawnType)
    {
        /*// 현재는 테스트로 초록 고블린만 스폰 중
        MonsterBase monster = Factory.Instance.GetMonster_Goblin_Green();

        if (monster == null)
            return;

        Vector3 spawnPos = new Vector3(xPos, 0.033f, 1.24f);

        monster.transform.position = spawnPos;
        monster.gameObject.SetActive(true);

        RegisterMonster(monster);*/

        MonsterBase monster = null;

        switch (spawnType)
        {
            case SpawnMonsterType.Normal:
                // 현재는 테스트로 초록 고블린만 스폰
                monster = Factory.Instance.GetMonster_Goblin_Green();
                break;

            case SpawnMonsterType.Gimmick:
                // 테스트용
                monster = Factory.Instance.GetMonster_Skull_Poison_Warrior();
                // 추후 :
                // monster = Factory.Instance.GetHealMonster();
                break;

            case SpawnMonsterType.Boss:
                // 테스트용
                monster = Factory.Instance.GetMonster_Slime_Orange_King();
                // 추후 :
                // monster = Factory.Instance.GetBossMonster();
                break;
        }

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
            return Random.Range(5, 7); // 5 ~ 6

        return 7;
    }

    /// <summary>
    /// 턴 마다 스폰되는 기믹 몬스터의 수를 조절하는 함수
    /// </summary>
    /// <param name="turn"></param>
    /// <param name="spawnCount"></param>
    /// <returns></returns>
    private int GetGimmickCount(int turn, int remainCount)
    {
        int gimmickCount = 0;

        if (turn > 39)
            gimmickCount = remainCount;     // 40턴 이후부터는 모든 몬스터 기믹
        else if (turn > 34)
            gimmickCount = 6;       // 35턴 부터는 기믹 몬스터 6마리
        else if (turn > 29)
            gimmickCount = 5;       // 30턴 부터는 기믹 몬스터 5마리
        else if (turn > 24)
            gimmickCount = 4;       // 25턴 부터는 기믹 몬스터 4마리
        else if (turn > 19)
            gimmickCount = 3;       // 20턴 부터는 기믹 몬스터 3마리
        else if (turn > 14)
            gimmickCount = 2;       // 15턴 부터는 기믹 몬스터 2마리
        else if (turn > 9)
            gimmickCount = 1;       // 10턴 부터는 기믹 몬스터 1마리

        return Mathf.Min(gimmickCount, remainCount);
    }

    /// <summary>
    /// 10번째 턴마다 보스 몬스터의 스폰 수를 계산하는 함수
    /// </summary>
    /// <param name="turn"></param>
    /// <returns></returns>
    private int GetBossCount(int turn)
    {
        return (turn % 10 == 0) ? 1 : 0;

        // 50턴부터 보스 2마리
        // 100턴부터 보스 + 엘리트
        // 같은 규칙이 생겨도 여기서 처리하면 됨
    }
}
