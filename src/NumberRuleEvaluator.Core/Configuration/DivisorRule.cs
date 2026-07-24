namespace NumberRuleEvaluator.Core.Configuration;

/// <summary>
/// Represents one positive divisor-to-text mapping used during evaluation.
/// </summary>
public sealed class DivisorRule
{
    /// <summary>
    /// Gets the positive divisor that identifies this rule.
    /// </summary>
    public int Divisor { get; }

    /// <summary>
    /// Gets the text produced when a number is evenly divisible by <see cref="Divisor"/>.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="DivisorRule"/>.
    /// </summary>
    /// <param name="divisor">The positive divisor that identifies this rule.</param>
    /// <param name="text">The text produced when a number is evenly divisible by <paramref name="divisor"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="divisor"/> is zero or negative.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="text"/> is empty or whitespace.</exception>
    public DivisorRule(int divisor, string text)
    {
        if (divisor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(divisor), divisor, "Divisor must be positive.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        Divisor = divisor;
        Text = text;
    }
}
