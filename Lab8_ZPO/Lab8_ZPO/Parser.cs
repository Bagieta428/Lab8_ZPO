using System;
using Lab8_ZPO.Expressions;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _position = 0;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    /*  =DRZEWO BINARNE= =DRZEWO BINARNE= =DRZEWO BINARNE= =DRZEWO BINARNE= =DRZEWO BINARNE= =DRZEWO BINARNE= =DRZEWO BINARNE= =DRZEWO BINARNE=  */
    /*                                                                                                                                           */
    /*           [+]                                 ######           ######         #    #                                                      */
    /*           / \                                      #                #         #    #                                                      */
    /*          /   \                                     #     #          #   # #   #    #                                                      */
    /*         2    [*]                              ######   #####   ######    #    ######                                                      */
    /*              / \                              #          #          #   # #        #                                                      */
    /*             /   \                             #                     #              #                                                      */
    /*            3     4                            ######           ######              #                                                      */                                                                                                                 */
    /*                                                                                                                                           */
    /*  1. Parser zaczyna od metody [ParseAddSubstract]: widzi wyrażenie [2 + 3 * 4]                                                             */
    /*  2. [ParseAddSubstract] wywołuje [ParseMultiplyDivide] i parsuje [2] jako zmienną [expr]                                                  */
    /*  3. Wykrywa operator [+] więc zapisuje go do zmiennej [op] i wywołuje [ParseMultiplyDivide] dla dalszej częśc działania ([3 * 4])         */
    /*  4. [ParseMultiplyDivide] parsuje [3] jako zmienną [expr] i wykrywa [*], potem parsuje [4] jako zmienną [right]                           */
    /*  5. [expr] wygląda teraz tak: BinaryExpression(3, 4, Multiply)                                                                            */
    /*  6. Wracamy do [ParseAddSubstract] które teraz ma przypisane wartości:                                                                    */
    /*      - expr = 2                                                                                                                           */
    /*      - right = BinaryExpression(3, 4, Multiply)                                                                                           */
    /*      - op = Plus                                                                                                                          */
    /*  7. Ostatecznie [expr] wygląda tak: BinaryExpression(2, BinaryExpression(3, 4, Multiply), Plus)                                           */
    /*  8. Wykonując [Evaluate](Expressions.cs) na tym wyrażeniu otrzymujemy wynik: 2 + (3 * 4) = 14                                             */


    // parser działa rekurencyjnie. każda metoda wywołuje kolejną metodę, która parsuje podane wyrażenie o wyższym priorytecie
    // np. ParseAddSubstract zawsze wywołuje ParseMultiplyDivide, ponieważ zanim zacznie sam wykonywać metodę Match to daje szansę na znalezenie wyrażenia o wyższym priorytecie
    public IExpression ParseExpression()
    {
        return ParseAddSubstract();
    }

    // + -
    private IExpression ParseAddSubstract()
    {
        var expr = ParseMultiplyDivide(); // spróbuj znaleść wyrażenie z wyższym pryjorytetem

        while (Match(TokenType.Plus, TokenType.Minus))
        {
            var op = Previous().Type;
            var right = ParseMultiplyDivide();
            expr = new BinaryExpression(expr, right, op);
        }

        return expr;
    }

    // * /
    private IExpression ParseMultiplyDivide()
    {
        var expr = ParsePower(); // spróbuj znaleść wyrażenie z wyższym pryjorytetem

        while (Match(TokenType.Multiply, TokenType.Divide))
        {
            var op = Previous().Type;
            var right = ParsePower();
            expr = new BinaryExpression(expr, right, op);
        }

        return expr;
    }

    // ^
    private IExpression ParsePower()
    {
        var expr = ParseUnary(); // spróbuj znaleść wyrażenie z wyższym pryjorytetem

        while (Match(TokenType.Power))
        {
            var op = Previous().Type;
            var right = ParseUnary();
            expr = new BinaryExpression(expr, right, op);
        }

        return expr;
    }

    // -1, +5 etc.
    private IExpression ParseUnary()
    {
        if (Match(TokenType.Plus, TokenType.Minus))
        {
            var op = Previous().Type;
            var right = ParseUnary();
            return new UnaryExpression(op, right);
        }

        return ParsePrimary(); // jeśli nie ma operatora przed wyrażeniem to przejdź do następnej metody
    }

    private IExpression ParsePrimary()
    {
        if (Match(TokenType.Number))
        {
            return new NumberExpression(double.Parse(Previous().Value)); // liczba
        }

        if (Match(TokenType.Pi))
        {
            return new NumberExpression(double.Parse(Previous().Value)); // symbol pi -> stałą liczbę (3.14...)
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
            Consume(TokenType.LeftParenthesis, "Expected '('"); // funkcje muszą mieć nawiasy
            var argument = ParseExpression();
            Consume(TokenType.RightParenthesis, "Expected ')'");
            return new FunctionExpression(function, argument);
        }

        throw new Exception("Unexpected token: " + Previous());
    }

    // przechodzi przez każdy przekazany token przez tokenizer
    // jeśli token ma jeden z podanych typów przechodzi do następnego
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

    // sprawdza czy token jest jednym z podanych typów
    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    // przesuwa pozycję i zwraca poprzedni token
    private Token Advance()
    {
        if (!IsAtEnd()) _position++;
        return Previous();
    }

    // czy jesteśmy na końcu listy tokenów
    private bool IsAtEnd()
    {
        return _position >= _tokens.Count;
    }

    // zwraca aktualny token
    private Token Peek()
    {
        return _tokens[_position];
    }

    // zwraca poprzedni token
    private Token Previous()
    {
        return _tokens[_position - 1];
    }

    // jest to używane tylko w przpadku gdy nie znaleziono tokena nawiasu przy niedomknięciu nawiasu
    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();
        throw new Exception(message);
    }
}