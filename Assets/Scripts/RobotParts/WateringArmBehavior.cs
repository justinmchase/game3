using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WateringArmBehavior : MonoBehaviour
{
    private GameManager game;
    private RobotBehavior robot;
    private SubRoutine waterRoutine;

    public float WateringTime = 3.0f;
    private float timeWatered = 0.0f;
    private Action wateringCallback;

    public ParticleSystem waterParticles;
    public Transform emitLocation;

    // Start is called before the first frame update
    void Start()
    {
        this.robot = this.GetComponentInParent<RobotBehavior>();
        this.game = GameObject.FindObjectOfType<GameManager>();

        var chassis = this.GetComponentInParent<ChassisBehavior>();
        var cpu = chassis.GetComponentInChildren<CpuBehavior>();


        this.waterParticles.Pause();

        this.waterRoutine = new SubRoutine
        {
            Name = "water",
            FullName = "arm.water",
            Owner = this.GetComponent<RobotPart>(),
            Fn = this.Water
        };

        cpu.AddSubroutine(this.waterRoutine);

    }

    void Water(int[] args, Action callback)
    {
        this.waterParticles.gameObject.SetActive(true);
        this.waterParticles.Play();
        this.timeWatered = 0.0f;
        this.wateringCallback = callback;

        GameObject.Instantiate(this.waterParticles, this.emitLocation);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.wateringCallback != null)
        {
            this.timeWatered += Time.deltaTime;
            if (this.timeWatered >= this.WateringTime)
            {
                // Mark as watered
                this.robot
                    .GetThingsInFront()
                    .GetComponents<BlockBehavior>()
                    .ForEach(w => w.Water());

                this.wateringCallback();
                this.wateringCallback = null;
            }
        }
    }
}
