using NumberRuleEvaluator.Core.Configuration;
using NumberRuleEvaluator.Core.Evaluation;

namespace NumberRuleEvaluator.Printing.Tests;

public class NumberRulePrintCoordinatorTests
{
    private static RuleEvaluator CreateEvaluator()
    {
        var configuration = new RuleEvaluatorConfig(
            new NumberRange(14, 72),
            [new DivisorRule(3, "Peter"), new DivisorRule(5, "Jeffrey")]);

        return new RuleEvaluator(configuration);
    }

    public static IEnumerable<TheoryDataRow<Func<NumberRulePrintCoordinator>>> NullArgumentFactories =>
    [
        new TheoryDataRow<Func<NumberRulePrintCoordinator>>(
            () => new NumberRulePrintCoordinator(null!, new InMemoryResultPrinter()))
        { TestDisplayName = "evaluator is null" },
        new TheoryDataRow<Func<NumberRulePrintCoordinator>>(
            () => new NumberRulePrintCoordinator(CreateEvaluator(), null!))
        { TestDisplayName = "printer is null" }
    ];

    [Theory]
    [MemberData(nameof(NullArgumentFactories))]
    public void Constructor_WhenARequiredArgumentIsNull_ShouldThrowArgumentNullException(
        Func<NumberRulePrintCoordinator> act)
    {
        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Execute_WhenNumberMatchesRules_ShouldForwardEvaluatedResultToPrinterExactlyOnce()
    {
        // Arrange
        const int Number = 15;
        const string Expected = "Jeffrey Peter";
        var printer = new InMemoryResultPrinter();
        var coordinator = new NumberRulePrintCoordinator(CreateEvaluator(), printer);

        // Act
        coordinator.Execute(Number);

        // Assert
        var actual = Assert.Single(printer.Results);
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void Execute_WhenNoRuleMatches_ShouldForwardNumberFormattedAsFallback()
    {
        // Arrange
        const int Number = 14;
        const string Expected = "14";
        var printer = new InMemoryResultPrinter();
        var coordinator = new NumberRulePrintCoordinator(CreateEvaluator(), printer);

        // Act
        coordinator.Execute(Number);

        // Assert
        var actual = Assert.Single(printer.Results);
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void Execute_WhenCalledMultipleTimes_ShouldForwardEachResultInOrder()
    {
        // Arrange
        const int FirstNumber = 15;
        const int SecondNumber = 14;
        string[] expected = ["Jeffrey Peter", "14"];
        var printer = new InMemoryResultPrinter();
        var coordinator = new NumberRulePrintCoordinator(CreateEvaluator(), printer);

        // Act
        coordinator.Execute(FirstNumber);
        coordinator.Execute(SecondNumber);

        // Assert
        Assert.Equal(expected, printer.Results);
    }

    [Fact]
    public void Execute_WhenNumberIsOutsideRange_ShouldThrowArgumentOutOfRangeExceptionAndNotPrint()
    {
        // Arrange
        const int OutOfRangeNumber = 1;
        var printer = new InMemoryResultPrinter();
        var coordinator = new NumberRulePrintCoordinator(CreateEvaluator(), printer);

        // Act
        var act = () => coordinator.Execute(OutOfRangeNumber);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
        Assert.Empty(printer.Results);
    }
}
