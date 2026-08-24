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

    /// <summary>
    /// 보스가 스폰될 수 있는 X 위치 (일반 칸 사이사이 위치)
    /// bossSpawnXPositions[i] 는 spawnXPositions[i], spawnXPositions[i+1] 두 칸을 점유
    /// </summary>
    private readonly float[] bossSpawnXPositions =
    {
        -0.775f,
        -0.465f,
        -0.155f,
         0.155f,
         0.465f,
         0.775f
    };

    private const float NormalSpawnZ = 1.24f;
    private const float BossSpawnZ = 1.395f;

    /// <summary>
    /// 보스가 아직 최상단 스폰 라인을 막고 있다고 판단하는 Z 오차 범위
    /// (스폰 직후 diff = 0.155, 한 턴 이동 후 diff = 0.155, 두 턴 이동 후 diff = 0.465)
    /// </summary>
    private const float BossBlockZThreshold = 0.3f;

    TurnManager turnManager;

    /// <summary>
    /// 강화소환된 몬스터의 체력 배율 (이번 턴 일반 몬스터 체력 대비)
    /// </summary>
    [SerializeField]
    private float reinforceHPRatio = 1f / 3f;

    private void Start()
    {
        turnManager = TurnManager.Instance;
        turnManager.onTurnEnd += OnTurnEnd;
        turnManager.onTurnStart += OnTurnStart;

        SpawnMonsters(); // 첫 웨이브

        BoardManager.Instance.Refresh(activeMonsters);      // 보드 갱신
    }

    private void OnDestroy()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.onTurnEnd -= OnTurnEnd;
            TurnManager.Instance.onTurnStart -= OnTurnStart;
        }
    }

    /// <summary>
    /// 턴 시작 시(공 발사 전) 호출. 보스의 소환 기믹 등 턴 시작 기믹을 실행.
    /// </summary>
    private void OnTurnStart(int turnNumber)
    {
        ExecuteTurnStartGimmicks();
        BoardManager.Instance.Refresh(activeMonsters);
    }

    private void OnTurnEnd()
    {
        ExecuteTurnEndGimmicks();
        MoveMonstersDown();
        BoardManager.Instance.Refresh(activeMonsters);   // 이동 반영

        SpawnMonsters();
        BoardManager.Instance.Refresh(activeMonsters);   // 스폰 반영
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

            //if (pos.z <= -1.24f)
            if (pos.z <= -1.085f)
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

    /*/// <summary>
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
    }*/




    public void SpawnMonsters()
    {
        Debug.Log("몬스터 스폰");

        int turn = TurnManager.Instance.turnNumber + 1;

        // 1. 이번 턴에 필요한 보스 수
        int desiredBossCount = GetBossCount(turn);

        // 2. 기존에 살아있는 보스가 아직 최상단 라인을 막고 있는지 계산
        HashSet<int> blockedByExistingBoss = GetBossBlockedNormalIndices();

        // 3. 새 보스가 쓸 수 있는 슬롯(기존 보스와 안 겹치는 것)
        List<int> availableBossSlots = GetAvailableBossSlots(blockedByExistingBoss);

        int actualBossCount = Mathf.Min(desiredBossCount, availableBossSlots.Count);

        if (actualBossCount < desiredBossCount)
        {
            Debug.LogWarning($"보스 스폰 자리 부족 : 목표 {desiredBossCount}마리, 실제 {actualBossCount}마리");
        }

        // 4. 새로 스폰할 보스 슬롯을 서로 안 겹치게 랜덤 선택
        List<int> chosenBossSlots = new List<int>();
        List<int> slotPool = new List<int>(availableBossSlots);

        for (int i = 0; i < actualBossCount; i++)
        {
            int r = Random.Range(0, slotPool.Count);
            int slot = slotPool[r];

            chosenBossSlots.Add(slot);
            slotPool.RemoveAll(s => IsSlotOverlapping(s, slot));
        }

        // 5. 이번 턴 최종적으로 막히는 일반 칸 (기존 보스 + 새 보스)
        HashSet<int> blockedThisTurn = new HashSet<int>(blockedByExistingBoss);
        foreach (int slot in chosenBossSlots)
        {
            blockedThisTurn.Add(slot);
            blockedThisTurn.Add(slot + 1);
        }

        // 6. 보스를 제외하고 실제 스폰 가능한 X좌표 목록
        List<float> availablePositions = new List<float>();
        for (int i = 0; i < spawnXPositions.Length; i++)
        {
            if (!blockedThisTurn.Contains(i))
                availablePositions.Add(spawnXPositions[i]);
        }

        // 7. 보스 제외 스폰 수는 남은 칸 한도 안에서 결정
        int spawnCount = Mathf.Min(GetSpawnCount(), availablePositions.Count);
        int gimmickCount = GetGimmickCount(turn, spawnCount);

        // 8. 보스 먼저 스폰 (자리 선점)
        foreach (int slot in chosenBossSlots)
        {
            SpawnMonsterAt(bossSpawnXPositions[slot], SpawnMonsterType.Boss);
        }

        // 9. 남은 칸에 기믹/일반 몬스터 배치
        for (int i = 0; i < spawnCount; i++)
        {
            if (availablePositions.Count <= 0)
                break;

            int randomIndex = Random.Range(0, availablePositions.Count);
            float spawnX = availablePositions[randomIndex];
            availablePositions.RemoveAt(randomIndex);

            SpawnMonsterType spawnType = SpawnMonsterType.Normal;

            if (gimmickCount > 0)
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


        float z = (spawnType == SpawnMonsterType.Boss) ? BossSpawnZ : NormalSpawnZ;



        Vector3 spawnPos = new Vector3(xPos, 0.033f, z);

        MonsterSpawnData spawnData = GetRandomMonster(spawnType);

        MonsterBase monster = spawnData.monster;

        if (monster == null)
            return;

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
        //return (turn % 10 == 0) ? 1 : 0;

        if(turn % 10 != 0)
        {
            return 0;
        }
        
        // 50턴 이전에는 보스 1마리 이후부터 보스 2마리 동시 스폰
        return turn >= 50 ? 2 : 1;

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
                spawnData.gimmick = GetRandomGimmick();         // 랜덤으로 기믹 결정
                spawnData.monster = GetRandomGimmickMonster();
                break;

            case SpawnMonsterType.Boss:
                /*spawnData.gimmick = GetRandomBossGimmick();     // 랜덤으로 보스 기믹 결정
                spawnData.monster = GetRandomBossMonster();*/
                AssignRandomBossType(spawnData);        // 기믹 + 생김새까지 한 번에 배정
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
    /// 보스 기믹과 그에 맞는 생김새(프리팹)를 함께 랜덤으로 결정하는 함수.
    /// 기믹마다 정해진 생김새가 있어야 하므로, 기믹과 프리팹을 따로 뽑지 않고
    /// 여기서 케이스별로 한 번에 묶어서 배정한다.
    /// </summary>
    private void AssignRandomBossType(MonsterSpawnData spawnData)
    {
        int random = Random.Range(0, 4);

        switch (random)
        {
            case 0:
                spawnData.gimmick = MonsterGimmicks.Summon;
                spawnData.monster = Factory.Instance.GetMonster_Slime_Green_King();
                break;

            case 1:
                spawnData.gimmick = MonsterGimmicks.Summon;
                spawnData.monster = Factory.Instance.GetMonster_Slime_Green_King();
                break;

            case 2:
                spawnData.gimmick = MonsterGimmicks.Summon;
                spawnData.monster = Factory.Instance.GetMonster_Slime_Green_King();
                break;

            case 3:
                spawnData.gimmick = MonsterGimmicks.Summon;
                spawnData.monster = Factory.Instance.GetMonster_Slime_Green_King();
                break;

            default:
                spawnData.gimmick = MonsterGimmicks.None;
                spawnData.monster = GetRandomBossMonster();
                break;
        }
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
    /// 보스 기믹을 랜덤으로 결정하는 함수
    /// </summary>
    private MonsterGimmicks GetRandomBossGimmick()
    {
        int random = Random.Range (0, 4);

        switch (random)
        {
            case 0: return MonsterGimmicks.Summon;
            case 1: return MonsterGimmicks.Summon;
            case 2: return MonsterGimmicks.Summon;
            case 3: return MonsterGimmicks.Summon;
        }

        return MonsterGimmicks.None;
    }

    /// <summary>
    /// 턴 시작 기믹을 가진 몬스터들을 찾아 실행.
    /// 스냅샷을 떠서 순회하는 이유: 강화소환처럼 실행 도중 activeMonsters에
    /// 새 몬스터가 추가될 수 있는데, foreach 중 원본 리스트를 수정하면
    /// InvalidOperationException(컬렉션 변경 예외)이 발생하기 때문.
    /// </summary>
    private void ExecuteTurnStartGimmicks()
    {
        List<MonsterBase> snapshot = new List<MonsterBase>(activeMonsters);

        foreach (MonsterBase monster in snapshot)
        {
            if (monster == null || !monster.gameObject.activeSelf)
                continue;

            if (monster.MonsterGimmick == MonsterGimmicks.None)
                continue;

            monster.OnTurnStartGimmick();
        }
    }

    /// <summary>
    /// 보스 강화소환 기믹 전용 함수.
    /// 필드의 가장 아랫줄을 제외한 빈 칸 중에서 무작위로 count개를 골라
    /// 그 자리에 직접 일반 몬스터를 스폰한다.
    /// (최상단 스폰 라인이 아니라 필드 중간의 빈 칸에 바로 꽂아 넣는다는 점이
    /// 기존 SpawnMonsters와 다름)
    /// </summary>
    public void SpawnReinforcements(int count)
    {
        HashSet<int> excludeRows = new HashSet<int>
        {
            BoardManager.Height - 1,        // 맨 아랫줄 제외
            BoardManager.Height - 2,        // 아래에서 2번째 줄 제외

        };
        List<Vector2Int> emptyCells = BoardManager.Instance.GetEmptyCells(excludeRows);

        for (int i = 0; i < count; i++)
        {
            if (emptyCells.Count <= 0)
            {
                Debug.LogWarning("강화소환 : 남은 빈 칸이 없어서 소환 중단");
                break;
            }

            int randomIndex = Random.Range(0, emptyCells.Count);
            Vector2Int cell = emptyCells[randomIndex];
            emptyCells.RemoveAt(randomIndex);

            Vector3 spawnPos = BoardManager.Instance.GetWorldPosition(cell.x, cell.y);

            /*MonsterSpawnData spawnData = GetRandomMonster(SpawnMonsterType.Normal);     // 기믹 없음            
            spawnData.element = MonsterElementals.Normal;                               // 속성 노말

            spawnData.overrideMaxHP =
            Mathf.Floor(MonsterBase.CalculateHPForTurn(TurnManager.Instance.turnNumber) * reinforceHPRatio);*/

            // 강화소환 몬스터는 랜덤 없이 생김새/속성/기믹을 전부 고정
            MonsterSpawnData spawnData = new MonsterSpawnData
            {
                spawnType = SpawnMonsterType.Normal,
                element = MonsterElementals.Normal,
                gimmick = MonsterGimmicks.None,
                monster = Factory.Instance.GetMonster_Slime_Green(),
                overrideMaxHP = Mathf.Floor(
                    MonsterBase.CalculateHPForTurn(TurnManager.Instance.turnNumber) * reinforceHPRatio)
            };

            MonsterBase monster = spawnData.monster;

            if (monster == null)
                continue;

            monster.Initialize(spawnData);
            monster.transform.position = spawnPos;

            RegisterMonster(monster);

            Debug.Log($"강화소환 : ({cell.x},{cell.y}) 에 {monster.name} 스폰");
        }

        BoardManager.Instance.Refresh(activeMonsters);
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











    /// <summary>
    /// 현재 필드에서, 아직 최상단 스폰 라인에 걸쳐있는 보스들이 막고 있는 일반 칸 인덱스 목록
    /// </summary>
    private HashSet<int> GetBossBlockedNormalIndices()
    {
        HashSet<int> blocked = new HashSet<int>();

        foreach (MonsterBase monster in activeMonsters)
        {
            if (monster == null || !monster.gameObject.activeSelf)
                continue;

            if (monster.SpawnType != SpawnMonsterType.Boss)
                continue;

            float z = monster.transform.position.z;

            if (Mathf.Abs(z - NormalSpawnZ) >= BossBlockZThreshold)
                continue; // 이미 최상단 라인을 지나간 보스는 무시

            int slotIndex = FindBossSlotIndex(monster.transform.position.x);

            if (slotIndex < 0)
                continue; // 안전장치

            blocked.Add(slotIndex);
            blocked.Add(slotIndex + 1);
        }

        return blocked;
    }

    /// <summary>
    /// 보스의 X좌표로 bossSpawnXPositions 배열 상의 인덱스(0~5)를 찾는 함수
    /// </summary>
    private int FindBossSlotIndex(float xPos)
    {
        for (int i = 0; i < bossSpawnXPositions.Length; i++)
        {
            if (Mathf.Approximately(bossSpawnXPositions[i], xPos))
                return i;
        }

        return -1;
    }

    /// <summary>
    /// 아직 막히지 않은 보스 스폰 슬롯(0~5) 목록
    /// </summary>
    private List<int> GetAvailableBossSlots(HashSet<int> blockedNormalIndices)
    {
        List<int> available = new List<int>();

        for (int slot = 0; slot < bossSpawnXPositions.Length; slot++)
        {
            if (blockedNormalIndices.Contains(slot) || blockedNormalIndices.Contains(slot + 1))
                continue;

            available.Add(slot);
        }

        return available;
    }

    /// <summary>
    /// 두 보스 슬롯이 점유하는 일반 칸이 겹치는지 확인
    /// </summary>
    private bool IsSlotOverlapping(int slotA, int slotB)
    {
        return Mathf.Abs(slotA - slotB) <= 1;
    }
}
