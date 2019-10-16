using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlantBehavior : MonoBehaviour
{
    private BlockBehavior block;

    public Transform Plant;
    public GameObject HarvestItem;

    public bool IsRipe = false;

    void Start()
    {
        this.block = this.GetThingsAt()
            .GetComponents<BlockBehavior>()
            .First();
    }

    void Update()
    {
        if (this.block.IsWet)
        {
            this.StartCoroutine(this.Grow());
        }
    }

    public IEnumerator Grow()
    {
        yield return new WaitForSeconds(2);
        this.Plant.localScale = new Vector3(.25f, 2, .25f);
        this.block.AbsorbWater();
        this.IsRipe = true;
    }

    public void Harvest()
    {
        if (this.IsRipe)
        {
            var item = GameObject.Instantiate(this.HarvestItem, this.transform.parent);
            item.transform.position = this.transform.position;
        }
        Destroy(this.gameObject);

    }
}
