using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트 풀을 사용하는 오브젝트의 종류
/// </summary>
public enum PoolObjectType
{
    Monster_Goblin_Green = 0,
    Monster_Goblin_Green_Archer,
    Monster_Goblin_Green_Warrior,

    Monster_Goblin_Yellow,
    Monster_Goblin_Yellow_Archer,
    Monster_Goblin_Yellow_Warrior,

    Monster_Skull,
    Monster_Skull_Archer,
    Monster_Skull_Warrior,

    Monster_Skull_Poison,
    Monster_Skull_Poison_Archer,
    Monster_Skull_Poison_Warrior,

    Monster_Slime_Green,
    Monster_Slime_Green_Stone,
    Monster_Slime_Green_King,
    
    Monster_Slime_Orange,
    Monster_Slime_Orange_Stone,
    Monster_Slime_Orange_King,
}

public class Factory : Singleton<Factory>
{
    // 오브젝트 풀들
    Monster_Goblin_Green_Pool monster_Goblin_Green;
    Monster_Goblin_Green_Archer_Pool monster_Goblin_Green_Archer;
    Monster_Goblin_Green_Warrior_Pool monster_Goblin_Green_Warrior;

    Monster_Goblin_Yellow_Pool monster_Goblin_Yellow;
    Monster_Goblin_Yellow_Archer_Pool monster_Goblin_Yellow_Archer;
    Monster_Goblin_Yellow_Warrior_Pool monster_Goblin_Yellow_Warrior;

    Monster_Skull_Pool monster_Skull;
    Monster_Skull_Archer_Pool monster_Skull_Archer;
    Monster_Skull_Warrior_Pool monster_Skull_Warrior;

    Monster_Skull_Poison_Pool monster_Skull_Poison;
    Monster_Skull_Poison_Archer_Pool monster_Skull_Poison_Archer;
    Monster_Skull_Poison_Warrior_Pool monster_Skull_Poison_Warrior;

    Monster_Slime_Green_Pool monster_Slime_Green;
    Monster_Slime_Green_Stone_Pool monster_Slime_Green_Stone;
    Monster_Slime_Green_King_Pool monster_Slime_Green_King;

    Monster_Slime_Orange_Pool monster_Slime_Orange;
    Monster_Slime_Orange_Stone_Pool monster_Slime_Orange_Stone;
    Monster_Slime_Orange_King_Pool monster_Slime_Orange_King;

