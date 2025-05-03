using System;

namespace Lab8_ZPO.Expressions
{
    public interface IExpression
    {
        double Evaluate();
    }

    public class NumberExpression : IExpression
    {
        public double Value { get; }

        public NumberExpression(double value)
        {
            Value = value;
        }

        public double Evaluate() => Value;
    }

    public class BinaryExpression : IExpression
    {
        public IExpression Left { get; }
        public IExpression Right { get; }
        public TokenType Operator { get; }

        public BinaryExpression(IExpression left, IExpression right, TokenType op)
        {
            Left = left;
            Right = right;
            Operator = op;
        }

        public double Evaluate()
        {
            var l = Left.Evaluate();
            var r = Right.Evaluate();

            return Operator switch
            {
                TokenType.Plus => l + r,
                TokenType.Minus => l - r,
                TokenType.Multiply => l * r,
                TokenType.Divide => l / r,
                TokenType.Power => Math.Pow(l, r),
                _ => throw new Exception("Invalid operator")
            };
        }
    }

    public class UnaryExpression : IExpression
    {
        public TokenType Operator { get; }
        public IExpression Operand { get; }

        public UnaryExpression(TokenType op, IExpression operand)
        {
            Operator = op;
            Operand = operand;
        }

        public double Evaluate()
        {
            var value = Operand.Evaluate();

            return Operator switch
            {
                TokenType.Plus => value,
                TokenType.Minus => -value,
                _ => throw new Exception("Invalid operator")
            };
        }
    }

    public class FunctionExpression : IExpression
    {
        public string Name { get; }
        public IExpression Argument { get; }

        public FunctionExpression(string name, IExpression arg)
        {
            Name = name;
            Argument = arg;
        }

        public double Evaluate()
        {
            var argValue = Argument.Evaluate();
            return Name.ToLower() switch
            {
                "sin" => Math.Sin(argValue),
                "cos" => Math.Cos(argValue),
                "tan" => Math.Tan(argValue),
                "sqrt" => Math.Sqrt(argValue),
                "log" => Math.Log10(argValue),
                _ => throw new Exception("Invalid function")
            };
        }
    }
}