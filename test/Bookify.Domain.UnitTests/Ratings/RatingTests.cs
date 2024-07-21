using Bookify.Domain.Reviews;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Ratings;
public class RatingTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-10)]
    [InlineData(10)]
    public void Create_FailedIfValueIsNotValid(int value)
    {
        // Act
        var result = Rating.Create(value);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Rating.Invalid);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Create_SuccessIfValueIsValid(int value)
    {
        // Act
        var result = Rating.Create(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(value);
    }
}
