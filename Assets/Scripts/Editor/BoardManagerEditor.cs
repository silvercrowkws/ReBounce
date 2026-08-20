// Assets/Editor/BoardManagerEditor.cs (반드시 Editor 폴더 안에 위치해야 함)
using UnityEditor;
using UnityEngine;

/// <summary>
/// BoardManager의 기본 Inspector 대신, 7x9 보드 상태를 격자 형태로 직접 그려주는
/// 커스텀 에디터. UnityEditor 네임스페이스를 쓰기 때문에 Editor 폴더 밖으로 나가면
/// 빌드 시 컴파일 에러가 남 - 반드시 "Editor"라는 이름의 폴더 안에 위치해야 함.
///
/// [CustomEditor(typeof(BoardManager))]가 핵심: BoardManager 컴포넌트를 가진
/// 오브젝트를 선택했을 때 Unity가 자동으로 이 클래스를 대신 사용해서 Inspector를
/// 그려줌. 별도로 어디에 붙이거나 참조를 연결할 필요 없음.
/// </summary>
[CustomEditor(typeof(BoardManager))]
public class BoardManagerEditor : Editor
{
    /// <summary>
    /// Inspector가 그려질 때마다 Unity가 자동으로 호출하는 함수.
    /// 여기서 기본 필드들(DrawDefaultInspector)에 이어서, 보드 격자를 추가로 그림.
    /// </summary>
    public override void OnInspectorGUI()
    {
        // BoardManager에 원래 있던 필드들(있다면)을 그대로 먼저 그려줌
        DrawDefaultInspector();

        // target은 현재 선택된 오브젝트에 붙은 BoardManager 컴포넌트를 가리킴
        BoardManager board = (BoardManager)target;

        // 플레이 모드가 아니면 cells 배열이 비어있는 초기 상태(에디터에서 인스턴스가
        // 아직 안 만들어졌거나 실행 전이라 의미 없는 값)이므로 안내 문구만 표시하고 종료
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("플레이 모드에서만 보드 상태를 확인할 수 있습니다.", MessageType.Info);
            return;
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("보드 상태", EditorStyles.boldLabel);

        // z=0(최상단 스폰 라인)부터 z=Height-1(게임오버 라인 방향)까지 한 줄씩 그림
        for (int z = 0; z < BoardManager.Height; z++)
        {
            // 한 줄을 가로로 나열하기 위해 가로 레이아웃 시작
            EditorGUILayout.BeginHorizontal();

            // x=0(왼쪽)부터 x=Width-1(오른쪽)까지 한 칸씩 그림
            for (int x = 0; x < BoardManager.Width; x++)
            {
                MonsterBase monster = board.GetMonsterAt(x, z);

                // 버튼 배경색을 몬스터 종류에 맞게 바꾸기 위해 현재 색을 백업
                Color prevColor = GUI.backgroundColor;
                GUI.backgroundColor = GetCellColor(monster);

                // 빈 칸은 "."으로, 몬스터가 있으면 라벨(현재는 HP)을 표시
                string label = monster == null ? "." : GetCellLabel(monster);
                GUILayout.Button(label, GUILayout.Width(32), GUILayout.Height(24));

                // 다음 칸에 영향 안 주도록 배경색 원상복구
                GUI.backgroundColor = prevColor;
            }

            EditorGUILayout.EndHorizontal();
        }

        // 플레이 중 매 프레임 갱신되도록 강제 리페인트
        // (턴이 진행되면서 cells 내용이 바뀌어도 Inspector가 자동으로 새로고침되게 함)
        Repaint();
    }

    /// <summary>
    /// 몬스터 종류에 따라 칸의 배경색을 결정.
    /// 보스=빨강, 기믹=노랑, 일반=하늘색, 빈 칸=흰색으로 한눈에 구분되게 함.
    /// </summary>
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

    /// <summary>
    /// 칸 버튼에 표시할 텍스트를 결정.
    /// 지금은 현재 HP를 정수로 반올림해서 표시하고 있음.
    /// 필요하면 속성 이니셜, 기믹 이름 등 다른 정보로 바꿔서 쓸 수 있음.
    /// </summary>
    private string GetCellLabel(MonsterBase monster)
    {
        return Mathf.RoundToInt(monster.CurrentHP).ToString();
    }
}