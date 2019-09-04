using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RogramOp
{
    noop,
    mov,
    reg
}

// mov 1 m0
// mov m0 legs

enum ParseState
{
    Op
}


public class RogramCompiler
{
    public Rogram Compile(string code)
    {
        var ops = new List<RogramOp>();
        this.Parse(code, ops);
        return new Rogram(ops.ToArray());
    }

    private void Parse(string code, List<RogramOp> ops)
    {
        var state = ParseState.Op;
        var op = "";
        for (int i = 0, n = code.Length; i < n; i++)
        {
            var c = code[i];
            switch (state)
            {
                case ParseState.Op:
                    Debug.Log(c);
                    if (c == '\n')
                    {
                        Debug.Log($"NEWLINE {op}");
                        ops.Add(this.ParseOp(op));
                        op = "";
                    }
                    else
                    {
                        Debug.Log($"CHAR {c}");
                        op += c;
                    }
                    break;
                default:
                    Debug.Log("Invalid Program.");
                    return;
            }
        }

        ops.Add(this.ParseOp(op));
    }

    private RogramOp ParseOp(string op)
    {
        switch (op)
        {
            case "mov":
                return RogramOp.mov;
            default:
                Debug.Log($"Invalid op: {op}");
                return RogramOp.noop;
        }
    }
}
