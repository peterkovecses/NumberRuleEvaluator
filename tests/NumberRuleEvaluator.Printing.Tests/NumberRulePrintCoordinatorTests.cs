using NumberRuleEvaluator.Core.Configuration;
using NumberRuleEvaluator.Core.Evaluation;

namespace NumberRuleEvaluator.Printing.Tests;

public class NumberRulePrintCoordinatorTests
{
    [Fact]
    public void Constructor_WhenEvaluatorIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new NumberRulePrintCoordinator(null!, new InMemoryResultPrinter());

        // Assert
        var ex = Assert.Throws<ArgumentNullException>(act);
        Assert.Equal("evaluator", ex.ParamName);
    }

    [Fact]
    public void Constructor_WhenPrinterIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new NumberRulePrintCoordinator(CreateEvaluator(), null!);

        // Assert
        var ex = Assert.Throws<ArgumentNullException>(act);
        Assert.Equal("printer", ex.ParamName);
    }

    [Fact]
    public void Execute_WhenNumberMatchesRules_ShouldForwardEvaluatedResultToPrinterExactlyOnce()
    {
        // Arrange
        const int number = 15;
        const string expected = "Jeffrey Peter";
        var printer = new InMemoryResultPrinter();
        var coordinator = new NumberRulePrintCoordinator(CreateEvaluator(), printer);

        // Act
        coordinator.Execute(number);

        // Assert
        var actual = Assert.Single(printer.Results);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Execute_WhenNoRuleMatches_ShouldForwardNumberFormattedAsFallback()
    {
        // Arrange
        const int number = 14;
        const string expected = "14";
        var printer = new InMemoryResultPrinter();
        var coordinator = new NumberRulePrintCoordinator(CreateEvaluator(), printer);

        // Act
        coordinator.Execute(number);

        // Assert
        var actual = Assert.Single(printer.Results);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Execute_WhenCalledMultipleTimes_ShouldForwardEachResultInOrder()
    {
        // Arrange
        const int firstNumber = 15;
        const int secondNumber = 14;
        string[] expected = ["Jeffrey Peter", "14"];
        var printer = new InMemoryResultPrinter();
        var coordinator = new NumberRulePrintCoordinator(CreateEvaluator(), printer);

        // Act
        coordinator.Execute(firstNumber);
        coordinator.Execute(secondNumber);

        // Assert
        Assert.Equal(expected, printer.Results);
    }

    [Fact]
    public void Execute_WhenNumberIsOutsideRange_ShouldThrowArgumentOutOfRangeExceptionAndNotPrint()
    {
        // Arrange
        const int outOfRangeNumber = 1;
        var printer = new InMemoryResultPrinter();
        var coordinator = new NumberRulePrintCoordinator(CreateEvaluator(), printer);

        // Act
        var act = () => coordinator.Execute(outOfRangeNumber);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
        Assert.Empty(printer.Results);
    }
    
    private static RuleEvaluator CreateEvaluator()
    {
        var configuration = new RuleEvaluatorConfig(
            new NumberRange(14, 72),
            [new DivisorRule(3, "Peter"), new DivisorRule(5, "Jeffrey")]);

        return new RuleEvaluator(configuration);
    }
}
