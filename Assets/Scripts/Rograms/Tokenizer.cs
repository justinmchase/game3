using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;

public enum TokenKind
{
    Whitespace,
    NewLine,
    Identifier,
    Number,
    Special
}

[DebuggerDisplay("{Kind}:{Value}")]
public struct Token
{
    public TokenKind Kind;
    public string Value;
}

class Tokenizer
{
    private Regex WhitespaceCharacter = new Regex("\\s");
    private Regex NumberCharacter = new Regex("[0-9]");
    private Regex LetterCharacter = new Regex("[a-zA-Z]");

    enum State
    {
        Scan,
        Whitespace,
        Identifier,
        Number
    }

    public Token[] Tokenize(string code)
    {
        var tokens = new List<Token>();
        var state = new Stack<State>();

        state.Push(State.Scan);
        var value = "";
        for (int i = 0, n = code.Length; i < n; i++)
        {
            var c = code[i].ToString();
            var s = state.Peek();
            switch (s)
            {
                case State.Scan:
                    if (c == "\n")
                    {
                        tokens.Add(new Token { Kind = TokenKind.NewLine, Value = "\n" });
                    }
                    else if (WhitespaceCharacter.IsMatch(c))
                    {
                        value = c;
                        state.Push(State.Whitespace);
                    }
                    else if (LetterCharacter.IsMatch(c))
                    {
                        value = c;
                        state.Push(State.Identifier);
                    }
                    else if (NumberCharacter.IsMatch(c))
                    {
                        value = c;
                        state.Push(State.Number);
                    }
                    else
                    {
                        tokens.Add(new Token { Kind = TokenKind.Special, Value = c });
                    }
                    break;
                case State.Whitespace:
                    if (c == "\n")
                    {
                        tokens.Add(new Token { Kind = TokenKind.Whitespace, Value = value });
                        tokens.Add(new Token { Kind = TokenKind.NewLine, Value = "\n" });
                        state.Pop();
                    }
                    else if (WhitespaceCharacter.IsMatch(c))
                    {
                        value += c;
                    }
                    else if (LetterCharacter.IsMatch(c))
                    {
                        tokens.Add(new Token { Kind = TokenKind.Whitespace, Value = value });
                        value = c;
                        state.Pop();
                        state.Push(State.Identifier);
                    }
                    else if (NumberCharacter.IsMatch(c))
                    {
                        tokens.Add(new Token { Kind = TokenKind.Whitespace, Value = value });
                        value = c;
                        state.Pop();
                        state.Push(State.Number);
                    }
                    else
                    {
                        tokens.Add(new Token { Kind = TokenKind.Whitespace, Value = value });
                        tokens.Add(new Token { Kind = TokenKind.Special, Value = c });
                        state.Pop();
                    }
                    break;
                case State.Identifier:
                    if (c == "\n")
                    {
                        tokens.Add(new Token { Kind = TokenKind.Identifier, Value = value });
                        tokens.Add(new Token { Kind = TokenKind.NewLine, Value = "\n" });
                        state.Pop();
                    }
                    else if (WhitespaceCharacter.IsMatch(c))
                    {
                        tokens.Add(new Token { Kind = TokenKind.Identifier, Value = value });
                        value = c;
                        state.Pop();
                        state.Push(State.Whitespace);
                    }
                    else if (LetterCharacter.IsMatch(c) || NumberCharacter.IsMatch(c))
                    {
                        value += c;
                    }
                    else
                    {
                        tokens.Add(new Token { Kind = TokenKind.Identifier, Value = value });
                        tokens.Add(new Token { Kind = TokenKind.Special, Value = c });
                        state.Pop();
                    }
                    break;
                case State.Number:
                    if (c == "\n")
                    {
                        tokens.Add(new Token { Kind = TokenKind.Number, Value = value });
                        tokens.Add(new Token { Kind = TokenKind.NewLine, Value = "\n" });
                        state.Pop();
                    }
                    else if (WhitespaceCharacter.IsMatch(c))
                    {
                        tokens.Add(new Token { Kind = TokenKind.Number, Value = value });
                        value = c;
                        state.Pop();
                        state.Push(State.Whitespace);
                    }
                    else if (LetterCharacter.IsMatch(c))
                    {
                        tokens.Add(new Token { Kind = TokenKind.Number, Value = value });
                        value = c;
                        state.Pop();
                        state.Push(State.Identifier);
                    }
                    else if (NumberCharacter.IsMatch(c))
                    {
                        value += c;
                    }
                    else
                    {
                        tokens.Add(new Token { Kind = TokenKind.Number, Value = value });
                        tokens.Add(new Token { Kind = TokenKind.Special, Value = c });
                        state.Pop();
                    }
                    break;
                default:
                    throw new NotImplementedException($"Tokenizer State {state} at index {i} not implemented.");
            }
        }

        switch (state.Peek())
        {
            case State.Whitespace:
                tokens.Add(new Token { Kind = TokenKind.Whitespace, Value = value });
                break;
            case State.Identifier:
                tokens.Add(new Token { Kind = TokenKind.Identifier, Value = value });
                break;
            case State.Number:
                tokens.Add(new Token { Kind = TokenKind.Number, Value = value });
                break;
        }

        return tokens.ToArray();
    }
}
