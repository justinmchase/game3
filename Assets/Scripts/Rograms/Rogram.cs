using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Rogram
{
    public IRogramOp[] ops = new IRogramOp[0];

    public Rogram(IRogramOp[] ops)
    {
        this.ops = ops;
    }
}
