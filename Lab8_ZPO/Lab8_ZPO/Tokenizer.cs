using System;
using System.Text;

public class Tokenizer
{
    private static readonly HashSet<string> Functions = new() { "sin", "cos", "tan", "sqrt", "log" };

    public List<Token> Tokenize(string input)
	{
        List<Token> tokens = new();
        int i = 0;

        while (i < input.Length)
        {
            char c = input[i];

            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            // UŻYCIE PRZECINKA NIE DZIAŁA, wywala exception
            // liczby i przecinek
            if (char.IsDigit(c) || c == ',')
            {
                StringBuilder number = new();
                while (i < input.Length && (char.IsDigit(input[i]) || input[i] == ','))
                {
                    number.Append(input[i]);
                    i++;
                }
                tokens.Add(new Token(TokenType.Number, number.ToString().Replace(",", ".")));
                continue;
            }

            // operator
            if ("+-*/^".Contains(c))
            {
                switch (c)
                {
                    case '+':
                        tokens.Add(new Token(TokenType.Plus, c.ToString()));
                        break;
                    case '-':
                        tokens.Add(new Token(TokenType.Minus, c.ToString()));
                        break;
                    case '*':
                        tokens.Add(new Token(TokenType.Multiply, c.ToString()));
                        break;
                    case '/':
                        tokens.Add(new Token(TokenType.Divide, c.ToString()));
                        break;
                    case '^':
                        tokens.Add(new Token(TokenType.Power, c.ToString()));
                        break;
                }
                i++;
                continue;
            }

            // nawiasy
            if (c == '(')
            {
                tokens.Add(new Token(TokenType.LeftParenthesis, "("));
                i++;
                continue;
            }
            if (c == ')')
            {
                tokens.Add(new Token(TokenType.RightParenthesis, ")"));
                i++;
                continue;
            }

            // pi
            if (c == 'π')
            {
                tokens.Add(new Token(TokenType.Pi, Math.PI.ToString()));
                i++;
                continue;
            }

            // funkcje
            bool foundFunction = false;
            foreach (string func in Functions)
            {
                if (input.Substring(i).StartsWith(func))
                {
                    tokens.Add(new Token(TokenType.Function, func));
                    i += func.Length;
                    foundFunction = true;
                    break;
                }
            }
            if (foundFunction) continue;

            throw new Exception ($"Nieznany symbol w pozycji {i}: '{c}'");
        }

        return tokens;
    }
}