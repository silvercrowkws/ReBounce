using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_03_Ball : TestBase
{
    public GameObject[] monsters;
    private Vector3[] monsterPos = { new (-0.31f,0.033f, -0.31f), new(0f, 0.033f, -0.31f), new(0.31f, 0.033f, -0.31f), 
        new(0f, 0.033f, 0.31f), new(0.61f, 0.033f, -0.31f),
        new(0.31f, 0.033f, -0.62f)};

    BallShooter ballShooter;

    private void Start()
    {
        ballShooter = FindAnyObjectByType<BallShooter>();
    }

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

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        //ballShooter.shootBalls.Insert(0, BallElementals.Water);
        ballShooter.shootBalls.Clear();

        ballShooter.shootBalls.Add(BallElementals.Fire);
        ballShooter.shootBalls.Add(BallElementals.Land);
        ballShooter.shootBalls.Add(BallElementals.Electric);
        ballShooter.shootBalls.Add(BallElementals.Water);
        ballShooter.shootBalls.Add(BallElementals.Wind);
        ballShooter.shootBalls.Add(BallElementals.Normal);

        //ballShooter.shootCount = ballShooter.shootBalls.Count;
    }

    protected override void OnTest5(InputAction.CallbackContext context)
    {
        
    }
}
