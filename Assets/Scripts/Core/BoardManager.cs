using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 필드에 어떤 몬스터가 어느 칸에 있는지 관리하는 클래스
/// </summary>
/// 
/// 사용법: 몬스터 이동/스폰 등 필드 상태가 바뀔 때마다 Refresh()를 호출해서
/// activeMonsters 리스트 기준으로 cells 배열을 다시 그려야 최신 상태가 유지됨.
/// (자동으로 동기화되는 게 아니라, 누군가 Refresh를 불러줘야 갱신됨)
/// 
/// IsEmpty / GetMonsterAt / GetEmptyCells 로 조회 가능
public class BoardManager : Singleton<BoardManager>
{
    // 보드의 가로 세로 칸 수
    public const int Width = 7;
    public const int Height = 9;

    /// <summary>
    /// 칸 하나의 크기(월드 좌표 기준 간격)
    /// </summary>
    private const float CellSize = 0.31f;

    /// <summary>
    /// 일반/기믹 몬스터가 스폰되는 최상단 줄의 월드 Z좌표
    /// </summary>
    private const float TopZ = 1.24f;

    /// <summary>
    /// 보스 몬스터가 스폰되는 Z좌표
    /// 스폰 직후엔 최상단 줄에 절반만 걸치고, 한 턴 지나야 온전히 2줄을 채우게 됨.
    /// (이 오프셋 덕분에 MoveMonstersDown의 -0.31f 이동 로직을 보스도 그대로 재사용 가능)
    /// </summary>
    private const float BossSpawnZ = 1.395f; // TopZ + CellSize/2

    /// <summary>
    /// 일반/기믹 몬스터가 스폰될 수 있는 7개의 X좌표.
    /// </summary>
    private readonly float[] columnXPositions =
    {
        -0.93f, -0.62f, -0.31f, 0f, 0.31f, 0.62f, 0.93f
    };

    /// <summary>
    /// 보스 몬스터가 스폰될 수 있는 6개의 X좌표
    /// </summary>
    private readonly float[] bossColumnXPositions =
    {
        -0.775f, -0.465f, -0.155f, 0.155f, 0.465f, 0.775f
    };

    /// <summary>
    /// 실제 보드 점유 상태를 저장하는 2차원 배열
    /// cells[x, z]가 null이면 빈 칸, 아니면 그 칸을 차지한 몬스터의 참조.
    /// 보스처럼 여러 칸을 차지하는 몬스터는 점유한 칸 전부에 같은 참조가 들어감.
    /// </summary>
    private MonsterBase[,] cells = new MonsterBase[Width, Height];

    /// <summary>
    /// 주어진 (x, z)가 보드 범위 안에 있는 유효한 칸인지 확인.
    /// 배열 인덱스 밖으로 벗어나는 접근(IndexOutOfRange)을 막기 위한 안전장치.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public bool IsValidCell(int x, int z)
    {
        return x >= 0 && x < Width && z >= 0 && z < Height;
    }

    /// <summary>
    /// 해당 칸이 유효하면서 동시에 비어있는지 확인.
    /// 기믹(예: 빈 칸에 몬스터 소환)에서 "여기 넣어도 되는가" 체크할 때 사용.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public bool IsEmpty(int x, int z)
    {
        return IsValidCell(x, z) && cells[x, z] == null;
    }

    /// <summary>
    /// 해당 칸에 있는 몬스터를 반환. 비어있거나 범위 밖이면 null.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public MonsterBase GetMonsterAt(int x, int z)
    {
        return IsValidCell(x, z) ? cells[x, z] : null;
    }

    /// <summary>
    /// 매 턴, 이동/스폰이 끝난 뒤 호출해서 보드 상태를 최신화.
    /// 몬스터 수가 적어서(최대 수십 마리) 매번 통째로 다시 그려도 성능 문제 없음.
    /// </summary>
    public void Refresh(List<MonsterBase> activeMonsters)
    {
        // 배열 전체를 null로 초기화 (이전 턴 상태를 지움)
        System.Array.Clear(cells, 0, cells.Length);

        foreach (MonsterBase monster in activeMonsters)
        {
            // 리스트에 null로 남아있거나(파괴됨), 비활성화된(죽어서 풀에 반환된) 몬스터는 제외
            if (monster == null || !monster.gameObject.activeSelf)
                continue;

            // 몬스터 종류에 따라 등록 방식이 다름 (보스는 4칸, 나머지는 1칸)
            if (monster.SpawnType == SpawnMonsterType.Boss)
                RegisterBoss(monster);
            else
                RegisterSingle(monster);
        }
    }

