namespace NumberRuleEvaluator.Core.Configuration;

/// <summary>
/// Holds immutable range, rule, and separator configuration for evaluation.
/// </summary>
public sealed class RuleEvaluatorConfig
{
    /// <summary>
    /// Gets the inclusive range of numbers that can be evaluated.
    /// </summary>
    public NumberRange Range { get; }

    /// <summary>
    /// Gets the configured divisor rules.
    /// </summary>
    public IReadOnlyList<DivisorRule> Rules { get; }

    /// <summary>
    /// Gets the separator used to join matching rule texts.
    /// </summary>
    public string Separator { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="RuleEvaluatorConfig"/> with the default space separator.
    /// </summary>
    /// <param name="range">The inclusive range of numbers that can be evaluated.</param>
    /// <param name="rules">The divisor rules to evaluate against. A private copy is created.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="range"/>, <paramref name="rules"/>, or any rule in <paramref name="rules"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">Thrown when two rules in <paramref name="rules"/> share the same divisor.</exception>
    public RuleEvaluatorConfig(NumberRange range, IEnumerable<DivisorRule> rules)
        : this(range, rules, " ")
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="RuleEvaluatorConfig"/>, validating and copying the supplied
    /// configuration state.
    /// </summary>
    /// <param name="range">The inclusive range of numbers that can be evaluated.</param>
    /// <param name="rules">The divisor rules to evaluate against. A private copy is created.</param>
    /// <param name="separator">The separator used to join matching rule texts.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="range"/>, <paramref name="rules"/>, any rule in <paramref name="rules"/>, or
    /// <paramref name="separator"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">Thrown when two rules in <paramref name="rules"/> share the same divisor.</exception>
    public RuleEvaluatorConfig(NumberRange range, IEnumerable<DivisorRule> rules, string separator)
    {
        ArgumentNullException.ThrowIfNull(range);
        ArgumentNullException.ThrowIfNull(rules);
        ArgumentNullException.ThrowIfNull(separator);

        var ruleCopy = rules.ToArray();
        var seenDivisors = new HashSet<int>();

        foreach (var rule in ruleCopy)
        {
            // Reports the outer "rules" parameter, since that is the argument the caller supplied;
            // the null entry has no name of its own from the caller's perspective.
            ArgumentNullException.ThrowIfNull(rule, nameof(rules));
            if (!seenDivisors.Add(rule.Divisor))
            {
                throw new ArgumentException(
                    $"Duplicate divisor {rule.Divisor} is not allowed; a divisor uniquely identifies a rule.",
                    nameof(rules));
            }
        }

        Range = range;
        Rules = Array.AsReadOnly(ruleCopy);
        Separator = separator;
    }
}
