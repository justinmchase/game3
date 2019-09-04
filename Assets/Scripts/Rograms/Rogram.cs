using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Rogram
{
    public RogramOp[] ops = new RogramOp[0];

    public Rogram(RogramOp[] ops)
    {
        this.ops = ops;
    }
}
