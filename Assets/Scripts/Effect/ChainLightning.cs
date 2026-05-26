using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : RecycleObject
{
    /// <summary>
    /// 이 이펙트의 수명
    /// </summary>
    public float lifeTime = 0.5f;

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(LifeOver(lifeTime));
    }
}
