
namespace NumberRuleEvaluator.Core.Tests.Configuration;

public class NumberRangeTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(-10, 10)]
    [InlineData(5, 5)]
    public void Constructor_WhenMinimumDoesNotExceedMaximum_ShouldSucceed(int minimum, int maximum)
    {
        // Act
        var actual = new NumberRange(minimum, maximum);

        // Assert
        Assert.Equal(minimum, actual.Minimum);
        Assert.Equal(maximum, actual.Maximum);
    }

    [Fact]
    public void Constructor_WhenMinimumGreaterThanMaximum_ShouldThrowArgumentException()
    {
        // Arrange
        const int Minimum = 10;
        const int Maximum = 5;

        // Act
        var act = () => new NumberRange(Minimum, Maximum);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Theory]
    [InlineData(1, 10, 1)]
    [InlineData(1, 10, 10)]
    [InlineData(1, 10, 5)]
    public void Contains_WhenNumberIsWithinInclusiveRange_ShouldReturnTrue(int minimum, int maximum, int number)
    {
        // Arrange
        var range = new NumberRange(minimum, maximum);

        // Act
        var actual = range.Contains(number);

        // Assert
        Assert.True(actual);
    }

    [Theory]
    [InlineData(1, 10, 0)]
    [InlineData(1, 10, 11)]
    public void Contains_WhenNumberIsOutsideInclusiveRange_ShouldReturnFalse(int minimum, int maximum, int number)
    {
        // Arrange
        var range = new NumberRange(minimum, maximum);

        // Act
        var actual = range.Contains(number);

        // Assert
        Assert.False(actual);
    }
}
