using NumberRuleEvaluator.Printing.Abstractions;

namespace NumberRuleEvaluator.Printing.Tests;
/// <summary>
/// In-memory <see cref="IResultPrinter"/> test double that records every result it receives.
/// </summary>
internal sealed class InMemoryResultPrinter : IResultPrinter
{
    private readonly List<string> _results = [];

    public IReadOnlyList<string> Results => _results;

    public void Print(string result) => _results.Add(result);
}
