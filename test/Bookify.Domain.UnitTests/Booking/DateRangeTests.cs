using Bookify.Domain.Bookings;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Booking;

public class DateRangeTests
{
    [Fact]
    public void Create_ShouldSetPropertisCorrectly()
    {
        // Act
        var dateRange = DateRange.Create(DateRangeData.startDate, DateRangeData.endDate);

        // Assert
        dateRange.Start.Should().Be(DateRangeData.startDate);
        dateRange.End.Should().Be(DateRangeData.endDate);
    }

    [Fact]
    public void Create_ShouldThrowExceptionIfStartDateIsAfterEndDate()
    {
        // Arrange
        var action = () => DateRange.Create(DateRangeData.startDate, DateRangeData.startDate.AddDays(-1));

        // Assert
        action.Should().Throw<ApplicationException>();
    }
}