    /// <summary>
    /// 일반/기믹 몬스터(1칸짜리)를 격자 좌표로 변환해서 cells에 등록.
    /// </summary>
    /// <param name="monster"></param>
    private void RegisterSingle(MonsterBase monster)
    {
        int x = FindColumnIndex(monster.transform.position.x);
        int z = Mathf.RoundToInt((TopZ - monster.transform.position.z) / CellSize);

        if (IsValidCell(x, z))
            cells[x, z] = monster;
    }

    /// <summary>
    /// 보스 몬스터(2x2, 4칸짜리)를 격자 좌표로 변환해서 cells에 등록.
    /// 보스는 X 2칸, Z 2칸을 동시에 차지하므로 4개의 셀 모두에 같은 참조를 넣음.
    /// </summary>
    /// <param name="boss"></param>
    private void RegisterBoss(MonsterBase boss)
    {
        // 보스의 X좌표로 어느 두 열(slot, slot+1)을 차지하는지 찾음
        int slot = FindBossColumnIndex(boss.transform.position.x);
        if (slot < 0) return;       // 매칭되는 좌표가 없으면(값 불일치 등) 등록 포기

        // 보스는 BossSpawnZ(=TopZ + 반 칸)에서 스폰되므로,
        // raw 값은 "스폰 후 몇 턴이 지났는지"를 나타내는 지표가 됨.
        // raw=0(스폰 직후): topRow=-1(보드 밖, 무시됨), bottomRow=0만 등록
        // raw=1(한 턴 경과): topRow=0, bottomRow=1 둘 다 정상 등록 → 온전히 2줄 차지
        int raw = Mathf.RoundToInt((BossSpawnZ - boss.transform.position.z) / CellSize);
        int topRow = raw - 1;
        int bottomRow = raw;

        // 보스가 차지하는 두 열(dx=0, dx=1) x 두 줄(topRow, bottomRow) = 최대 4칸에 등록
        for (int dx = 0; dx <= 1; dx++)
        {
            int x = slot + dx;
            if (IsValidCell(x, topRow)) cells[x, topRow] = boss;
            if (IsValidCell(x, bottomRow)) cells[x, bottomRow] = boss;
        }
    }

    /// <summary>
    /// 월드 X좌표로 columnXPositions 배열 상의 인덱스(0~6)를 찾음.
    /// 부동소수점 비교라 Mathf.Approximately 사용 (== 비교는 오차로 실패할 수 있음).
    /// </summary>
    /// <returns>일치하는 인덱스, 없으면 -1</returns>
    private int FindColumnIndex(float worldX)
    {
        for (int i = 0; i < columnXPositions.Length; i++)
            if (Mathf.Approximately(columnXPositions[i], worldX))
                return i;
        return -1;
    }

    /// <summary>
    /// 월드 X좌표로 bossColumnXPositions 배열 상의 인덱스(0~5)를 찾음.
    /// </summary>
    /// <returns>일치하는 인덱스, 없으면 -1</returns>
    private int FindBossColumnIndex(float worldX)
    {
        for (int i = 0; i < bossColumnXPositions.Length; i++)
            if (Mathf.Approximately(bossColumnXPositions[i], worldX))
                return i;
        return -1;
    }

    /// <summary>
    /// 현재 비어있는 칸들의 좌표 목록을 반환.
    /// 보스 기믹(예: 빈 공간에 몬스터 N마리 강화소환) 등에서
    /// "어디에 새로 몬스터를 놓을 수 있는가"를 조회할 때 사용 예정.
    /// </summary>
    /// <param name="excludeRows">
    /// 결과에서 제외할 줄(z 인덱스) 목록.
    /// 예: 가장 아랫줄(Height - 1)을 제외하고 싶으면 { Height - 1 }을 넘기면 됨.
    /// null이면 모든 줄을 대상으로 검색.
    /// </param>
    public List<Vector2Int> GetEmptyCells(HashSet<int> excludeRows = null)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        for (int z = 0; z < Height; z++)
        {
            if (excludeRows != null && excludeRows.Contains(z))
                continue;

            for (int x = 0; x < Width; x++)
            {
                if (cells[x, z] == null)
                    result.Add(new Vector2Int(x, z));
            }
        }

        return result;
    }
}