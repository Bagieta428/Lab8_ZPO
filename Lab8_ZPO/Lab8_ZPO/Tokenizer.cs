using System;
using System.Text;

public class Tokenizer
{
    private static readonly HashSet<string> Operators = new() { "+", "-", "*", "/", "^" };
    private static readonly HashSet<string> Functions = new() { "sin", "cos", "tan", "sqrt", "log" };
    private static readonly Dictionary<string, string> Constants = new()
    {
        { "π", Math.PI.ToString() }
    };

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

            // liczba lub przecinek
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
            if (Operators.Contains(c.ToString()))
            {
                tokens.Add(new Token(TokenType.Operator, c.ToString()));
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

            // stałe
            if (Constants.ContainsKey(c.ToString()))
            {
                tokens.Add(new Token(TokenType.Constant, Constants[c.ToString()]));
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