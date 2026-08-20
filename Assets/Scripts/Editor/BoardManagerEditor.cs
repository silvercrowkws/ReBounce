// Assets/Editor/BoardManagerEditor.cs (반드시 Editor 폴더 안에 위치해야 함)
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoardManager))]
public class BoardManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BoardManager board = (BoardManager)target;

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("플레이 모드에서만 보드 상태를 확인할 수 있습니다.", MessageType.Info);
            return;
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("보드 상태", EditorStyles.boldLabel);

        for (int z = 0; z < BoardManager.Height; z++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < BoardManager.Width; x++)
            {
                MonsterBase monster = board.GetMonsterAt(x, z);

                Color prevColor = GUI.backgroundColor;
                GUI.backgroundColor = GetCellColor(monster);

                string label = monster == null ? "." : GetCellLabel(monster);
                GUILayout.Button(label, GUILayout.Width(32), GUILayout.Height(24));

                GUI.backgroundColor = prevColor;
            }

            EditorGUILayout.EndHorizontal();
        }

        // 플레이 중 매 프레임 갱신되도록
        Repaint();
    }

    private Color GetCellColor(MonsterBase monster)
    {
        if (monster == null) return Color.white;

        return monster.SpawnType switch
        {
            SpawnMonsterType.Boss => new Color(1f, 0.4f, 0.4f),
            SpawnMonsterType.Gimmick => new Color(1f, 0.9f, 0.4f),
            _ => new Color(0.6f, 0.8f, 1f),
        };
    }

    private string GetCellLabel(MonsterBase monster)
    {
        // CurrentHP 등 MonsterBase에 있는 정보로 원하는 대로 커스터마이즈 가능
        return Mathf.RoundToInt(monster.CurrentHP).ToString();
    }
}