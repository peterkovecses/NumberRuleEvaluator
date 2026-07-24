using System.Globalization;
using NumberRuleEvaluator.Core.Configuration;

namespace NumberRuleEvaluator.Core.Evaluation;

/// <summary>
/// Receives immutable configuration through its constructor, validates input, and evaluates configured divisor rules.
/// </summary>
/// <param name="configuration">The immutable configuration to evaluate against.</param>
/// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/> is <see langword="null"/>.</exception>
public sealed class RuleEvaluator(RuleEvaluatorConfig configuration)
{
    private readonly RuleEvaluatorConfig _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    /// <summary>
    /// Evaluates <paramref name="number"/> against the configured divisor rules.
    /// </summary>
    /// <param name="number">The number to evaluate. Must be within the configured inclusive range.</param>
    /// <returns>
    /// The alphabetically sorted texts of all matching rules joined with the configured separator, or
    /// <paramref name="number"/> formatted using invariant culture if no rule matched.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="number"/> is outside the configured inclusive range.</exception>
    public string Evaluate(int number)
    {
        if (!_configuration.Range.Contains(number))
        {
            throw new ArgumentOutOfRangeException(
                nameof(number),
                number,
                $"Number must be within the configured range [{_configuration.Range.Minimum}, {_configuration.Range.Maximum}].");
        }

        var matchingTexts = _configuration.Rules
            .Where(rule => number % rule.Divisor == 0)
            .Select(rule => rule.Text)
            .OrderBy(text => text, StringComparer.Ordinal)
            .ToArray();

        if (matchingTexts.Length == 0)
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }

        return string.Join(_configuration.Separator, matchingTexts);
    }
}
