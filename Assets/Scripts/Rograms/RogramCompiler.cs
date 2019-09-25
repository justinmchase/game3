using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System;


// mov 1 m0
// mov m0 legs

public class RogramCompiler
{
    private Regex NumberCharacter = new Regex("[0-9]");
    private Regex LetterCharacter = new Regex("[a-z]");
    private Regex JoinCharacter = new Regex("[._]");
    private Tokenizer tokenizer = new Tokenizer();

    public Rogram Compile(string code)
    {
        var ops = new List<IRogramOp>();
        this.Parse(code, ops);
        return new Rogram(ops.ToArray());
    }

    enum State
    {
        Op,
        Arg,
        Identifier,
        Comment,
        Error,
    }

    private void Parse(string code, List<IRogramOp> ops)
    {
        var tokens = this.tokenizer
            .Tokenize(code)
            .Where(t => t.Kind != TokenKind.Whitespace)
            .ToArray();

        var state = new Stack<State>();

        state.Push(State.Op);
        var name = "";
        var identifier = "";
        var args = new List<IRogramOpArg>();

        for (int i = 0, n = tokens.Length; i < n; i++)
        {
            var t = tokens[i];
            var s = state.Peek();
            switch (s)
            {
                case State.Op:
                    if (t.Kind == TokenKind.Identifier)
                    {
                        name = t.Value;
                        state.Push(State.Arg);
                    }
                    else if (t.Kind == TokenKind.Special && t.Value == "#")
                    {
                        state.Push(State.Comment);
                    }
                    else
                    {
                        state.Push(State.Error);
                    }
                    break;
                case State.Comment:
                    if (t.Kind == TokenKind.NewLine)
                    {
                        state.Pop();
                    }
                    break;
                case State.Arg:
                    if (t.Kind == TokenKind.Identifier)
                    {
                        identifier = t.Value;
                        state.Push(State.Identifier);
                    }
                    else if (t.Kind == TokenKind.Number)
                    {
                        args.Add(new RogramValueArg { Value = int.Parse(t.Value) });
                    }
                    else if (t.Kind == TokenKind.NewLine)
                    {
                        ops.Add(this.ParseOp(name, args));
                        args.Clear();
                        name = "";
                        state.Pop();
                    }
                    else if (t.Kind == TokenKind.Special && t.Value == "#")
                    {
                        ops.Add(this.ParseOp(name, args));
                        args.Clear();
                        name = "";
                        state.Pop();
                        state.Push(State.Comment);
                    }
                    break;
                case State.Identifier:
                    if ((t.Kind == TokenKind.Special && this.JoinCharacter.IsMatch(t.Value)) || t.Kind == TokenKind.Identifier)
                    {
                        identifier += t.Value;
                    }
                    else if (t.Kind == TokenKind.NewLine)
                    {
                        args.Add(new RogramRegisterArg { Register = identifier });
                        ops.Add(this.ParseOp(name, args));
                        args.Clear();
                        name = "";
                        identifier = "";
                        state.Pop();
                        state.Pop();
                    }
                    else if (t.Kind == TokenKind.Special && t.Value == "#")
                    {
                        args.Add(new RogramRegisterArg { Register = identifier });
                        ops.Add(this.ParseOp(name, args));
                        args.Clear();
                        name = "";
                        identifier = "";
                        state.Pop();
                        state.Pop();
                        state.Push(State.Comment);
                    }
                    else if (t.Kind == TokenKind.Number)
                    {
                        args.Add(new RogramRegisterArg { Register = identifier });
                        args.Add(new RogramValueArg { Value = int.Parse(t.Value) });
                        identifier = "";
                        state.Pop();
                    }
                    break;
                case State.Error:
                    if (t.Kind == TokenKind.NewLine)
                    {
                        state.Pop();
                        args.Clear();
                        name = "";
                        identifier = "";
                    }
                    // else skip for now...
                    break;
                default:
                    Debug.Log("Invalid Program.");
                    return;
            }
        }

        switch (state.Peek())
        {
            case State.Arg:
            case State.Identifier:
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
            case "call":
                return new CallOp
                {
                    Sub = (RogramRegisterArg)args[0],
                    Args = args.Skip(1).ToArray()
                };
            default:
                Debug.Log($"Invalid op: {name}");
                return new NoopOp();
        }
    }
}
