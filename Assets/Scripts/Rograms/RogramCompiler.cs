using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;


// mov 1 m0
// mov m0 legs

enum ParseState
{
    OpName,
    Arg,
    Register,
    Literal
}


public class RogramCompiler
{
    private Regex NumberCharacter = new Regex("[0-9]");
    private Regex LetterCharacter = new Regex("[a-z]");

    public Rogram Compile(string code)
    {
        var ops = new List<IRogramOp>();
        this.Parse(code, ops);
        return new Rogram(ops.ToArray());
    }

    private void Parse(string code, List<IRogramOp> ops)
    {
        var state = new Stack<ParseState>();

        state.Push(ParseState.OpName);
        var name = "";
        var value = "";
        var args = new List<IRogramOpArg>();
        for (int i = 0, n = code.Length; i < n; i++)
        {
            var c = code[i];
            var s = state.Peek();
            switch (s)
            {
                case ParseState.OpName:
                    if (c == ' ')
                    {
                        state.Push(ParseState.Arg);
                    }
                    else
                    {
                        name += c;
                    }
                    break;
                case ParseState.Arg:
                    if (this.LetterCharacter.IsMatch(c.ToString()))
                    {
                        state.Push(ParseState.Register);
                        value += c;
                    }
                    else if (this.NumberCharacter.IsMatch(c.ToString()))
                    {
                        state.Push(ParseState.Literal);
                        value += c;
                    }
                    break;
                case ParseState.Register:
                    if (this.LetterCharacter.IsMatch(c.ToString()))
                    {
                        value += c;
                    }
                    else if (c == ' ')
                    {
                        state.Pop();
                        args.Add(new RogramRegisterArg { Register = value });
                        value = "";
                    }
                    else if (c == '\n')
                    {
                        state.Pop();
                        state.Pop();
                        args.Add(new RogramRegisterArg { Register = value });
                        if (state.Peek() == ParseState.OpName)
                        {
                            ops.Add(this.ParseOp(name, args));
                            name = "";
                            args.Clear();
                        }
                        value = "";
                    }
                    break;
                case ParseState.Literal:
                    if (this.NumberCharacter.IsMatch(c.ToString()))
                    {
                        value += c;
                    }
                    else if (c == ' ')
                    {
                        state.Pop();
                        args.Add(new RogramValueArg { Value = int.Parse(value) });
                        value = "";
                    }
                    else if (c == '\n')
                    {
                        state.Pop();
                        state.Pop();
                        args.Add(new RogramValueArg { Value = int.Parse(value) });
                        if (state.Peek() == ParseState.OpName)
                        {
                            ops.Add(this.ParseOp(name, args));
                            name = "";
                            args.Clear();
                        }
                        value = "";
                    }
                    break;
                default:
                    Debug.Log("Invalid Program.");
                    return;
            }
        }

        switch (state.Peek())
        {
            case ParseState.Register:
                args.Add(new RogramRegisterArg { Register = value });
                ops.Add(this.ParseOp(name, args));
                break;
            case ParseState.Literal:
                args.Add(new RogramValueArg { Value = int.Parse(value) });
                ops.Add(this.ParseOp(name, args));
                break;
        }
    }

    private IRogramOp ParseOp(string name, List<IRogramOpArg> args)
    {
        switch (name)
        {
            case "mov":
                return new MovOp {
                    Src = args[0],
                    Dest = (RogramRegisterArg)args[1]
                };
            case "wait":
                return new WaitOp
                {
                    Time = args[0]
                };
            default:
                Debug.Log($"Invalid op: {name}");
                return new NoopOp();
        }
    }
}
