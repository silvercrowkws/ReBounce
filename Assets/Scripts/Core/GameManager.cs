using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Vector3 firstGroundHitPos { get; private set; }
    public bool hasFirstGroundHit { get; private set; }

    /// <summary>
    /// 첫번째로 땅에 닿은 위치를 알리는 델리게이트
    /// </summary>
    public Action<Vector3> onFirstGroundHitPos;

    public void ResetRound()
    {
        hasFirstGroundHit = false;
        firstGroundHitPos = Vector3.zero;
    }


    public void RegisterFirstGroundHit(Vector3 pos)
    {
        if (hasFirstGroundHit) return;

        hasFirstGroundHit = true;
        firstGroundHitPos = pos;

        Debug.Log("첫 바닥 충돌 위치: " + firstGroundHitPos);
        onFirstGroundHitPos?.Invoke(firstGroundHitPos);
    }
}
