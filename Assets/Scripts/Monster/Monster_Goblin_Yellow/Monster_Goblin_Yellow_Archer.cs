using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Goblin_Yellow_Archer : RecycleObject
{
    /// <summary>
    /// 이 이펙트의 수명
    /// </summary>
    //public float lifeTime = 3.0f;

    protected override void OnEnable()
    {
        base.OnEnable();
        //StartCoroutine(LifeOver(lifeTime));
    }
}
