using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace AngularCalculator.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NumberController : ControllerBase
{
    [HttpPost]
    public Calculator Post(string number)
    {
        return number switch
        {
            "+" or "-" or "*" or "/" or "%" => new Calculator(Equation: $" {number} "),

            _ => new Calculator(Equation: number),
        };
    }

    [HttpGet]
    public Calculator Get(string equation)
    {
        var rpnEquation = ConvertToPostFixEquation(new Equation(equation));

        var answer = SolveEquation(rpnEquation);

        return new Calculator(Calculation: answer);
    }

    private string SolveEquation(string rpnEquation)
    {
        var splitEquation = rpnEquation.Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToArray();
        int previousNumber = -1;

        for (int i = 0; i < splitEquation.Length; i++)
        {
            string sequence = splitEquation[i];
            int solution = 0;

            switch (sequence)
            {
                case "*":
                    solution = int.Parse(splitEquation[previousNumber - 1]) * int.Parse(splitEquation[previousNumber]);

                    splitEquation[previousNumber - 1] = solution.ToString();
                    previousNumber--;
                    break;

                case "/":
                    solution = int.Parse(splitEquation[previousNumber - 1]) / int.Parse(splitEquation[previousNumber]);

                    splitEquation[previousNumber - 1] = solution.ToString();
                    previousNumber--;
                    break;

                case "+":
                    solution = int.Parse(splitEquation[previousNumber - 1]) + int.Parse(splitEquation[previousNumber]);

                    splitEquation[previousNumber - 1] = solution.ToString();
                    previousNumber--;
                    break;

                case "-":
                    solution = int.Parse(splitEquation[previousNumber - 1]) - int.Parse(splitEquation[previousNumber]);

                    splitEquation[previousNumber - 1] = solution.ToString();
                    previousNumber--;
                    break;

                case "%":
                    solution = int.Parse(splitEquation[previousNumber - 1]) % int.Parse(splitEquation[previousNumber]);

                    splitEquation[previousNumber - 1] = solution.ToString();
                    previousNumber--;
                    break;

                default:
                    previousNumber++;
                    splitEquation[previousNumber] = splitEquation[i];
                    break;
            }
        }

        return splitEquation[previousNumber];
    }

    /// <summary>
    /// Takes a regular equation and converts it to Reverse Polish Notation.
    /// </summary>
    /// <param name="equation"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private static string ConvertToPostFixEquation(Equation equation)
    {
        var stack = new Stack<OperatorOperand>();
        var postfixEquation = new StringBuilder();
        
        foreach (var item in equation.Values)
        {
            switch (item.Sequence)
            {
                // When the sequence is a number, we just add it to the equation string builder.
                case var _ when int.TryParse(item.Sequence, out var number):
                    postfixEquation.Append($" {number} ");
                    break;

                case "(":
                case ")":
                    throw new NotImplementedException();

                // For operands, we add the sequence to the equation string builder only if the
                // precision of the current item is less than the precision of what's currently at
                // the top of the stack or if the precisions of both the current item and what's
                // currently on the stack are the same and the current item has left associativity.
                default:
                    while ((stack.Count > 0 && item.Precision < stack.Peek().Precision)
                        || (stack.Count > 0 && item.Precision == stack.Peek().Precision && item.Associativity == 'L'))
                    {
                        postfixEquation.Append($" {stack.Pop().Sequence} ");
                    }

                    stack.Push(item);
                    break;
            }
        }

        // Empty the stack regardless of what's inside by this point.
        while (stack.Count > 0)
            postfixEquation.Append($" {stack.Pop().Sequence} ");

        return postfixEquation.ToString();
    }
}
