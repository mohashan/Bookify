using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Bookings;
using Bookify.Domain.Shared;
using Bookify.Domain.UnitTests.Apartments;
using Bookify.Domain.UnitTests.Users;
using Bookify.Domain.Users;
using FluentAssertions;
using Bookify.Domain.UnitTests.Infrastructure;

namespace Bookify.Domain.UnitTests.Booking;
public class BookingConfirmTests:BaseTest
{
    [Theory]
    [InlineData(BookingStatus.Rejected)]
    [InlineData(BookingStatus.Completed)]
    [InlineData(BookingStatus.Cancelled)]
    [InlineData(BookingStatus.Confirmed)]
    public void Confirm_ShouldFailedIfBookingIsNotReserved(BookingStatus status)
    {
        // Arrange
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);
        var price = new Money(10.0m, Currency.Eur);
        var period = DateRange.Create(DateRangeData.startDate, DateRangeData.endDate);
        var apartment = ApartmentData.Create(price);
        var pricingService = new PricingService();

        // Act
        var booking = BookingData.Create(apartment, period, pricingService, status);
        var result = booking.Confirm(DateTime.UtcNow);
        // Assert
        result.IsFailure.Should().Be(true);
        result.Error.Should().Be(BookingErrors.NotReserved);
    }

    [Fact]
    public void Confirm_ShouldSetProperties()
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
        var result = booking.Confirm(now);
        // Assert
        result.IsSuccess.Should().Be(true);
        booking.Status.Should().Be(BookingStatus.Confirmed);
        booking.ConfirmedOnUtc.Should().Be(now);
    }

    [Fact]
    public void Confirm_ShouldRaiseDomainEvent()
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
        var result = booking.Confirm(now);
        // Assert
        var resultBooking = AssertDomainEventWasPublished<BookingConfirmedDomainEvent>(booking);
        resultBooking.BookingId.Should().Be(booking.Id);
    }
}

public class BookingCancelTests : BaseTest
{
    [Theory]
    [InlineData(BookingStatus.Rejected)]
    [InlineData(BookingStatus.Completed)]
    [InlineData(BookingStatus.Cancelled)]
    [InlineData(BookingStatus.Reserved)]
    public void Cancel_ShouldFailedIfBookingIsNotConfirmed(BookingStatus status)
    {
        // Arrange
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);
        var price = new Money(10.0m, Currency.Eur);
        var period = DateRange.Create(DateRangeData.startDate, DateRangeData.endDate);
        var apartment = ApartmentData.Create(price);
        var pricingService = new PricingService();

        // Act
        var booking = BookingData.Create(apartment, period, pricingService, status);
        var result = booking.Cancel(DateTime.UtcNow);
        // Assert
        result.IsFailure.Should().Be(true);
        result.Error.Should().Be(BookingErrors.NotConfirmed);
    }

    [Fact]
    public void Cancel_ShouldFailedIfStartDateIsPast()
    {
        // Arrange
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);
        var price = new Money(10.0m, Currency.Eur);
        var period = DateRange.Create(DateRangeData.startDate, DateRangeData.endDate);
        var apartment = ApartmentData.Create(price);
        var pricingService = new PricingService();

        // Act
        var booking = BookingData.Create(apartment, period, pricingService, BookingStatus.Confirmed);
        var result = booking.Cancel(DateTime.UtcNow);
        // Assert
        result.IsFailure.Should().Be(true);
        result.Error.Should().Be(BookingErrors.AlreadyStarted);
    }

    [Fact]
    public void Cancel_ShouldSetProperties()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);
        var price = new Money(10.0m, Currency.Eur);
        var period = DateRange.Create(new DateOnly(now.Year,now.Month,now.Day).AddDays(1),
            new DateOnly(now.Year, now.Month, now.Day).AddDays(2));
        var apartment = ApartmentData.Create(price);
        var pricingService = new PricingService();
        // Act
        var booking = BookingData.Create(apartment, period, pricingService, BookingStatus.Confirmed);
        var result = booking.Cancel(now);
        // Assert
        result.IsSuccess.Should().Be(true);
        booking.Status.Should().Be(BookingStatus.Cancelled);
        booking.CancelledOnUtc.Should().Be(now);
    }

    [Fact]
    public void Cancel_ShouldRaiseDomainEvent()
    {
        // Arrange
        var now = DateTime.UtcNow;

        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);
        var price = new Money(10.0m, Currency.Eur);
        var period = DateRange.Create(new DateOnly(now.Year, now.Month, now.Day).AddDays(1),
                    new DateOnly(now.Year, now.Month, now.Day).AddDays(2)); var apartment = ApartmentData.Create(price);
        var pricingService = new PricingService();
        // Act
        var booking = BookingData.Create(apartment, period, pricingService, BookingStatus.Confirmed);
        var result = booking.Cancel(now);
        // Assert
        var resultBooking = AssertDomainEventWasPublished<BookingCancelledDomainEvent>(booking);
        resultBooking.BookingId.Should().Be(booking.Id);
    }
}
