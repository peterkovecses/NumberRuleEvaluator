namespace NumberRuleEvaluator.Printing.Abstractions;

/// <summary>
/// Output port for delivering an evaluated result. The consuming application provides the concrete
/// presentation adapter (for example, a console or file writer).
/// </summary>
public interface IResultPrinter
{
    /// <summary>
    /// Outputs the given evaluated result text.
    /// </summary>
    /// <param name="result">The evaluated result text to output.</param>
    void Print(string result);
}
