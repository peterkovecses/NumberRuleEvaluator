namespace NumberRuleEvaluator.Printing.Tests;

public class NumberRulePrintCoordinatorTests
{
    private static CoreEvaluator CreateEvaluator()
    {
        var configuration = new NumberRuleEvaluatorConfig(
            new NumberRange(14, 72),
            [new DivisorRule(3, "Peter"), new DivisorRule(5, "Jeffrey")],
            " ");

        return new CoreEvaluator(configuration);
    }

    [Fact]
    public void Constructor_WhenEvaluatorIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var printer = new InMemoryResultPrinter();

        // Act
        var act = () => new NumberRulePrintCoordinator(null!, printer);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Constructor_WhenPrinterIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var evaluator = CreateEvaluator();

        // Act
        var act = () => new NumberRulePrintCoordinator(evaluator, null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void EvaluateAndPrint_ShouldForwardEvaluatedResultToPrinterExactlyOnce()
    {
        // Arrange
        const int Number = 15;
        const string Expected = "Jeffrey Peter";
        var printer = new InMemoryResultPrinter();
        var coordinator = new NumberRulePrintCoordinator(CreateEvaluator(), printer);

        // Act
        coordinator.EvaluateAndPrint(Number);

        // Assert
        var actual = Assert.Single(printer.Results);
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void EvaluateAndPrint_ShouldReturnEvaluatedResult()
    {
        // Arrange
        const int Number = 15;
        const string Expected = "Jeffrey Peter";
        var printer = new InMemoryResultPrinter();
        var coordinator = new NumberRulePrintCoordinator(CreateEvaluator(), printer);

        // Act
        var actual = coordinator.EvaluateAndPrint(Number);

        // Assert
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void EvaluateAndPrint_WhenNumberIsOutsideRange_ShouldThrowArgumentOutOfRangeExceptionAndNotPrint()
    {
        // Arrange
        const int OutOfRangeNumber = 1;
        var printer = new InMemoryResultPrinter();
        var coordinator = new NumberRulePrintCoordinator(CreateEvaluator(), printer);

        // Act
        var act = () => coordinator.EvaluateAndPrint(OutOfRangeNumber);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
        Assert.Empty(printer.Results);
    }
}
