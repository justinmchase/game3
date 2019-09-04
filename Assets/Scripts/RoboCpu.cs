using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoboCpu: MonoBehaviour
{
    const float STEP_SIZE = 2.0f;

    public Rogram rogram;

    private int index = 0;

    // Update is called once per frame
    void Update()
    {
        if (this.rogram != null && this.index < rogram.ops.Length)
        {
            var op = rogram.ops[this.index];
            switch (op)
            {
                case RogramOp.noop:
                    break;
                case RogramOp.mov:
                    this.transform.position = this.transform.position + (STEP_SIZE * this.transform.forward);
                    break;
                default:
                    Debug.Log($"Invalid Op {op}");
                    break;
            }

            this.index++;
        }
        
    }

    public void Load(Rogram rogram)
    {
        this.index = 0;
        this.rogram = rogram;

        Debug.Log("Rogram loaded!");
    }
}
