using NumberRuleEvaluator.Printing.Abstractions;
using NumberRuleEvaluator.Core.Evaluation;

namespace NumberRuleEvaluator.Printing;

/// <summary>
/// Printing-library component that coordinates evaluation followed by optional output through
/// <see cref="IResultPrinter"/>, without adding I/O responsibility to Core.
/// </summary>
public sealed class NumberRulePrintCoordinator
{
    private readonly RuleEvaluator _evaluator;
    private readonly IResultPrinter _printer;

    /// <summary>
    /// Initializes a new instance of <see cref="NumberRulePrintCoordinator"/>.
    /// </summary>
    /// <param name="evaluator">The evaluator used to compute the result for a given number.</param>
    /// <param name="printer">The output port used to deliver the evaluated result.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="evaluator"/> or <paramref name="printer"/> is <see langword="null"/>.
    /// </exception>
    public NumberRulePrintCoordinator(RuleEvaluator evaluator, IResultPrinter printer)
    {
        ArgumentNullException.ThrowIfNull(evaluator);
        ArgumentNullException.ThrowIfNull(printer);

        _evaluator = evaluator;
        _printer = printer;
    }

    /// <summary>
    /// Evaluates <paramref name="number"/> and forwards the result to the configured <see cref="IResultPrinter"/>.
    /// </summary>
    /// <param name="number">The number to evaluate. Must be within the evaluator's configured range.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="number"/> is outside the evaluator's configured range.
    /// </exception>
    public void Execute(int number)
    {
        var result = _evaluator.Evaluate(number);
        _printer.Print(result);
    }
}
