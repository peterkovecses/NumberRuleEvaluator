namespace NumberRuleEvaluator.Core.Tests.Configuration;

public class RuleEvaluatorConfigTests
{
    private static readonly NumberRange ValidRange = new(1, 100);
    private const string ValidSeparator = " ";

    public static IEnumerable<TheoryDataRow<Func<RuleEvaluatorConfig>>> NullArgumentFactories =>
    [
        new TheoryDataRow<Func<RuleEvaluatorConfig>>(
            () => new RuleEvaluatorConfig(null!, Array.Empty<DivisorRule>(), ValidSeparator))
        { TestDisplayName = "range is null" },
        new TheoryDataRow<Func<RuleEvaluatorConfig>>(
            () => new RuleEvaluatorConfig(ValidRange, null!, ValidSeparator))
        { TestDisplayName = "rules is null" },
        new TheoryDataRow<Func<RuleEvaluatorConfig>>(
            () => new RuleEvaluatorConfig(ValidRange, new DivisorRule[] { null! }, ValidSeparator))
        { TestDisplayName = "a rule is null" },
        new TheoryDataRow<Func<RuleEvaluatorConfig>>(
            () => new RuleEvaluatorConfig(ValidRange, Array.Empty<DivisorRule>(), null!))
        { TestDisplayName = "separator is null" }
    ];

    [Theory]
    [MemberData(nameof(NullArgumentFactories))]
    public void Constructor_WhenARequiredArgumentIsNull_ShouldThrowArgumentNullException(Func<RuleEvaluatorConfig> act)
    {
        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Constructor_WhenDivisorsAreDuplicated_ShouldThrowArgumentException()
    {
        // Arrange
        const int DuplicateDivisor = 3;
        var rules = new[]
        {
            new DivisorRule(DuplicateDivisor, "Peter"),
            new DivisorRule(DuplicateDivisor, "Jeffrey")
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
        const string EmptySeparator = "";

        // Act
        var actual = new RuleEvaluatorConfig(ValidRange, rules, EmptySeparator);

        // Assert
        Assert.Equal(EmptySeparator, actual.Separator);
    }

    [Fact]
    public void Constructor_WhenSeparatorIsNotSpecified_ShouldDefaultToSingleSpace()
    {
        // Arrange
        const string ExpectedSeparator = " ";
        var rules = Array.Empty<DivisorRule>();

        // Act
        var actual = new RuleEvaluatorConfig(ValidRange, rules);

        // Assert
        Assert.Equal(ExpectedSeparator, actual.Separator);
    }

    [Fact]
    public void Constructor_WhenRulesAreMutatedAfterConstruction_ShouldNotAffectConfig()
    {
        // Arrange
        const int Divisor = 3;
        var rules = new List<DivisorRule> { new(Divisor, "Peter") };

        // Act
        var config = new RuleEvaluatorConfig(ValidRange, rules, ValidSeparator);
        rules.Add(new DivisorRule(5, "Jeffrey"));

        // Assert
        Assert.Single(config.Rules);
    }
}
