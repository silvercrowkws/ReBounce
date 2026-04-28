using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_03_Ball : TestBase
{
    public GameObject[] monsters;
    private Vector3[] monsterPos = { new (-0.31f,0.033f, -0.31f), new(0f, 0.033f, -0.31f), new(0.31f, 0.033f, -0.31f) };

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        Time.timeScale = 0.1f;
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        Time.timeScale = 1;
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {

        for(int i = 0; i < monsters.Length; i++)
        {
            monsters[i].gameObject.transform.position = monsterPos[i];
        }
    }
}
