using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bookify.Application.Bookings.ReserveBooking;

public record ReserveBookingCommand(Guid apartmentId, Guid UserId, DateOnly StartDate, DateOnly EndDate):ICommand<Guid>;


internal sealed class ReserveBookingCommandHandler : ICommandHandler<ReserveBookingCommand, Guid>
{
    private readonly IUserRepository userRepository;
    private readonly IApartmentRepository apartmentRepository;
    private readonly IBookingRepository bookingRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly PricingService pricingService;
    private readonly IDateTimeProvider dateTimeProvider;

    public ReserveBookingCommandHandler(IUserRepository userRepository,
    IApartmentRepository apartmentRepository,
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork,
    PricingService pricingService,
    IDateTimeProvider dateTimeProvider)
    {
        this.userRepository = userRepository;
        this.apartmentRepository = apartmentRepository;
        this.bookingRepository = bookingRepository;
        this.unitOfWork = unitOfWork;
        this.pricingService = pricingService;
        this.dateTimeProvider = dateTimeProvider;
    }
    public async Task<Result<Guid>> Handle(ReserveBookingCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound);
        }

        var apartment = await apartmentRepository.GetByIdAsync(request.apartmentId, cancellationToken);
        if(apartment is null)
        {
            return Result.Failure<Guid>(ApartmentErrors.NotFound);
        }

        var duration = DateRange.Create(request.StartDate, request.EndDate);
        if(await bookingRepository.IsOverlappingAsync(apartment, duration, cancellationToken))
        {
            return Result.Failure<Guid>(BookingErrors.Overlap);
        }

        var booking = Booking.Reserve(
            apartment, 
            user.Id, 
            duration, 
            dateTimeProvider.UtcNow,
            pricingService);

        bookingRepository.Add(booking);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(booking.Id);
    }
}