    /// <summary>
    /// 씬이 로딩 완료될 때마다 실행되는 초기화 함수
    /// </summary>
    protected override void OnInitialize()
    {
        base.OnInitialize();

        // 풀 컴포넌트 찾고, 찾으면 초기화하기

        // 초록 고블린
        monster_Goblin_Green = GetComponentInChildren<Monster_Goblin_Green_Pool>();
        if (monster_Goblin_Green != null)
        {
            Debug.Log("Monster_Goblin_Green_Pool 초기화");
            monster_Goblin_Green.Initialize();
        }
        else
        {
            Debug.LogError("Monster_Goblin_Green_Pool 을 찾을 수 없습니다.");
        }

        // 초록 고블린 아처
        monster_Goblin_Green_Archer = GetComponentInChildren<Monster_Goblin_Green_Archer_Pool>();
        if (monster_Goblin_Green_Archer != null)
        {
            Debug.Log("Monster_Goblin_Green_Archer_Pool 초기화");
            monster_Goblin_Green_Archer.Initialize();
        }
        else
        {
            Debug.LogError("Monster_Goblin_Green_Archer_Pool 을 찾을 수 없습니다.");
        }

        // 초록 고블린 워리어
        monster_Goblin_Green_Warrior = GetComponentInChildren<Monster_Goblin_Green_Warrior_Pool>();
        if (monster_Goblin_Green_Warrior != null)
        {
            Debug.Log("Monster_Goblin_Green_Warrior_Pool 초기화");
            monster_Goblin_Green_Warrior.Initialize();
        }
        else
        {
            Debug.LogError("Monster_Goblin_Green_Warrior_Pool 을 찾을 수 없습니다.");
        }



        // 노랑 고블린
        monster_Goblin_Yellow = GetComponentInChildren<Monster_Goblin_Yellow_Pool>();
        if (monster_Goblin_Yellow != null)
        {
            Debug.Log("Monster_Goblin_Yellow_Pool 초기화");
            monster_Goblin_Yellow.Initialize();
        }
        else
        {
            Debug.LogError("Monster_Goblin_Yellow_Pool 을 찾을 수 없습니다.");
        }

        // 노랑 고블린 아처
        monster_Goblin_Yellow_Archer = GetComponentInChildren<Monster_Goblin_Yellow_Archer_Pool>();
        if (monster_Goblin_Yellow_Archer != null)
        {
            Debug.Log("Monster_Goblin_Yellow_Archer_Pool 초기화");
            monster_Goblin_Yellow_Archer.Initialize();
        }
        else
        {
            Debug.LogError("Monster_Goblin_Yellow_Archer_Pool 을 찾을 수 없습니다.");
        }

        // 노랑 고블린 워리어
        monster_Goblin_Yellow_Warrior = GetComponentInChildren<Monster_Goblin_Yellow_Warrior_Pool>();
        if (monster_Goblin_Yellow_Warrior != null)
        {
            Debug.Log("Monster_Goblin_Yellow_Warrior_Pool 초기화");
            monster_Goblin_Yellow_Warrior.Initialize();
        }
        else
        {
            Debug.LogError("Monster_Goblin_Yellow_Warrior_Pool 을 찾을 수 없습니다.");
        }



        // 스컬
        monster_Skull = GetComponentInChildren<Monster_Skull_Pool>();
        if (monster_Skull != null)
        {
            Debug.Log("Monster_Skull_Pool 초기화");
            monster_Skull.Initialize();
        }
        else
        {
            Debug.LogError("Monster_Skull_Pool 을 찾을 수 없습니다.");
        }

        // 스컬 아처
        monster_Skull_Archer = GetComponentInChildren<Monster_Skull_Archer_Pool>();
        if (monster_Skull_Archer != null)
        {
            Debug.Log("Monster_Skull_Archer_Pool 초기화");
            monster_Skull_Archer.Initialize();
        }
        else
        {
            Debug.LogError("Monster_Skull_Archer_Pool 을 찾을 수 없습니다.");
        }

        // 스컬 워리어
        monster_Skull_Warrior = GetComponentInChildren<Monster_Skull_Warrior_Pool>();
        if (monster_Skull_Warrior != null)
        {
            Debug.Log("Monster_Skull_Warrior_Pool 초기화");
            monster_Skull_Warrior.Initialize();
        }
        else
        {
            Debug.LogError("Monster_Skull_Warrior_Pool 을 찾을 수 없습니다.");
        }



        // 스컬 포이즌
        monster_Skull_Poison = GetComponentInChildren<Monster_Skull_Poison_Pool>();
        if (monster_Skull_Poison != null)
        {
            Debug.Log("Monster_Skull_Poison_Pool 초기화");
            monster_Skull_Poison.Initialize();
        }
        else
        {
            Debug.LogError("Monster_Skull_Poison_Pool 을 찾을 수 없습니다.");
        }

        // 스컬 포이즌 아처
        monster_Skull_Poison_Archer = GetComponentInChildren<Monster_Skull_Poison_Archer_Pool>();
        if (monster_Skull_Poison_Archer != null)
        {
            Debug.Log("Monster_Skull_Poison_Archer_Pool 초기화");
            monster_Skull_Poison_Archer.Initialize();
        }
        else
        {
            Debug.LogError("Monster_Skull_Poison_Archer_Pool 을 찾을 수 없습니다.");
        }

        // 스컬 포이즌 워리어
        monster_Skull_Poison_Warrior = GetComponentInChildren<Monster_Skull_Poison_Warrior_Pool>();
        if (monster_Skull_Poison_Warrior != null)
        {
            Debug.Log("Monster_Skull_Poison_Warrior_Pool 초기화");
            monster_Skull_Poison_Warrior.Initialize();
        }
        else
        {
            Debug.LogError("Monster_Skull_Poison_Warrior_Pool 을 찾을 수 없습니다.");
        }


        // 초록 슬라임
        monster_Slime_Green = GetComponentInChildren<Monster_Slime_Green_Pool>();
        if (monster_Slime_Green != null)
        {
            Debug.Log("Monster_Slime_Green_Pool 초기화");
            monster_Slime_Green.Initialize();
        }
        else
        {
            Debug.LogError("Monster_Slime_Green 을 찾을 수 없습니다.");
        }

        // 초록 슬라임 스톤
        monster_Slime_Green_Stone = GetComponentInChildren<Monster_Slime_Green_Stone_Pool>();
        if (monster_Slime_Green_Stone != null)
        {
            Debug.Log("Monster_Slime_Green_Stone_Pool 초기화");
            monster_Slime_Green_Stone.Initialize();
        }
        else
        {
            Debug.LogError("Monster_Slime_Green_Stone_Pool 을 찾을 수 없습니다.");
        }

        // 초록 슬라임 킹
        monster_Slime_Green_King = GetComponentInChildren<Monster_Slime_Green_King_Pool>();
        if (monster_Slime_Green_King != null)
        {
            Debug.Log("Monster_Slime_Green_King_Pool 초기화");
            monster_Slime_Green_King.Initialize();
        }
        else
        {
            Debug.LogError("Monster_Slime_Green_King_Pool 을 찾을 수 없습니다.");
        }



        // 오렌지 슬라임
        monster_Slime_Orange = GetComponentInChildren<Monster_Slime_Orange_Pool>();
        if (monster_Slime_Orange != null)
        {
            Debug.Log("Monster_Slime_Orange_Pool 초기화");
            monster_Slime_Orange.Initialize();
        }
        else
        {
            Debug.LogError("Monster_Slime_Orange 을 찾을 수 없습니다.");
        }

        // 오렌지 슬라임 스톤
        monster_Slime_Orange_Stone = GetComponentInChildren<Monster_Slime_Orange_Stone_Pool>();
        if (monster_Slime_Orange_Stone != null)
        {
            Debug.Log("Monster_Slime_Orange_Stone_Pool 초기화");
            monster_Slime_Orange_Stone.Initialize();
        }
        else
        {
            Debug.LogError("Monster_Slime_Orange_Stone_Pool 을 찾을 수 없습니다.");
        }

        // 오렌지 슬라임 킹
        monster_Slime_Orange_King = GetComponentInChildren<Monster_Slime_Orange_King_Pool>();
        if (monster_Slime_Orange_King != null)
        {
            Debug.Log("Monster_Slime_Orange_King_Pool 초기화");
            monster_Slime_Orange_King.Initialize();
        }
        else
        {
            Debug.LogError("Monster_Slime_Orange_King_Pool 을 찾을 수 없습니다.");
        }
    }

