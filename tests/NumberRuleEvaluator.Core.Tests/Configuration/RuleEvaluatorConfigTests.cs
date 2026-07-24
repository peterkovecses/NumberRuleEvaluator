namespace NumberRuleEvaluator.Core.Tests.Configuration;

public class RuleEvaluatorConfigTests
{
    private static readonly NumberRange ValidRange = new(1, 100);
    private const string ValidSeparator = " ";

    [Fact]
    public void Constructor_WhenRangeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rules = Array.Empty<DivisorRule>();

        // Act
        var act = () => new RuleEvaluatorConfig(null!, rules, ValidSeparator);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Constructor_WhenRulesIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new RuleEvaluatorConfig(ValidRange, null!, ValidSeparator);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Constructor_WhenARuleIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rules = new DivisorRule[] { null! };

        // Act
        var act = () => new RuleEvaluatorConfig(ValidRange, rules, ValidSeparator);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Constructor_WhenSeparatorIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rules = Array.Empty<DivisorRule>();

        // Act
        var act = () => new RuleEvaluatorConfig(ValidRange, rules, null!);

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
