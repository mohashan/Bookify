using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;

namespace Bookify.Domain.UnitTests.Booking;

public static class BookingData
{
    public static Bookings.Booking Create(Apartment apartment,
    DateRange duration,
    PricingService pricingService,
    BookingStatus status)
    {
        var pricingDetails = pricingService.CalculatePrice(apartment, duration);

        return new Bookings.Booking(
            Guid.NewGuid(),
            apartment.Id,
            Guid.NewGuid(),
            duration,
            pricingDetails.PriceForPeriod,
            pricingDetails.CleaningFee,
            pricingDetails.AmenitiesUpCharge,
            pricingDetails.TotalPrice,
            status,
            DateTime.UtcNow);
    }

}
