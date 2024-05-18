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
        // Converts a regular equation to reverse polish notation.
        var rpnEquation = ConvertToPostFixEquation(new Equation(equation));

        // Solves the equation.
        var answer = SolveEquation(rpnEquation);

        // Returns the answer so we can display it in the frontend.
        return new Calculator(Calculation: answer);
    }

    /// <summary>
    /// Finds the solution to the reverse polish notation equation.
    /// </summary>
    /// <param name="rpnEquation"></param>
    /// <returns>The solution to the equation in string form.</returns>
    private static string SolveEquation(string rpnEquation)
    {
        // Gets all numbers and/or operators.
        var splitEquation =
            rpnEquation.Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToArray();
        
        int previousNumber = -1;

        foreach (string sequence in splitEquation)
        {
            float solution = sequence switch {
                // Performs addition on the previous two numbers.
                "*" => float.Parse(splitEquation[previousNumber - 1]) * float.Parse(splitEquation[previousNumber]),

                // Performs division on the previous two numbers.
                "/" => float.Parse(splitEquation[previousNumber - 1]) / float.Parse(splitEquation[previousNumber]),

                // Performs modulus on the previous two numbers.
                "%" => float.Parse(splitEquation[previousNumber - 1]) % float.Parse(splitEquation[previousNumber]),

                // Performs addition on the previous two numbers.
                "+" => float.Parse(splitEquation[previousNumber - 1]) + float.Parse(splitEquation[previousNumber]),

                // Performs subtraction on the previous two numbers.
                "-" => float.Parse(splitEquation[previousNumber - 1]) - float.Parse(splitEquation[previousNumber]),

                // No number is assigned if the character is not an operand.
                _ => float.NaN
            };

            switch (solution)
            {
                // If we did not perform an operation, then increment the current value of
                // previousNumber and assign the current sequence to the location of the previous number.
                case float.NaN:
                    previousNumber++;
                    splitEquation[previousNumber] = sequence;

                    break;

                // If we performed an operation, then we assign the value to the location of the
                // number right before the previous number in the array and decrement the previous
                // number variable.
                default:
                    splitEquation[previousNumber - 1] = solution.ToString();
                    previousNumber--;

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