    /// <summary>
    /// 풀에 있는 게임 오브젝트 하나 가져오기
    /// </summary>
    /// <param name="type">가져올 오브젝트의 종류</param>
    /// <param name="position">오브젝트가 배치될 위치</param>
    /// <param name="angle">오브젝트의 초기 각도</param>
    /// <returns>활성화된 오브젝트</returns>
    public GameObject GetObject(PoolObjectType type, Vector3? position = null, Vector3? euler = null)
    {
        GameObject result = null;
        switch (type)
        {
            case PoolObjectType.Monster_Goblin_Green:                                       // 초록 고블린
                result = monster_Goblin_Green.GetObject(position, euler).gameObject;
                break;

            case PoolObjectType.Monster_Goblin_Green_Archer:                                // 초록 고블린 아처
                result = monster_Goblin_Green_Archer.GetObject(position, euler).gameObject;
                break;

            case PoolObjectType.Monster_Goblin_Green_Warrior:                               // 초록 고블린 워리어
                result = monster_Goblin_Green_Warrior.GetObject(position, euler).gameObject;
                break;



            case PoolObjectType.Monster_Goblin_Yellow:                                       // 노랑 고블린
                result = monster_Goblin_Yellow.GetObject(position, euler).gameObject;
                break;

            case PoolObjectType.Monster_Goblin_Yellow_Archer:                                // 노랑 고블린 아처
                result = monster_Goblin_Yellow_Archer.GetObject(position, euler).gameObject;
                break;

            case PoolObjectType.Monster_Goblin_Yellow_Warrior:                               // 노랑 고블린 워리어
                result = monster_Goblin_Yellow_Warrior.GetObject(position, euler).gameObject;
                break;



            case PoolObjectType.Monster_Skull:                                       // 스컬
                result = monster_Skull.GetObject(position, euler).gameObject;
                break;

            case PoolObjectType.Monster_Skull_Archer:                                // 스컬 아처
                result = monster_Skull_Archer.GetObject(position, euler).gameObject;
                break;

            case PoolObjectType.Monster_Skull_Warrior:                               // 스컬 워리어
                result = monster_Skull_Warrior.GetObject(position, euler).gameObject;
                break;


            
            case PoolObjectType.Monster_Skull_Poison:                                       // 스컬 포이즌
                result = monster_Skull_Poison.GetObject(position, euler).gameObject;
                break;

            case PoolObjectType.Monster_Skull_Poison_Archer:                                // 스컬 포이즌 아처
                result = monster_Skull_Poison_Archer.GetObject(position, euler).gameObject;
                break;

            case PoolObjectType.Monster_Skull_Poison_Warrior:                               // 스컬 포이즌 워리어
                result = monster_Skull_Poison_Warrior.GetObject(position, euler).gameObject;
                break;
            
            
            
            case PoolObjectType.Monster_Slime_Green:                                        // 초록 슬라임
                result = monster_Slime_Green.GetObject(position, euler).gameObject;
                break;

            case PoolObjectType.Monster_Slime_Green_Stone:                                  // 초록 슬라임 스톤
                result = monster_Slime_Green_Stone.GetObject(position, euler).gameObject;
                break;

            case PoolObjectType.Monster_Slime_Green_King:                                   // 초록 슬라임 킹
                result = monster_Slime_Green_King.GetObject(position, euler).gameObject;
                break;
            
            
            
            case PoolObjectType.Monster_Slime_Orange:                                        // 오렌지 슬라임
                result = monster_Slime_Orange.GetObject(position, euler).gameObject;
                break;

            case PoolObjectType.Monster_Slime_Orange_Stone:                                  // 오렌지 슬라임 스톤
                result = monster_Slime_Orange_Stone.GetObject(position, euler).gameObject;
                break;

            case PoolObjectType.Monster_Slime_Orange_King:                                   // 오렌지 슬라임 킹
                result = monster_Slime_Orange_King.GetObject(position, euler).gameObject;
                break;
        }

        return result;
    }

