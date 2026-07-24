using NumberRuleEvaluator.Core.Evaluation;

namespace NumberRuleEvaluator.Core.Tests.Evaluation;

public class RuleEvaluatorTests
{
    private static readonly NumberRange DefaultRange = new(1, 100);

    [Fact]
    public void Constructor_WhenConfigurationIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new RuleEvaluator(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Evaluate_WhenNumberMatchesSingleRule_ShouldReturnRuleText()
    {
        // Arrange
        const int number = 9;
        const string expectedText = "Peter";
        var rules = new[] { new DivisorRule(3, expectedText) };
        var config = new RuleEvaluatorConfig(DefaultRange, rules);
        var evaluator = new RuleEvaluator(config);

        // Act
        var actual = evaluator.Evaluate(number);

        // Assert
        Assert.Equal(expectedText, actual);
    }

    [Theory]
    [InlineData(" ", "Jeffrey Peter")]
    [InlineData("-", "Jeffrey-Peter")]
    [InlineData("", "JeffreyPeter")]
    public void Evaluate_WhenNumberMatchesMultipleRules_ShouldReturnAlphabeticallySortedTextsJoinedWithSeparator(
        string separator, string expected)
    {
        // Arrange
        const int number = 15;
        var config = new RuleEvaluatorConfig(new NumberRange(14, 72), CreatePeterJeffreyRules(), separator);
        var evaluator = new RuleEvaluator(config);

        // Act
        var actual = evaluator.Evaluate(number);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Evaluate_WhenSeparatorIsNotSpecified_ShouldJoinMultipleMatchesWithDefaultSingleSpace()
    {
        // Arrange
        const int number = 15;
        const string expected = "Jeffrey Peter";
        var config = new RuleEvaluatorConfig(new NumberRange(14, 72), CreatePeterJeffreyRules());
        var evaluator = new RuleEvaluator(config);

        // Act
        var actual = evaluator.Evaluate(number);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Evaluate_WhenNoRuleMatches_ShouldReturnNumberFormattedWithInvariantCulture()
    {
        // Arrange
        const int number = 14;
        const string expected = "14";
        var config = new RuleEvaluatorConfig(new NumberRange(14, 72), CreatePeterJeffreyRules());
        var evaluator = new RuleEvaluator(config);

        // Act
        var actual = evaluator.Evaluate(number);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Evaluate_WhenRulesAreEmpty_ShouldReturnNumberAsString()
    {
        // Arrange
        const int number = 42;
        const string expected = "42";
        var config = new RuleEvaluatorConfig(DefaultRange, []);
        var evaluator = new RuleEvaluator(config);

        // Act
        var actual = evaluator.Evaluate(number);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Evaluate_WhenNumberIsNegativeAndMatchesRule_ShouldReturnRuleText()
    {
        // Arrange
        const int number = -9;
        const string expectedText = "Peter";
        var rules = new[] { new DivisorRule(3, expectedText) };
        var config = new RuleEvaluatorConfig(new NumberRange(-10, 10), rules);
        var evaluator = new RuleEvaluator(config);

        // Act
        var actual = evaluator.Evaluate(number);

        // Assert
        Assert.Equal(expectedText, actual);
    }

    [Fact]
    public void Evaluate_WhenMultipleRulesShareTheSameText_ShouldReturnTextRepeatedPerMatch()
    {
        // Arrange
        const int number = 6;
        const string sharedText = "Fizz";
        const string expected = "Fizz Fizz";
        var rules = new[]
        {
            new DivisorRule(3, sharedText),
            new DivisorRule(6, sharedText)
        };
        var config = new RuleEvaluatorConfig(DefaultRange, rules);
        var evaluator = new RuleEvaluator(config);

        // Act
        var actual = evaluator.Evaluate(number);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Evaluate_WhenMatchingTextsDifferByCase_ShouldSortUsingOrdinalComparer()
    {
        // Arrange
        const int number = 6;
        // Ordinal sorts "Banana" before "apple" (uppercase code points precede lowercase ones);
        // a case-insensitive or culture-aware comparer would sort "apple" first instead.
        const string expected = "Banana apple";
        var rules = new[]
        {
            new DivisorRule(2, "apple"),
            new DivisorRule(3, "Banana")
        };
        var config = new RuleEvaluatorConfig(DefaultRange, rules);
        var evaluator = new RuleEvaluator(config);

        // Act
        var actual = evaluator.Evaluate(number);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Evaluate_WhenNumberIsOutsideInclusiveRange_ShouldThrowArgumentOutOfRangeException(int number)
    {
        // Arrange
        var config = new RuleEvaluatorConfig(new NumberRange(1, 100), []);
        var evaluator = new RuleEvaluator(config);

        // Act
        var act = () => evaluator.Evaluate(number);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Theory]
    [InlineData(1, "1")]
    [InlineData(100, "100")]
    public void Evaluate_WhenNumberIsAtInclusiveBoundary_ShouldNotThrow(int number, string expected)
    {
        // Arrange
        var config = new RuleEvaluatorConfig(new NumberRange(1, 100), []);
        var evaluator = new RuleEvaluator(config);

        // Act
        var actual = evaluator.Evaluate(number);

        // Assert
        Assert.Equal(expected, actual);
    }
    
    private static DivisorRule[] CreatePeterJeffreyRules() =>
    [
        new DivisorRule(3, "Peter"),
        new DivisorRule(5, "Jeffrey")
    ];
}
