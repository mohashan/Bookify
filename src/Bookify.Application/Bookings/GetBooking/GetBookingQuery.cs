using Bookify.Application.Abstractions.Caching;
using Bookify.Application.Abstractions.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Application.Bookings.GetBooking;
public sealed record GetBookingQuery(Guid BookingId) : ICachedQuery<BookingResponse>
{

    public string CacheKey => $"booking-{BookingId}";

    public TimeSpan? Expiration => null;


}
