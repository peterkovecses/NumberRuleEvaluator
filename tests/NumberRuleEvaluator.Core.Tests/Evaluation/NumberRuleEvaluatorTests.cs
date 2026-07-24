namespace NumberRuleEvaluator.Core.Tests.Evaluation;

public class NumberRuleEvaluatorTests
{
    private static readonly NumberRange DefaultRange = new(1, 100);

    [Fact]
    public void Constructor_WhenConfigurationIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new CoreEvaluator(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Evaluate_WhenNumberMatchesSingleRule_ShouldReturnRuleText()
    {
        // Arrange
        const int Number = 9;
        const string ExpectedText = "Peter";
        var rules = new[] { new DivisorRule(3, ExpectedText) };
        var config = new NumberRuleEvaluatorConfig(DefaultRange, rules, " ");
        var evaluator = new CoreEvaluator(config);

        // Act
        var actual = evaluator.Evaluate(Number);

        // Assert
        Assert.Equal(ExpectedText, actual);
    }

    [Fact]
    public void Evaluate_WhenNumberMatchesMultipleRules_ShouldReturnAlphabeticallySortedJoinedTexts()
    {
        // Arrange
        const int Number = 15;
        const string Separator = " ";
        const string Expected = "Jeffrey Peter";
        var rules = new[]
        {
            new DivisorRule(3, "Peter"),
            new DivisorRule(5, "Jeffrey")
        };
        var config = new NumberRuleEvaluatorConfig(new NumberRange(14, 72), rules, Separator);
        var evaluator = new CoreEvaluator(config);

        // Act
        var actual = evaluator.Evaluate(Number);

        // Assert
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void Evaluate_WhenSeparatorIsCustom_ShouldJoinMatchesWithSeparator()
    {
        // Arrange
        const int Number = 15;
        const string Separator = "-";
        const string Expected = "Jeffrey-Peter";
        var rules = new[]
        {
            new DivisorRule(3, "Peter"),
            new DivisorRule(5, "Jeffrey")
        };
        var config = new NumberRuleEvaluatorConfig(new NumberRange(14, 72), rules, Separator);
        var evaluator = new CoreEvaluator(config);

        // Act
        var actual = evaluator.Evaluate(Number);

        // Assert
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void Evaluate_WhenSeparatorIsEmpty_ShouldConcatenateMatchesDirectly()
    {
        // Arrange
        const int Number = 15;
        const string EmptySeparator = "";
        const string Expected = "JeffreyPeter";
        var rules = new[]
        {
            new DivisorRule(3, "Peter"),
            new DivisorRule(5, "Jeffrey")
        };
        var config = new NumberRuleEvaluatorConfig(new NumberRange(14, 72), rules, EmptySeparator);
        var evaluator = new CoreEvaluator(config);

        // Act
        var actual = evaluator.Evaluate(Number);

        // Assert
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void Evaluate_WhenNoRuleMatches_ShouldReturnNumberFormattedWithInvariantCulture()
    {
        // Arrange
        const int Number = 14;
        var rules = new[]
        {
            new DivisorRule(3, "Peter"),
            new DivisorRule(5, "Jeffrey")
        };
        var config = new NumberRuleEvaluatorConfig(new NumberRange(14, 72), rules, " ");
        var evaluator = new CoreEvaluator(config);

        // Act
        var actual = evaluator.Evaluate(Number);

        // Assert
        Assert.Equal("14", actual);
    }

    [Fact]
    public void Evaluate_WhenRulesAreEmpty_ShouldReturnNumberAsString()
    {
        // Arrange
        const int Number = 42;
        var config = new NumberRuleEvaluatorConfig(DefaultRange, Array.Empty<DivisorRule>(), " ");
        var evaluator = new CoreEvaluator(config);

        // Act
        var actual = evaluator.Evaluate(Number);

        // Assert
        Assert.Equal("42", actual);
    }

    [Fact]
    public void Evaluate_WhenNumberIsBelowMinimum_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        const int BelowMinimum = 0;
        var config = new NumberRuleEvaluatorConfig(new NumberRange(1, 100), Array.Empty<DivisorRule>(), " ");
        var evaluator = new CoreEvaluator(config);

        // Act
        var act = () => evaluator.Evaluate(BelowMinimum);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void Evaluate_WhenNumberIsAboveMaximum_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        const int AboveMaximum = 101;
        var config = new NumberRuleEvaluatorConfig(new NumberRange(1, 100), Array.Empty<DivisorRule>(), " ");
        var evaluator = new CoreEvaluator(config);

        // Act
        var act = () => evaluator.Evaluate(AboveMaximum);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Theory]
    [InlineData(1, "1")]
    [InlineData(100, "100")]
    public void Evaluate_WhenNumberIsAtInclusiveBoundary_ShouldNotThrow(int number, string expected)
    {
        // Arrange
        var config = new NumberRuleEvaluatorConfig(new NumberRange(1, 100), Array.Empty<DivisorRule>(), " ");
        var evaluator = new CoreEvaluator(config);

        // Act
        var actual = evaluator.Evaluate(number);

        // Assert
        Assert.Equal(expected, actual);
    }
}
