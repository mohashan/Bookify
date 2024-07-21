using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Bookings.ReserveBooking;
using Bookify.Application.Exceptions;
using Bookify.Application.UnitTests.Apartments;
using Bookify.Application.UnitTests.Infrastructure;
using Bookify.Application.UnitTests.Users;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Application.UnitTests.Bookings;
public class ReserveBookingTests
{
    private static readonly DateTime now = DateTime.UtcNow;
    private static readonly ReserveBookingCommand Command = new ReserveBookingCommand(Guid.NewGuid(),Guid.NewGuid(),new DateOnly(2024,1,1), new DateOnly(2024, 1, 10));

    private readonly ReserveBookingCommandHandler handler;
    private readonly IUserRepository userRepositoryMock;
    private readonly IApartmentRepository apartmentRepositoryMock;
    private readonly IBookingRepository bookingRepositoryMock;
    private readonly IUnitOfWork unitOfWorkMock;
    private readonly PricingService pricingService;
    private readonly IDateTimeProvider dateTimeProviderMock;

    public ReserveBookingTests()
    {
        this.userRepositoryMock = Substitute.For<IUserRepository>();
        this.apartmentRepositoryMock = Substitute.For<IApartmentRepository>();
        this.bookingRepositoryMock = Substitute.For<IBookingRepository>();
        this.unitOfWorkMock = Substitute.For<IUnitOfWork>();

        this.dateTimeProviderMock = Substitute.For<IDateTimeProvider>();
        dateTimeProviderMock.UtcNow.Returns(now);

        handler = new ReserveBookingCommandHandler(userRepositoryMock, 
            apartmentRepositoryMock, 
            bookingRepositoryMock, 
            unitOfWorkMock,
            new PricingService(),
            dateTimeProviderMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserIsNull()
    {
        // Arrange
        userRepositoryMock.GetByIdAsync(Command.UserId,Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        var result = await handler.Handle(Command, default);

        // Assert
        result.Error.Should().Be(UserErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ApartmentIsNull()
    {
        // Arrange
        var user = UserData.Create();
        userRepositoryMock.GetByIdAsync(Command.UserId, Arg.Any<CancellationToken>()).Returns(user);

        apartmentRepositoryMock.GetByIdAsync(Command.apartmentId, Arg.Any<CancellationToken>()).Returns((Apartment?)null);

        // Act
        var result = await handler.Handle(Command, default);

        // Assert
        result.Error.Should().Be(ApartmentErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BookingOverlapped()
    {
        // Arrange
        var user = UserData.Create();
        var apartment = ApartmentData.Create();
        userRepositoryMock.GetByIdAsync(Command.UserId, Arg.Any<CancellationToken>()).Returns(user);

        apartmentRepositoryMock.GetByIdAsync(Command.apartmentId, Arg.Any<CancellationToken>()).Returns(apartment);

        bookingRepositoryMock.IsOverlappingAsync(apartment,DateRangeData.Create(),Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await handler.Handle(Command, default);

        // Assert
        result.Error.Should().Be(BookingErrors.Overlap);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BookingUnitOfWorkFailed()
    {
        // Arrange
        var user = UserData.Create();
        var apartment = ApartmentData.Create();
        userRepositoryMock.GetByIdAsync(Command.UserId, Arg.Any<CancellationToken>()).Returns(user);

        apartmentRepositoryMock.GetByIdAsync(Command.apartmentId, Arg.Any<CancellationToken>()).Returns(apartment);

        bookingRepositoryMock.IsOverlappingAsync(apartment, DateRangeData.Create(), Arg.Any<CancellationToken>()).Returns(false);

        unitOfWorkMock.SaveChangesAsync().ThrowsAsync(new ConcurrencyException("Concurrency",new Exception()));

        // Act
        var result = await handler.Handle(Command, default);

        // Assert
        result.Error.Should().Be(BookingErrors.Overlap);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_BookingIsReserved()
    {
        // Arrange
        var user = UserData.Create();
        var apartment = ApartmentData.Create();
        userRepositoryMock.GetByIdAsync(Command.UserId, Arg.Any<CancellationToken>()).Returns(user);

        apartmentRepositoryMock.GetByIdAsync(Command.apartmentId, Arg.Any<CancellationToken>()).Returns(apartment);

        bookingRepositoryMock.IsOverlappingAsync(apartment, DateRangeData.Create(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await handler.Handle(Command, default);

        // Assert
        result.IsSuccess.Should().Be(true);
    }

    [Fact]
    public async Task Handle_Should_CallRepository_When_BookingIsReserved()
    {
        // Arrange
        var user = UserData.Create();
        var apartment = ApartmentData.Create();
        userRepositoryMock.GetByIdAsync(Command.UserId, Arg.Any<CancellationToken>()).Returns(user);

        apartmentRepositoryMock.GetByIdAsync(Command.apartmentId, Arg.Any<CancellationToken>()).Returns(apartment);

        bookingRepositoryMock.IsOverlappingAsync(apartment, DateRangeData.Create(), Arg.Any<CancellationToken>()).Returns(false);



        // Act
        var result = await handler.Handle(Command, default);

        // Assert
        bookingRepositoryMock.Received(1).Add(Arg.Is<Booking>(b=>b.Id == result.Value));
    }
}
