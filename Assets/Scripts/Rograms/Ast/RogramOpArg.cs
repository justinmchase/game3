using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RogramArgKind
{
    Literal,
    Register
}

public interface IRogramOpArg
{
    RogramArgKind Kind { get; }
}

public struct RogramValueArg : IRogramOpArg
{
    public RogramArgKind Kind { get { return RogramArgKind.Literal; } }
    public int Value;
}

public struct RogramRegisterArg : IRogramOpArg
{
    public RogramArgKind Kind { get { return RogramArgKind.Register; } }
    public string Register;
}

