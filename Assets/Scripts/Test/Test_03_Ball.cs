using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_03_Ball : TestBase
{
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        Time.timeScale = 0.1f;
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        Time.timeScale = 1;
    }
}
