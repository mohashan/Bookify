﻿using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Application.UnitTests.Apartments;
public static class ApartmentData
{
    public static Apartment Create() => new
        (Guid.NewGuid(),
        new Name("Test Apartment"),
        new Description("Test Description"),
        new Address("Test Country", "Test State", "Test ZipCode", "Test City", "Test Street"),
        new Money(100.0m,Currency.Usd),
        Money.Zero(),
        []
         );
}