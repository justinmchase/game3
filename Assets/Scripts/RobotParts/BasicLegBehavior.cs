using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BasicLegBehavior : MonoBehaviour
{
    const float STEP_SIZE = 2.0f;

    public int Speed;

    private GameManager game;
    private RobotBehavior robot;
    private SubRoutine moveForward;

    /* MOVING */

    private float worldSpeed = 0f;
    private Vector3 destination;

    private Action MoveForwardCallback;

    private void Start()
    {
        this.robot = this.GetComponentInParent<RobotBehavior>();
        this.game = GameObject.FindObjectOfType<GameManager>();

        var chassis = this.GetComponentInParent<ChassisBehavior>();
        var cpu = chassis.GetComponentInChildren<CpuBehavior>();

        this.moveForward = new SubRoutine
        {
            Name = "move_forward",
            FullName = "leg.move_forward",
            Owner = this.GetComponent<RobotPart>(),
            Fn = this.MoveForward
        };

        cpu.AddSubroutine(this.moveForward);

        this.destination = robot.transform.position;
    }

    private void MoveForward(int[] args, Action callback)
    {
        this.MoveForwardCallback = callback;
        int dist = args.Length == 1 ? args[0] : 0;
        this.destination = robot.transform.position + robot.transform.forward * STEP_SIZE * dist;
        this.worldSpeed = this.TickSpeedToWorldSpeed(this.Speed);
    }

    private void Update()
    {
        if(this.worldSpeed < 0.0001f || this.MoveForwardCallback == null)
        {
            return;
        }

        this.robot.transform.position = this.robot.transform.position + this.robot.transform.forward * this.worldSpeed * Time.deltaTime;
        var dirToDest = this.destination - this.robot.transform.position;
        var dot = Vector3.Dot(dirToDest, this.transform.forward);
        if(dot < 0)
        {
            this.robot.transform.position = this.destination;
            this.MoveForwardCallback();
            this.MoveForwardCallback = null;
        }
    }

    private float TickSpeedToWorldSpeed(float tickSpeed)
    {
        return (1.0f / tickSpeed) * this.game.TicksPerSecond *  STEP_SIZE;
    }

}
