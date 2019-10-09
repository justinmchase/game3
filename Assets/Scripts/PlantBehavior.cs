using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlantBehavior : MonoBehaviour
{
    private bool isWatered = false;
    private BlockBehavior block;

    public Transform Plant;

    void Start()
    {
        this.block = this.GetThingsAt()
            .GetComponents<BlockBehavior>()
            .First();
    }

    void Update()
    {
        if (this.block.IsWet && !this.isWatered)
        {
            this.isWatered = true;
            this.Plant.localScale = new Vector3(.25f, 2, .25f);
        }
    }
}
