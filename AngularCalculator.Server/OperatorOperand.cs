namespace AngularCalculator.Server;

/// <summary>
/// Holds the precision and associativity values for each given string.
/// </summary>
public record OperatorOperand
{
    public string Sequence { get; init; }
    public int Precision { get; init; }
    public char Associativity { get; init; }

    public OperatorOperand(string sequence)
    {
        Sequence = sequence;

        // Determines the precision, I.E: which operations come first.
        Precision = Sequence switch
        {
            "*" or "/" or "%" => 2,

            "+" or "-" => 1,

            _ => -1
        };

        // Determines the associativity, I.E: whether we look to the left or right when we see this
        // this operand.
        Associativity = Sequence switch
        {
            "^" => 'R',

            _ => 'L'
        };
    }
}
