namespace NumberRuleEvaluator.Core.Configuration;

/// <summary>
/// Represents an inclusive integer range and validates that its minimum does not exceed its maximum.
/// </summary>
public sealed class NumberRange
{
    /// <summary>
    /// Gets the inclusive minimum of the range.
    /// </summary>
    public int Minimum { get; }

    /// <summary>
    /// Gets the inclusive maximum of the range.
    /// </summary>
    public int Maximum { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="NumberRange"/>.
    /// </summary>
    /// <param name="minimum">The inclusive minimum of the range.</param>
    /// <param name="maximum">The inclusive maximum of the range.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="minimum"/> is greater than <paramref name="maximum"/>.</exception>
    public NumberRange(int minimum, int maximum)
    {
        if (minimum > maximum)
        {
            throw new ArgumentException($"Minimum ({minimum}) cannot be greater than maximum ({maximum}).", nameof(minimum));
        }

        Minimum = minimum;
        Maximum = maximum;
    }

    /// <summary>
    /// Determines whether <paramref name="number"/> falls within the inclusive range.
    /// </summary>
    /// <param name="number">The number to check.</param>
    /// <returns><see langword="true"/> if the number is within the inclusive range; otherwise, <see langword="false"/>.</returns>
    public bool Contains(int number) => number >= Minimum && number <= Maximum;
}
