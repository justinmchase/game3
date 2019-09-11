﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RogramOpName
{
    noop,
    wait,
    mov,
    add,
    sub,
    mul,
    div,
}

public interface IRogramOp
{
    RogramOpName Name { get; }
}

public struct NoopOp : IRogramOp
{
    public RogramOpName Name { get { return RogramOpName.noop; } }
}

public struct MovOp : IRogramOp
{
    public RogramOpName Name { get { return RogramOpName.mov; } }

    public IRogramOpArg Src;
    public RogramRegisterArg Dest;
}

public struct WaitOp : IRogramOp
{
    public RogramOpName Name {  get { return RogramOpName.wait; } }
    public IRogramOpArg Time;
}
