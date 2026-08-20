using System.Collections.Generic;
using UnityEngine;

/*[System.Serializable]
public class BoardRow
{
    public MonsterBase[] cells = new MonsterBase[BoardManager.Width];
}*/

public class BoardManager : Singleton<BoardManager>
{
    public const int Width = 7;
    public const int Height = 9;

    private const float CellSize = 0.31f;
    private const float TopZ = 1.24f;
    private const float BossSpawnZ = 1.395f; // TopZ + CellSize/2

    private readonly float[] columnXPositions =
    {
        -0.93f, -0.62f, -0.31f, 0f, 0.31f, 0.62f, 0.93f
    };

    private readonly float[] bossColumnXPositions =
    {
        -0.775f, -0.465f, -0.155f, 0.155f, 0.465f, 0.775f
    };

    private MonsterBase[,] cells = new MonsterBase[Width, Height];

    public bool IsValidCell(int x, int z)
    {
        return x >= 0 && x < Width && z >= 0 && z < Height;
    }

    public bool IsEmpty(int x, int z)
    {
        return IsValidCell(x, z) && cells[x, z] == null;
    }

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
        System.Array.Clear(cells, 0, cells.Length);

        foreach (MonsterBase monster in activeMonsters)
        {
            if (monster == null || !monster.gameObject.activeSelf)
                continue;

            if (monster.SpawnType == SpawnMonsterType.Boss)
                RegisterBoss(monster);
            else
                RegisterSingle(monster);
        }
    }

    private void RegisterSingle(MonsterBase monster)
    {
        int x = FindColumnIndex(monster.transform.position.x);
        int z = Mathf.RoundToInt((TopZ - monster.transform.position.z) / CellSize);

        if (IsValidCell(x, z))
            cells[x, z] = monster;
    }

    private void RegisterBoss(MonsterBase boss)
    {
        int slot = FindBossColumnIndex(boss.transform.position.x);
        if (slot < 0) return;

        int raw = Mathf.RoundToInt((BossSpawnZ - boss.transform.position.z) / CellSize);
        int topRow = raw - 1;
        int bottomRow = raw;

        for (int dx = 0; dx <= 1; dx++)
        {
            int x = slot + dx;
            if (IsValidCell(x, topRow)) cells[x, topRow] = boss;
            if (IsValidCell(x, bottomRow)) cells[x, bottomRow] = boss;
        }
    }

    private int FindColumnIndex(float worldX)
    {
        for (int i = 0; i < columnXPositions.Length; i++)
            if (Mathf.Approximately(columnXPositions[i], worldX))
                return i;
        return -1;
    }

    private int FindBossColumnIndex(float worldX)
    {
        for (int i = 0; i < bossColumnXPositions.Length; i++)
            if (Mathf.Approximately(bossColumnXPositions[i], worldX))
                return i;
        return -1;
    }

    /// <summary>
    /// 특정 줄(들)을 제외한 빈 칸 목록 (강화소환 기믹용)
    /// </summary>
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