namespace NumberRuleEvaluator.Core.Tests.Configuration;

public class NumberRangeTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(-10, 10)]
    [InlineData(5, 5)]
    [InlineData(-101, -49)]
    [InlineData(int.MinValue, int.MaxValue)]
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
        const int minimum = 10;
        const int maximum = 5;

        // Act
        var act = () => new NumberRange(minimum, maximum);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Theory]
    [InlineData(1, 10, 1)]
    [InlineData(1, 10, 10)]
    [InlineData(1, 10, 5)]
    [InlineData(-10, -1, -10)]
    [InlineData(-10, -1, -1)]
    [InlineData(-10, -1, -5)]
    [InlineData(-10, 10, 0)]
    [InlineData(int.MinValue, int.MaxValue, 0)]
    [InlineData(int.MinValue, int.MaxValue, int.MinValue)]
    [InlineData(int.MinValue, int.MaxValue, int.MaxValue)]
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
    [InlineData(-10, -1, -11)]
    [InlineData(-10, -1, 0)]
    [InlineData(-10, 10, -11)]
    [InlineData(-10, 10, 11)]
    [InlineData(0, int.MaxValue - 1, int.MaxValue)]
    [InlineData(int.MinValue + 1, 0, int.MinValue)]
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