    // 초록 고블린 --------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Monster_Goblin_Green 하나 가져오는 함수
    /// </summary>
    /// <returns></returns>
    public Monster_Goblin_Green GetMonster_Goblin_Green()
    {
        return monster_Goblin_Green.GetObject();
    }

    /// <summary>
    /// Monster_Goblin_Green 하나 가져와서 특정 위치에 배치하는 함수
    /// </summary>
    /// <param name="position"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Monster_Goblin_Green GetMonster_Goblin_Green(Vector3 position, float angle = 0.0f)
    {
        return monster_Goblin_Green.GetObject(position, angle * Vector3.forward);
    }

    /// <summary>
    /// Monster_Goblin_Green_Archer 하나 가져오는 함수
    /// </summary>
    /// <returns></returns>
    public Monster_Goblin_Green_Archer GetMonster_Goblin_Green_Archer()
    {
        return monster_Goblin_Green_Archer.GetObject();
    }

    /// <summary>
    /// Monster_Goblin_Green_Archer 하나 가져와서 특정 위치에 배치하는 함수
    /// </summary>
    /// <param name="position"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Monster_Goblin_Green_Archer GetMonster_Goblin_Green_Archer(Vector3 position, float angle = 0.0f)
    {
        return monster_Goblin_Green_Archer.GetObject(position, angle * Vector3.forward);
    }

