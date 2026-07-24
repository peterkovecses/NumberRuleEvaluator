namespace NumberRuleEvaluator.Core.Tests.Configuration;

public class DivisorRuleTests
{
    private const int ValidDivisor = 3;
    private const string ValidText = "Peter";

    [Fact]
    public void Constructor_WhenDivisorIsPositive_ShouldSucceed()
    {
        // Act
        var actual = new DivisorRule(ValidDivisor, ValidText);

        // Assert
        Assert.Equal(ValidDivisor, actual.Divisor);
        Assert.Equal(ValidText, actual.Text);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WhenDivisorIsZeroOrNegative_ShouldThrowArgumentOutOfRangeException(int divisor)
    {
        // Act
        var act = () => new DivisorRule(divisor, ValidText);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void Constructor_WhenTextIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new DivisorRule(ValidDivisor, null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenTextIsEmptyOrWhitespace_ShouldThrowArgumentException(string text)
    {
        // Act
        var act = () => new DivisorRule(ValidDivisor, text);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WhenDivisorAndTextAreBothInvalid_ShouldThrowArgumentOutOfRangeExceptionForDivisor()
    {
        // Arrange
        const int InvalidDivisor = 0;

        // Act
        var act = () => new DivisorRule(InvalidDivisor, null!);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }
}
