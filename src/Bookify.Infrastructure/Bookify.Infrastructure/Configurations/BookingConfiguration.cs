using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Shared;
using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Infrastructure.Configurations;
internal sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("bookings");

        builder.HasIndex(c=>c.Id);

        builder.OwnsOne(c=>c.PriceForPeriod,priceBuilder =>
        {
            priceBuilder.Property(Money => Money.Currency).HasConversion(currency => currency.Code, value => Currency.FromCode(value));
        });

        builder.OwnsOne(c => c.CleaningFee, CleaningFeeBuilder =>
        {
            CleaningFeeBuilder.Property(Money => Money.Currency).HasConversion(currency => currency.Code, value => Currency.FromCode(value));
        });

        builder.OwnsOne(c => c.AmenitiesUpCharge, amnitionBuilder =>
        {
            amnitionBuilder.Property(Money => Money.Currency).HasConversion(currency => currency.Code, value => Currency.FromCode(value));
        });

        builder.OwnsOne(c => c.TotalPrice, CleaningFeeBuilder =>
        {
            CleaningFeeBuilder.Property(Money => Money.Currency).HasConversion(currency => currency.Code, value => Currency.FromCode(value));
        });

        builder.OwnsOne(c => c.Duration);

        builder.HasOne<Apartment>()
            .WithMany()
            .HasForeignKey(x => x.ApartmentId);

        builder.HasOne<User>()
        .WithMany()
        .HasForeignKey(x => x.UserId);
    }
}