    /// <summary>
    /// Monster_Goblin_Green_Warrior 하나 가져오는 함수
    /// </summary>
    /// <returns></returns>
    public Monster_Goblin_Green_Warrior GetMonster_Goblin_Green_Warrior()
    {
        return monster_Goblin_Green_Warrior.GetObject();
    }

    /// <summary>
    /// Monster_Goblin_Green_Warrior 하나 가져와서 특정 위치에 배치하는 함수
    /// </summary>
    /// <param name="position"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Monster_Goblin_Green_Warrior GetMonster_Goblin_Green_Warrior(Vector3 position, float angle = 0.0f)
    {
        return monster_Goblin_Green_Warrior.GetObject(position, angle * Vector3.forward);
    }

    // 초록 고블린 끝 --------------------------------------------------------------------------------------------------------------------------



    // 노랑 고블린 --------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Monster_Goblin_Yellow 하나 가져오는 함수
    /// </summary>
    /// <returns></returns>
    public Monster_Goblin_Yellow GetMonster_Goblin_Yellow()
    {
        return monster_Goblin_Yellow.GetObject();
    }

    /// <summary>
    /// Monster_Goblin_Yellow 하나 가져와서 특정 위치에 배치하는 함수
    /// </summary>
    /// <param name="position"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Monster_Goblin_Yellow GetMonster_Goblin_Yellow(Vector3 position, float angle = 0.0f)
    {
        return monster_Goblin_Yellow.GetObject(position, angle * Vector3.forward);
    }

    /// <summary>
    /// Monster_Goblin_Yellow_Archer 하나 가져오는 함수
    /// </summary>
    /// <returns></returns>
    public Monster_Goblin_Yellow_Archer GetMonster_Goblin_Yellow_Archer()
    {
        return monster_Goblin_Yellow_Archer.GetObject();
    }

    /// <summary>
    /// Monster_Goblin_Yellow_Archer 하나 가져와서 특정 위치에 배치하는 함수
    /// </summary>
    /// <param name="position"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Monster_Goblin_Yellow_Archer GetMonster_Goblin_Yellow_Archer(Vector3 position, float angle = 0.0f)
    {
        return monster_Goblin_Yellow_Archer.GetObject(position, angle * Vector3.forward);
    }

    /// <summary>
    /// Monster_Goblin_Yellow_Warrior 하나 가져오는 함수
    /// </summary>
    /// <returns></returns>
    public Monster_Goblin_Yellow_Warrior GetMonster_Goblin_Yellow_Warrior()
    {
        return monster_Goblin_Yellow_Warrior.GetObject();
    }

    /// <summary>
    /// Monster_Goblin_Yellow_Warrior 하나 가져와서 특정 위치에 배치하는 함수
    /// </summary>
    /// <param name="position"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Monster_Goblin_Yellow_Warrior GetMonster_Goblin_Yellow_Warrior(Vector3 position, float angle = 0.0f)
    {
        return monster_Goblin_Yellow_Warrior.GetObject(position, angle * Vector3.forward);
    }

    // 노랑 고블린 끝 --------------------------------------------------------------------------------------------------------------------------



    // 스컬 --------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Monster_Skull 하나 가져오는 함수
    /// </summary>
    /// <returns></returns>
    public Monster_Skull GetMonster_Skull()
    {
        return monster_Skull.GetObject();
    }

