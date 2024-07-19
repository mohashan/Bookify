using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Shared;
using Bookify.Domain.UnitTests.Apartments;
using Bookify.Domain.UnitTests.Infrastructure;
using Bookify.Domain.UnitTests.Users;
using Bookify.Domain.Users;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Booking;
public class BookingTests:BaseTest
{
    [Fact]
    public void Reserved_Should_RaiseBookingReservedDomainModel()
    {
        // Arrange
        var user = User.Create(UserData.FirstName, UserData.LastName,UserData.Email);
        var price = new Money(10.0m, Currency.Eur);
        var period = DateRange.Create(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 10));
        var apartment = ApartmentData.Create(price);
        var pricingService = new PricingService();

        // Act
        var booking = Domain.Bookings.Booking.Reserve(apartment, user.Id, period, DateTime.UtcNow, pricingService);

        // Assert
        var domainEvent = AssertDomainEventWasPublished<BookingReservedDomainEvent>(booking);

        domainEvent.BookingId.Should().Be(booking.Id);
    }
}
