using Bookify.Domain.Apartments;
using Bookify.Domain.Reviews;
using Bookify.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Infrastructure.Configurations;
internal sealed class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
    public void Configure(EntityTypeBuilder<Apartment> builder)
    {
        builder.ToTable("apartments");

        builder.HasKey(x => x.Id);

        builder.OwnsOne(c => c.Address);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .HasConversion(c => c.value, value => new Name(value));

        builder.Property(x => x.Description)
            .HasMaxLength(2000)
            .HasConversion(c => c.value, value => new Description(value));

        builder.OwnsOne(x => x.Price, priceBuilder =>
        {
            priceBuilder.Property(Money=>Money.Currency).HasConversion(c=>c.Code,value => Currency.FromCode(value));
        });

        builder.OwnsOne(x => x.CleaningFee, feeBuilder =>
        {
            feeBuilder.Property(Money => Money.Currency).HasConversion(c => c.Code, value => Currency.FromCode(value));
        });
    }
}

