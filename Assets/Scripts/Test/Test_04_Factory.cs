using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_04_Factory : TestBase
{
    GameManager gameManager;
    Factory factory;

    public Vector3 spawnPos = Vector3.zero;

    private void Start()
    {
        gameManager = GameManager.Instance;
        factory = Factory.Instance;
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        factory.GetBall(spawnPos, 0f);
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        factory.GeMonster_Slime_Orange_King(spawnPos, 0f);
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        factory.GetBall_Yellow(spawnPos, 0f);
    }
}
