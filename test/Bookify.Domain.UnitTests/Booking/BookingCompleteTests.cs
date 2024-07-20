using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Bookings;
using Bookify.Domain.Shared;
using Bookify.Domain.UnitTests.Apartments;
using Bookify.Domain.UnitTests.Users;
using Bookify.Domain.Users;
using FluentAssertions;
using Bookify.Domain.UnitTests.Infrastructure;

namespace Bookify.Domain.UnitTests.Booking;

public class BookingCompleteTests : BaseTest
{
    [Theory]
    [InlineData(BookingStatus.Rejected)]
    [InlineData(BookingStatus.Completed)]
    [InlineData(BookingStatus.Cancelled)]
    [InlineData(BookingStatus.Reserved)]
    public void Complete_ShouldFailedIfBookingIsNotConfirmed(BookingStatus status)
    {
        // Arrange
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);
        var price = new Money(10.0m, Currency.Eur);
        var period = DateRange.Create(DateRangeData.startDate, DateRangeData.endDate);
        var apartment = ApartmentData.Create(price);
        var pricingService = new PricingService();

        // Act
        var booking = BookingData.Create(apartment, period, pricingService, status);
        var result = booking.Complete(DateTime.UtcNow);
        // Assert
        result.IsFailure.Should().Be(true);
        result.Error.Should().Be(BookingErrors.NotConfirmed);
    }

    [Fact]
    public void Complete_ShouldSetProperties()
    {
        // Arrange
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);
        var price = new Money(10.0m, Currency.Eur);
        var period = DateRange.Create(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 10));
        var apartment = ApartmentData.Create(price);
        var pricingService = new PricingService();
        var now = DateTime.UtcNow;
        // Act
        var booking = BookingData.Create(apartment, period, pricingService, BookingStatus.Confirmed);
        var result = booking.Complete(now);
        // Assert
        result.IsSuccess.Should().Be(true);
        booking.Status.Should().Be(BookingStatus.Completed);
        booking.CompletedOnUtc.Should().Be(now);
    }

    [Fact]
    public void Complete_ShouldRaiseDomainEvent()
    {
        // Arrange
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);
        var price = new Money(10.0m, Currency.Eur);
        var period = DateRange.Create(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 10));
        var apartment = ApartmentData.Create(price);
        var pricingService = new PricingService();
        var now = DateTime.UtcNow;
        // Act
        var booking = BookingData.Create(apartment, period, pricingService, BookingStatus.Confirmed);
        var result = booking.Complete(now);
        // Assert
        var resultBooking = AssertDomainEventWasPublished<BookingCompletedDomainEvent>(booking);
        resultBooking.BookingId.Should().Be(booking.Id);
    }
}
