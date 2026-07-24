namespace NumberRuleEvaluator.Core.Tests.Configuration;

public class RuleEvaluatorConfigTests
{
    private static readonly NumberRange ValidRange = new(1, 100);
    private const string ValidSeparator = " ";

    [Fact]
    public void Constructor_WhenRangeIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new RuleEvaluatorConfig(null!, [], ValidSeparator);

        // Assert
        var ex = Assert.Throws<ArgumentNullException>(act);
        Assert.Equal("range", ex.ParamName);
    }

    [Fact]
    public void Constructor_WhenRulesIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new RuleEvaluatorConfig(ValidRange, null!, ValidSeparator);

        // Assert
        var ex = Assert.Throws<ArgumentNullException>(act);
        Assert.Equal("rules", ex.ParamName);
    }

    [Fact]
    public void Constructor_WhenRuleIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new RuleEvaluatorConfig(ValidRange, [null!], ValidSeparator);

        // Assert
        var ex = Assert.Throws<ArgumentNullException>(act);
        Assert.Equal("rules", ex.ParamName);
    }

    [Fact]
    public void Constructor_WhenSeparatorIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new RuleEvaluatorConfig(ValidRange, [], null!);

        // Assert
        var ex = Assert.Throws<ArgumentNullException>(act);
        Assert.Equal("separator", ex.ParamName);
    }

    [Fact]
    public void Constructor_WhenDivisorsAreDuplicated_ShouldThrowArgumentException()
    {
        // Arrange
        const int duplicateDivisor = 3;
        var rules = new[]
        {
            new DivisorRule(duplicateDivisor, "Peter"),
            new DivisorRule(duplicateDivisor, "Jeffrey")
        };

        // Act
        var act = () => new RuleEvaluatorConfig(ValidRange, rules, ValidSeparator);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WhenRulesAreEmpty_ShouldSucceed()
    {
        // Arrange
        var rules = Array.Empty<DivisorRule>();

        // Act
        var actual = new RuleEvaluatorConfig(ValidRange, rules, ValidSeparator);

        // Assert
        Assert.Empty(actual.Rules);
    }

    [Fact]
    public void Constructor_WhenSeparatorIsEmpty_ShouldSucceed()
    {
        // Arrange
        var rules = Array.Empty<DivisorRule>();
        const string emptySeparator = "";

        // Act
        var actual = new RuleEvaluatorConfig(ValidRange, rules, emptySeparator);

        // Assert
        Assert.Equal(emptySeparator, actual.Separator);
    }

    [Fact]
    public void Constructor_WhenSeparatorIsNotSpecified_ShouldDefaultToSingleSpace()
    {
        // Arrange
        const string expectedSeparator = " ";
        var rules = Array.Empty<DivisorRule>();

        // Act
        var actual = new RuleEvaluatorConfig(ValidRange, rules);

        // Assert
        Assert.Equal(expectedSeparator, actual.Separator);
    }

    [Fact]
    public void Constructor_WhenRulesAreMutatedAfterConstruction_ShouldNotAffectConfig()
    {
        // Arrange
        const int divisor = 3;
        var rules = new List<DivisorRule> { new(divisor, "Peter") };

        // Act
        var config = new RuleEvaluatorConfig(ValidRange, rules, ValidSeparator);
        rules.Add(new DivisorRule(5, "Jeffrey"));

        // Assert
        Assert.Single(config.Rules);
    }
}
