using NumberRuleEvaluator.Printing.Abstractions;

namespace NumberRuleEvaluator.Sample;

/// <summary>
/// Concrete console-based presentation adapter for <see cref="IResultPrinter"/>.
/// </summary>
public sealed class ConsolePrinter : IResultPrinter
{
    /// <summary>
    /// Writes the given evaluated result text to the console.
    /// </summary>
    /// <param name="result">The evaluated result text to output.</param>
    public void Print(string result) => Console.WriteLine(result);
}
