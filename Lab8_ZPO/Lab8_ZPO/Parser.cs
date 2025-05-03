using Lab8_ZPO.Expressions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _position = 0;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public IExpression ParseExpression()
    {
        return ParseAddSubstract();
    }

    private IExpression ParseAddSubstract()
    {
        var expr = ParseMultiplyDivide();

        while (Match(TokenType.Plus, TokenType.Minus))
        {
            var op = Previous().Type;
            var right = ParseMultiplyDivide();
            expr = new BinaryExpression(expr, right, op);
        }

        return expr;
    }

    private IExpression ParseMultiplyDivide()
    {
        var expr = ParsePower();

        while (Match(TokenType.Multiply, TokenType.Divide))
        {
            var op = Previous().Type;
            var right = ParsePower();
            expr = new BinaryExpression(expr, right, op);
        }

        return expr;
    }

    private IExpression ParsePower()
    {
        var expr = ParseUnary();

        while (Match(TokenType.Power))
        {
            var op = Previous().Type;
            var right = ParseUnary();
            expr = new BinaryExpression(expr, right, op);
        }

        return expr;
    }

    private IExpression ParseUnary()
    {
        if (Match(TokenType.Plus, TokenType.Minus))
        {
            var op = Previous().Type;
            var right = ParseUnary();
            return new UnaryExpression(op, right);
        }

        return ParsePrimary();
    }

    private IExpression ParsePrimary()
    {
        if (Match(TokenType.Number))
        {
            return new NumberExpression(double.Parse(Previous().Value));
        }

        if (Match(TokenType.LeftParenthesis))
        {
            var expr = ParseExpression();
            Consume(TokenType.RightParenthesis, "Expected ')'");
            return expr;
        }

        if (Match(TokenType.Function))
        {
            var function = Previous().Value;
            Consume(TokenType.LeftParenthesis, "Expected '('");
            var argument = ParseExpression();
            Consume(TokenType.RightParenthesis, "Expected ')'");
            return new FunctionExpression(function, argument);
        }

        throw new Exception("Unexpected token: " + Previous());
    }

    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd()) _position++;
        return Previous();
    }

    private bool IsAtEnd()
    {
        return _position >= _tokens.Count;
    }

    private Token Peek()
    {
        return _tokens[_position];
    }

    private Token Previous()
    {
        return _tokens[_position - 1];
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();
        throw new Exception(message);
    }
}