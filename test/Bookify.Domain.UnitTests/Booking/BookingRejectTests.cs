using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Bookings;
using Bookify.Domain.Shared;
using Bookify.Domain.UnitTests.Apartments;
using Bookify.Domain.UnitTests.Users;
using Bookify.Domain.Users;
using FluentAssertions;
using Bookify.Domain.UnitTests.Infrastructure;

namespace Bookify.Domain.UnitTests.Booking;

public class BookingRejectTests : BaseTest
{
    [Theory]
    [InlineData(BookingStatus.Rejected)]
    [InlineData(BookingStatus.Completed)]
    [InlineData(BookingStatus.Cancelled)]
    [InlineData(BookingStatus.Confirmed)]
    public void Reject_ShouldFailedIfBookingIsNotReserved(BookingStatus status)
    {
        // Arrange
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);
        var price = new Money(10.0m, Currency.Eur);
        var period = DateRange.Create(DateRangeData.startDate, DateRangeData.endDate);
        var apartment = ApartmentData.Create(price);
        var pricingService = new PricingService();

        // Act
        var booking = BookingData.Create(apartment, period, pricingService, status);
        var result = booking.Reject(DateTime.UtcNow);
        // Assert
        result.IsFailure.Should().Be(true);
        result.Error.Should().Be(BookingErrors.NotReserved);
    }

    [Fact]
    public void Reject_ShouldSetProperties()
    {
        // Arrange
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);
        var price = new Money(10.0m, Currency.Eur);
        var period = DateRange.Create(DateRangeData.startDate, DateRangeData.endDate);
        var apartment = ApartmentData.Create(price);
        var pricingService = new PricingService();
        var now = DateTime.UtcNow;
        // Act
        var booking = BookingData.Create(apartment, period, pricingService, BookingStatus.Reserved);
        var result = booking.Reject(now);
        // Assert
        result.IsSuccess.Should().Be(true);
        booking.Status.Should().Be(BookingStatus.Rejected);
        booking.RejectedOnUtc.Should().Be(now);
    }

    [Fact]
    public void Reject_ShouldRaiseDomainEvent()
    {
        // Arrange
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);
        var price = new Money(10.0m, Currency.Eur);
        var period = DateRange.Create(DateRangeData.startDate, DateRangeData.endDate);
        var apartment = ApartmentData.Create(price);
        var pricingService = new PricingService();
        var now = DateTime.UtcNow;
        // Act
        var booking = BookingData.Create(apartment, period, pricingService, BookingStatus.Reserved);
        _ = booking.Reject(now);
        // Assert
        var resultBooking = AssertDomainEventWasPublished<BookingRejectedDomainEvent>(booking);
        resultBooking.BookingId.Should().Be(booking.Id);
    }
}
