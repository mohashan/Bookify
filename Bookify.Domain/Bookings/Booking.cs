using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Domain.Bookings;

public sealed class Booking : Entity
{
    public Booking(
        Guid id,
        Guid apartmentId,
        Guid userId,
        DateRange duration,
        Money priceForPeriod,
        Money amenitiesUpCharge,
        Money totalPrice,
        BookingStatus status,
        DateTime createdOnUtc
        ) : base(id)
    {
        ApartmentId = apartmentId;
        UserId = userId;
        Duration = duration;
        PriceForPeriod = priceForPeriod;
        AmenitiesUpCharge = amenitiesUpCharge;
        TotalPrice = totalPrice;
        Status = status;
        CreatedOnUtc = createdOnUtc;
    }

    public Guid ApartmentId { get; private set; }

    public Guid UserId { get; private set; }

    public DateRange Duration { get; private set; }

    public Money PriceForPeriod { get; private set; }

    public Money AmenitiesUpCharge { get; private set; }

    public Money TotalPrice { get; private set; }

    public BookingStatus Status { get; set; }

    public DateTime CreatedOnUtc { get; private set; }

    public DateTime? ConfirmedOnUtc { get; set; }
    public DateTime? RejectedOnUtc { get; set; }
    public DateTime? CompletedOnUtc { get; set; }
    public DateTime? CancelledOnUtc { get; set; }

    public static Booking Reserve(Guid apartmentId,Guid userId, DateRange duration, DateTime utcNow,PricingDetails pricingDetails)
    {
        var booking = new Booking(
            Guid.NewGuid(),
            apartmentId,
            userId,
            duration,
            pricingDetails.PriceForPeriod,
            pricingDetails.AmenitiesUpCharge,
            pricingDetails.TotalPrice,
            BookingStatus.Reserved,
            utcNow);

        booking.RaiseDomainEvent(new BookingReservedDomainEvent(booking.Id));

        return booking;
    }
}