    /// <summary>
    /// Monster_Skull 하나 가져와서 특정 위치에 배치하는 함수
    /// </summary>
    /// <param name="position"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Monster_Skull GetMonster_Skull(Vector3 position, float angle = 0.0f)
    {
        return monster_Skull.GetObject(position, angle * Vector3.forward);
    }

    /// <summary>
    /// Monster_Skull_Archer 하나 가져오는 함수
    /// </summary>
    /// <returns></returns>
    public Monster_Skull_Archer GetMonster_Skull_Archer()
    {
        return monster_Skull_Archer.GetObject();
    }

    /// <summary>
    /// Monster_Skull_Archer 하나 가져와서 특정 위치에 배치하는 함수
    /// </summary>
    /// <param name="position"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Monster_Skull_Archer GetMonster_Skull_Archer(Vector3 position, float angle = 0.0f)
    {
        return monster_Skull_Archer.GetObject(position, angle * Vector3.forward);
    }

    /// <summary>
    /// Monster_Skull_Warrior 하나 가져오는 함수
    /// </summary>
    /// <returns></returns>
    public Monster_Skull_Warrior GetMonster_Skull_Warrior()
    {
        return monster_Skull_Warrior.GetObject();
    }

    /// <summary>
    /// Monster_Skull_Warrior 하나 가져와서 특정 위치에 배치하는 함수
    /// </summary>
    /// <param name="position"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Monster_Skull_Warrior GetMonster_Skull_Warrior(Vector3 position, float angle = 0.0f)
    {
        return monster_Skull_Warrior.GetObject(position, angle * Vector3.forward);
    }

    // 스컬 끝 --------------------------------------------------------------------------------------------------------------------------



    // 스컬 포이즌 --------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Monster_Skull_Poison 하나 가져오는 함수
    /// </summary>
    /// <returns></returns>
    public Monster_Skull_Poison GetMonster_Skull_Poison()
    {
        return monster_Skull_Poison.GetObject();
    }

    /// <summary>
    /// Monster_Skull_Poison 하나 가져와서 특정 위치에 배치하는 함수
    /// </summary>
    /// <param name="position"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Monster_Skull_Poison GetMonster_Skull_Poison(Vector3 position, float angle = 0.0f)
    {
        return monster_Skull_Poison.GetObject(position, angle * Vector3.forward);
    }

    /// <summary>
    /// Monster_Skull_Poison_Archer 하나 가져오는 함수
    /// </summary>
    /// <returns></returns>
    public Monster_Skull_Poison_Archer GetMonster_Skull_Poison_Archer()
    {
        return monster_Skull_Poison_Archer.GetObject();
    }

    /// <summary>
    /// Monster_Skull_Poison_Archer 하나 가져와서 특정 위치에 배치하는 함수
    /// </summary>
    /// <param name="position"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Monster_Skull_Poison_Archer GetMonster_Skull_Poison_Archer(Vector3 position, float angle = 0.0f)
    {
        return monster_Skull_Poison_Archer.GetObject(position, angle * Vector3.forward);
    }

    /// <summary>
    /// Monster_Skull_Poison_Warrior 하나 가져오는 함수
    /// </summary>
    /// <returns></returns>
    public Monster_Skull_Poison_Warrior GetMonster_Skull_Poison_Warrior()
    {
        return monster_Skull_Poison_Warrior.GetObject();
    }

    /// <summary>
    /// Monster_Skull_Poison_Warrior 하나 가져와서 특정 위치에 배치하는 함수
    /// </summary>
    /// <param name="position"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Monster_Skull_Poison_Warrior GetMonster_Skull_Poison_Warrior(Vector3 position, float angle = 0.0f)
    {
        return monster_Skull_Poison_Warrior.GetObject(position, angle * Vector3.forward);
    }

    // 스컬 포이즌 끝 --------------------------------------------------------------------------------------------------------------------------



    // 초록 슬라임 --------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Monster_Slime_Green 하나 가져오는 함수
    /// </summary>
    /// <returns></returns>
    public Monster_Slime_Green GetMonster_Slime_Green()
    {
        return monster_Slime_Green.GetObject();
    }

