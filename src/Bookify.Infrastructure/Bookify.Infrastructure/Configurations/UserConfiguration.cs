using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Infrastructure.Configurations;
internal sealed class UserConfiguration:IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(c=>c.FirstName)
            .HasMaxLength(200)
            .HasConversion(c=>c.value,value=>new FirstName(value));

        builder.Property(c => c.LastName)
            .HasMaxLength(200)
            .HasConversion(c => c.value, value => new LastName(value));

        builder.Property(c => c.Email)
            .HasMaxLength(400)
            .HasConversion(c => c.value, value => new Domain.Users.Email(value));

        builder.HasIndex(u => u.Email).IsUnique();
    }

}
