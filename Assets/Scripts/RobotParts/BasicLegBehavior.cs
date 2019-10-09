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
    private SubRoutine rotateLeft;
    private SubRoutine rotateRight;

    /* MOVING */

    private float worldSpeed = 0f;
    private Vector3 destination;
    private Quaternion targetRotation;

    private Action FinishedCallback;
    private SubRoutine runningRoutine;

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

        this.rotateLeft = new SubRoutine
        {
            Name = "rotate_left",
            FullName = "leg.rotate_left",
            Owner = this.GetComponent<RobotPart>(),
            Fn = this.RotateLeft
        };

        this.rotateRight = new SubRoutine
        {
            Name = "rotate_right",
            FullName = "leg.rotate_right",
            Owner = this.GetComponent<RobotPart>(),
            Fn = this.RotateRight
        };

        cpu.AddSubroutine(this.moveForward);
        cpu.AddSubroutine(this.rotateLeft);
        cpu.AddSubroutine(this.rotateRight);

        this.destination = robot.transform.position;
    }

    private void MoveForward(int[] args, Action callback)
    {
        this.runningRoutine = this.moveForward;
        this.FinishedCallback = callback;
        int dist = args.Length == 1 ? args[0] : 0;
        this.destination = robot.transform.position + robot.transform.forward * STEP_SIZE * dist;
        this.worldSpeed = this.TickSpeedToWorldSpeed(this.Speed);
    }

    private void RotateLeft(int[] args, Action callback)
    {
        this.runningRoutine = this.rotateLeft;
        this.FinishedCallback = callback;
        int dist = args.Length == 1 ? args[0] : 0;
        this.targetRotation = robot.transform.rotation * Quaternion.Euler(0, -90, 0);
        this.worldSpeed = this.TickSpeedToWorldSpeed(this.Speed);
    }

    private void RotateRight(int[] args, Action callback)
    {
        this.runningRoutine = this.rotateRight;
        this.FinishedCallback = callback;
        int dist = args.Length == 1 ? args[0] : 0;
        this.targetRotation = robot.transform.rotation * Quaternion.Euler(0, 90, 0);
        this.worldSpeed = this.TickSpeedToWorldSpeed(this.Speed);
    }

    private void Update()
    {
        if(this.worldSpeed < 0.0001f || this.runningRoutine == null)
        {
            return;
        }

        if (this.runningRoutine == this.moveForward)
        {
            this.robot.transform.position = this.robot.transform.position + this.robot.transform.forward * this.worldSpeed * Time.deltaTime;
            var dirToDest = this.destination - this.robot.transform.position;
            var dot = Vector3.Dot(dirToDest, this.transform.forward);
            if (dot < 0)
            {
                this.robot.transform.position = this.destination;
                this.FinishedCallback();
                this.FinishedCallback = null;
                this.runningRoutine = null;
            }
        }

        if (this.runningRoutine == this.rotateLeft || this.runningRoutine == this.rotateRight)
        {
            this.robot.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, this.targetRotation, 1f);
            var dot = Quaternion.Dot(this.transform.rotation, this.targetRotation);
            if (dot > 0.9999f)
            {
                this.robot.transform.rotation = this.targetRotation;
                this.FinishedCallback();
                this.FinishedCallback = null;
                this.runningRoutine = null;
            }
        }
    }

    private float TickSpeedToWorldSpeed(float tickSpeed)
    {
        return (1.0f / tickSpeed) * this.game.TicksPerSecond *  STEP_SIZE;
    }

}
