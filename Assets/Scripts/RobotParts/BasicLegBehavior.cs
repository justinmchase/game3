using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicLegBehavior : MonoBehaviour, ITickable
{
    const float STEP_SIZE = 2.0f;

    public int Speed;

    private GameManager game;
    private Register register;
    private RobotBehavior robot;

    /* MOVING */
    private int steps;
    private float stepTime;
    private float elapsedTime;
    private Vector3 destination;

    private void Start()
    {
        this.robot = this.GetComponentInParent<RobotBehavior>();
        this.game = GameObject.FindObjectOfType<GameManager>();

        var chassis = this.GetComponentInParent<ChassisBehavior>();
        var cpu = chassis.GetComponentInChildren<CpuBehavior>();

        this.register = new Register
        {
            Name = "leg",
            Owner = this.GetComponent<RobotPart>(),
            Value = 0
        };
        cpu.AddRegister(this.register);

        this.game.Register(this);
        this.destination = robot.transform.position;
    }

    private void Update()
    {
        if (steps > 0)
        {
            this.elapsedTime += Time.deltaTime;
            this.robot.transform.position = Vector3.Lerp(this.robot.transform.position, this.destination, elapsedTime / this.stepTime);
        }
        else
        {
            this.robot.transform.position = this.destination;
        }
    }

    public void Tick()
    {
        if (this.register != null && this.register.Value > 0 && this.steps == 0)
        {
            this.register.Value--;
            this.steps = this.Speed;
        }

        if (this.steps > 0)
        {
            // How many steps are left
            this.steps--;

            // How much time a single step should take
            this.stepTime = 1.0f / this.game.TicksPerSecond;

            // How much time has elapsed taking this step
            this.elapsedTime = 0.0f;

            this.destination = robot.transform.position + robot.transform.forward * (STEP_SIZE / this.Speed);
        }
    }
}
