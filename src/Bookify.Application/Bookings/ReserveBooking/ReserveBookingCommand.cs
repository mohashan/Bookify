using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Bookings.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bookify.Application.Bookings.ReserveBooking;

public record ReserveBookingCommand(Guid apartmentId, Guid UserId, DateOnly StartDate, DateOnly EndDate):ICommand<Guid>;
