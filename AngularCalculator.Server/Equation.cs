namespace AngularCalculator.Server;

/// <summary>
/// A representation of each operator and operand of an equation.
/// </summary>
public record Equation
{
    public List<OperatorOperand> Values { get; init; } = [];

    /// <summary>
    /// Takes each character and finds a precision and associativity value for each.
    /// </summary>
    /// <param name="rawEquation"></param>
    public Equation(string rawEquation) =>
        Values.AddRange(rawEquation.Split(" ").Select(item => new OperatorOperand(item)));
}