    /// <summary>
    /// Monster_Slime_Green 하나 가져와서 특정 위치에 배치하는 함수
    /// </summary>
    /// <param name="position"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Monster_Slime_Green GetMonster_Slime_Green(Vector3 position, float angle = 0.0f)
    {
        return monster_Slime_Green.GetObject(position, angle * Vector3.forward);
    }

    /// <summary>
    /// Monster_Slime_Green_Stone 하나 가져오는 함수
    /// </summary>
    /// <returns></returns>
    public Monster_Slime_Green_Stone GetMonster_Slime_Green_Stone()
    {
        return monster_Slime_Green_Stone.GetObject();
    }

    /// <summary>
    /// Monster_Slime_Green_Stone 하나 가져와서 특정 위치에 배치하는 함수
    /// </summary>
    /// <param name="position"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Monster_Slime_Green_Stone GetMonster_Slime_Green_Stone(Vector3 position, float angle = 0.0f)
    {
        return monster_Slime_Green_Stone.GetObject(position, angle * Vector3.forward);
    }

    /// <summary>
    /// Monster_Slime_Green_King 하나 가져오는 함수
    /// </summary>
    /// <returns></returns>
    public Monster_Slime_Green_King GetMonster_Slime_Green_King()
    {
        return monster_Slime_Green_King.GetObject();
    }

    /// <summary>
    /// Monster_Slime_Green_King 하나 가져와서 특정 위치에 배치하는 함수
    /// </summary>
    /// <param name="position"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Monster_Slime_Green_King GeMonster_Slime_Green_King(Vector3 position, float angle = 0.0f)
    {
        return monster_Slime_Green_King.GetObject(position, angle * Vector3.forward);
    }

    // 초록 슬라임 끝 --------------------------------------------------------------------------------------------------------------------------
    
    
    
    // 오렌지 슬라임 --------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Monster_Slime_Orange 하나 가져오는 함수
    /// </summary>
    /// <returns></returns>
    public Monster_Slime_Orange GetMonster_Slime_Orange()
    {
        return monster_Slime_Orange.GetObject();
    }

    /// <summary>
    /// Monster_Slime_Orange 하나 가져와서 특정 위치에 배치하는 함수
    /// </summary>
    /// <param name="position"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Monster_Slime_Orange GetMonster_Slime_Orange(Vector3 position, float angle = 0.0f)
    {
        return monster_Slime_Orange.GetObject(position, angle * Vector3.forward);
    }

    /// <summary>
    /// Monster_Slime_Orange_Stone 하나 가져오는 함수
    /// </summary>
    /// <returns></returns>
    public Monster_Slime_Orange_Stone GetMonster_Slime_Orange_Stone()
    {
        return monster_Slime_Orange_Stone.GetObject();
    }

    /// <summary>
    /// Monster_Slime_Orange_Stone 하나 가져와서 특정 위치에 배치하는 함수
    /// </summary>
    /// <param name="position"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Monster_Slime_Orange_Stone GetMonster_Slime_Orange_Stone(Vector3 position, float angle = 0.0f)
    {
        return monster_Slime_Orange_Stone.GetObject(position, angle * Vector3.forward);
    }

    /// <summary>
    /// Monster_Slime_Orange_King 하나 가져오는 함수
    /// </summary>
    /// <returns></returns>
    public Monster_Slime_Orange_King GetMonster_Slime_Orange_King()
    {
        return monster_Slime_Orange_King.GetObject();
    }

    /// <summary>
    /// Monster_Slime_Orange_King 하나 가져와서 특정 위치에 배치하는 함수
    /// </summary>
    /// <param name="position"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Monster_Slime_Orange_King GeMonster_Slime_Orange_King(Vector3 position, float angle = 0.0f)
    {
        return monster_Slime_Orange_King.GetObject(position, angle * Vector3.forward);
    }

    // 오렌지 슬라임 끝 --------------------------------------------------------------------------------------------------------------------------
}
