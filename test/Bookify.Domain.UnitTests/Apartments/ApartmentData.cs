using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Domain.UnitTests.Apartments;
public static class ApartmentData
{
    public static Apartment Create(Money price, Money? cleaningFee = null) => new
        (Guid.NewGuid(),
        new Name("Test Apartment"),
        new Description("Test Description"),
        new Address("Test Country", "Test State", "Test ZipCode", "Test City", "Test Street"),
        price,
        cleaningFee ?? Money.Zero(),
        []
         );
}
