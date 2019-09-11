using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicLegBehavior : MonoBehaviour
{
    const float STEP_SIZE = 2.0f;

    private Register register;

    private void Start()
    {
        var chassis = this.GetComponentInParent<ChassisBehavior>();
        var cpu = chassis.GetComponentInChildren<CpuBehavior>();

        this.register = new Register
        {
            Name = "leg",
            Owner = this.GetComponent<RobotPart>(),
            Value = 0
        };
        cpu.AddRegister(this.register);
    }

    private void Update()
    {
        if (this.register != null && this.register.Value > 0)
        {
            var robot = this.GetComponentInParent<RobotBehavior>();
            robot.transform.position = robot.transform.position + robot.transform.forward * STEP_SIZE;
            this.register.Value -= 1;
        }
    }
}
