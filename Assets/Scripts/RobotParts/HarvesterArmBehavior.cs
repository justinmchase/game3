using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HarvesterArmBehavior : MonoBehaviour
{
    private GameManager game;
    private RobotBehavior robot;
    private Animator animator;
    private SubRoutine harvestRoutine;
    private Action finishedCallback;

    // Start is called before the first frame update
    void Start()
    {

        this.robot = this.GetComponentInParent<RobotBehavior>();
        this.game = GameObject.FindObjectOfType<GameManager>();
        this.animator = this.GetComponent<Animator>();

        var chassis = this.GetComponentInParent<ChassisBehavior>();
        var cpu = chassis.GetComponentInChildren<CpuBehavior>();

        this.harvestRoutine = new SubRoutine
        {
            Name = "harvest",
            FullName = "arm.harvest",
            Owner = this.GetComponent<RobotPart>(),
            Fn = this.Harvest
        };

        cpu.AddSubroutine(this.harvestRoutine);



    }

    private void Harvest(int[] arg1, Action finished)
    {
        this.finishedCallback = finished;
        this.GetComponent<Animator>().SetTrigger("StartSpinning");
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }


    public void DoneSpinning()
    {
        this.robot
            .GetThingsInFront()
            .GetComponents<PlantBehavior>()
            .ForEach(w => w.Harvest());

        this.finishedCallback();
    }
}
