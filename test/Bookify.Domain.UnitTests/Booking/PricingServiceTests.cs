using Bookify.Domain.Bookings;
using Bookify.Domain.Shared;
using Bookify.Domain.UnitTests.Apartments;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Domain.UnitTests.Booking;
public class PricingServiceTests
{
    [Fact]
    public void CalculatePrice_ShouldReturnCorrectPrice()
    {
        // Arrange
        var price = new Money(10.0m, Currency.Eur);
        var period = DateRange.Create(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 10));
        var apartment = ApartmentData.Create(price);
        var expectedTotalPrice = new Money(price.Amount * period.LengthInDays,price.Currency);
        var pricingService = new PricingService();

        // Act
        var pricingDetails = pricingService.CalculatePrice(apartment, period);

        //Assert
        pricingDetails.TotalPrice.Should().Be(expectedTotalPrice);
    }

    [Fact]
    public void CalculatePrice_ShouldReturnCorrectPrice_IncludeCleaningFee()
    {
        // Arrange
        var price = new Money(10.0m, Currency.Eur);
        var cleaningFee = new Money(2.0m, Currency.Eur);
        var period = DateRange.Create(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 10));
        var apartment = ApartmentData.Create(price,cleaningFee);
        var expectedTotalPrice = new Money((price.Amount * period.LengthInDays) + cleaningFee.Amount, price.Currency);
        var pricingService = new PricingService();

        // Act
        var pricingDetails = pricingService.CalculatePrice(apartment, period);

        //Assert
        pricingDetails.TotalPrice.Should().Be(expectedTotalPrice);
    }
}
