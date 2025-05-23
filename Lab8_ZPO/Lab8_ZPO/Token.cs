﻿using System;

public enum TokenType
{
    Number,
    Function,

    Pi,

    LeftParenthesis,
    RightParenthesis,

    Plus,
    Minus,
    Multiply,
    Divide,
    Power
}

public class Token
{
    public TokenType Type { get; }
    public string Value { get; }

    public Token(TokenType type, string value)
    {
        Type = type;
        Value = value;
    }

    public override string ToString() => $"{Type}: {Value}";
}