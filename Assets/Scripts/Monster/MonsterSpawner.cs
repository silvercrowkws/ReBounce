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
        ExecuteTurnEndGimmicks();
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
                spawnType = SpawnMonsterType.Boss;      // 보스 몬스터로 타입 변경
                bossCount--;
            }
            else if (gimmickCount > 0)
            {
                spawnType = SpawnMonsterType.Gimmick;   // 기믹 몬스터로 타입 변경
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

        /*MonsterBase monster = null;
        switch (spawnType)
        {
            case SpawnMonsterType.Normal:
                // 현재는 테스트로 초록 고블린만 스폰
                //monster = Factory.Instance.GetMonster_Goblin_Green();
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
        }*/

        // 각 타입에 맞는 랜덤한 몬스터를 결정
        /*MonsterBase monster = GetRandomMonster(spawnType);

        if (monster == null)
            return;*/


        
        Vector3 spawnPos = new Vector3(xPos, 0.033f, 1.24f);

        MonsterSpawnData spawnData = GetRandomMonster(spawnType);

        MonsterBase monster = spawnData.monster;

        monster.Initialize(spawnData);

        // 위치 설정
        monster.transform.position = spawnPos;

        // 리스트 등록
        RegisterMonster(monster);
/*
        monster.transform.position = spawnPos;
        monster.gameObject.SetActive(true);

        RegisterMonster(monster);*/
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

    private MonsterSpawnData GetRandomMonster(SpawnMonsterType spawnType)
    {
        MonsterSpawnData spawnData = new MonsterSpawnData();

        // 스폰 타입 저장
        spawnData.spawnType = spawnType;

        // 속성 결정
        spawnData.element = GetRandomElement();

        // 기믹 결정
        switch (spawnType)
        {
            case SpawnMonsterType.Normal:
                spawnData.gimmick = MonsterGimmicks.None;
                spawnData.monster = GetRandomNormalMonster();
                break;

            case SpawnMonsterType.Gimmick:
                spawnData.gimmick = GetRandomGimmick();
                spawnData.monster = GetRandomGimmickMonster();
                break;

            case SpawnMonsterType.Boss:
                spawnData.gimmick = MonsterGimmicks.None;   // 일단 보스 기믹은 없는 상태(변경 예정)
                spawnData.monster = GetRandomBossMonster();
                break;
        }

        /*// 몬스터 결정 => 나중에 각 몬스터 별로
         * Factory.Instance.GetMonster_Skull 은 A 기믹
         * Instance.GetMonster_Skull_Archer 는 B 기믹
         * 이런 식으로 변경할 때 쓸지도?
        switch (spawnType)
        {
            case SpawnMonsterType.Normal:
                switch (Random.Range(0, 6))
                {
                    case 0: spawnData.monster = Factory.Instance.GetMonster_Goblin_Green(); break;
                    case 1: spawnData.monster = Factory.Instance.GetMonster_Goblin_Green_Archer(); break;
                    case 2: spawnData.monster = Factory.Instance.GetMonster_Goblin_Green_Warrior(); break;
                    case 3: spawnData.monster = Factory.Instance.GetMonster_Goblin_Yellow(); break;
                    case 4: spawnData.monster = Factory.Instance.GetMonster_Goblin_Yellow_Archer(); break;
                    case 5: spawnData.monster = Factory.Instance.GetMonster_Goblin_Yellow_Warrior(); break;
                }
                break;

            case SpawnMonsterType.Gimmick:
                switch (Random.Range(0, 6))
                {
                    case 0: spawnData.monster = Factory.Instance.GetMonster_Skull(); break;
                    case 1: spawnData.monster = Factory.Instance.GetMonster_Skull_Archer(); break;
                    case 2: spawnData.monster = Factory.Instance.GetMonster_Skull_Warrior(); break;
                    case 3: spawnData.monster = Factory.Instance.GetMonster_Skull_Poison(); break;
                    case 4: spawnData.monster = Factory.Instance.GetMonster_Skull_Poison_Archer(); break;
                    case 5: spawnData.monster = Factory.Instance.GetMonster_Skull_Poison_Warrior(); break;
                }
                break;

            case SpawnMonsterType.Boss:
                switch (Random.Range(0, 6))
                {
                    case 0: spawnData.monster = Factory.Instance.GetMonster_Slime_Green(); break;
                    case 1: spawnData.monster = Factory.Instance.GetMonster_Slime_Green_King(); break;
                    case 2: spawnData.monster = Factory.Instance.GetMonster_Slime_Green_Stone(); break;
                    case 3: spawnData.monster = Factory.Instance.GetMonster_Slime_Orange(); break;
                    case 4: spawnData.monster = Factory.Instance.GetMonster_Slime_Orange_King(); break;
                    case 5: spawnData.monster = Factory.Instance.GetMonster_Slime_Orange_Stone(); break;
                }
                break;
        }*/

        return spawnData;
    }

    /// <summary>
    /// 일반 몬스터 랜덤 결정
    /// </summary>
    /// <returns></returns>
    private MonsterBase GetRandomNormalMonster()
    {
        switch (Random.Range(0, 6))
        {
            case 0: return Factory.Instance.GetMonster_Goblin_Green();
            case 1: return Factory.Instance.GetMonster_Goblin_Green_Archer();
            case 2: return Factory.Instance.GetMonster_Goblin_Green_Warrior();
            case 3: return Factory.Instance.GetMonster_Goblin_Yellow();
            case 4: return Factory.Instance.GetMonster_Goblin_Yellow_Archer();
            case 5: return Factory.Instance.GetMonster_Goblin_Yellow_Warrior();
        }

        return null;
    }

    /// <summary>
    /// 기믹 몬스터 랜덤 결정
    /// </summary>
    /// <returns></returns>
    private MonsterBase GetRandomGimmickMonster()
    {
        switch (Random.Range(0, 6))
        {
            case 0: return Factory.Instance.GetMonster_Skull();
            case 1: return Factory.Instance.GetMonster_Skull_Archer();
            case 2: return Factory.Instance.GetMonster_Skull_Warrior();
            case 3: return Factory.Instance.GetMonster_Skull_Poison();
            case 4: return Factory.Instance.GetMonster_Skull_Poison_Archer();
            case 5: return Factory.Instance.GetMonster_Skull_Poison_Warrior();
        }

        return null;
    }

    /// <summary>
    /// 보스 몬스터 랜덤 결정
    /// </summary>
    /// <returns></returns>
    private MonsterBase GetRandomBossMonster()
    {
        switch (Random.Range(0, 6))
        {
            case 0: return Factory.Instance.GetMonster_Slime_Green();
            case 1: return Factory.Instance.GetMonster_Slime_Green_King();
            case 2: return Factory.Instance.GetMonster_Slime_Green_Stone();
            case 3: return Factory.Instance.GetMonster_Slime_Orange();
            case 4: return Factory.Instance.GetMonster_Slime_Orange_King();
            case 5: return Factory.Instance.GetMonster_Slime_Orange_Stone();
        }

        return null;
    }

    /// <summary>
    /// 가중치 랜덤 속성 결정 함수
    /// </summary>
    /// <returns></returns>
    private MonsterElementals GetRandomElement()
    {
        int rand = UnityEngine.Random.Range(0, 100);

        if (rand < 50)
            return MonsterElementals.Normal;    // 50% 확률로 노말

        if (rand < 60)
            return MonsterElementals.Fire;      // 10% 확률로 불

        if (rand < 70)
            return MonsterElementals.Water;     // 10% 확률로 물

        if (rand < 80)
            return MonsterElementals.Land;      // 10% 확률로 땅

        if (rand < 90)
            return MonsterElementals.Electric;  // 10% 확률로 전기

        return MonsterElementals.Wind;          // 10% 확률로 바람 
    }


    /// <summary>
    /// 테스트 용이므로 나중에 수정 필요
    /// 랜덤 기믹을 결정하는 함수
    /// </summary>
    private MonsterGimmicks GetRandomGimmick()
    {
        int random = Random.Range(0, 4);

        switch (random)
        {
            case 0: return MonsterGimmicks.Heal;
            case 1: return MonsterGimmicks.Barrier;
            case 2: return MonsterGimmicks.Shield;
            case 3: return MonsterGimmicks.Magnetic;
        }

        return MonsterGimmicks.None;
    }

    /// <summary>
    /// 매 턴이 끝날 때, 필드에 있는 기믹 몬스터들을 찾아서
    /// 각자의 턴 종료 기믹을 실행시켜주는 함수
    /// </summary>
    private void ExecuteTurnEndGimmicks()
    {
        // 현재 필드에 살아있는 몬스터(activeMonsters) 순회
        foreach (MonsterBase monster in activeMonsters)
        {
            // null 상태의 몬스터나 비활성화된 몬스터는 건너뜀
            if (monster == null || !monster.gameObject.activeSelf)
                continue;

            // 기믹이 없는 일반 몬스터 / 보스 몬스터는 건너뜀
            if (monster.MonsterGimmick == MonsterGimmicks.None)
                continue;

            // 턴 종료 기믹이 있는 몬스터만 자기 자신의 OnTurnEndGimmick()을 실행
            monster.OnTurnEndGimmick();
        }
    }
}
