using Bookify.Domain.Bookings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Application.UnitTests.Infrastructure;
public static class DateRangeData
{
    public static DateRange Create() => DateRange.Create(startDate,endDate);
    public static readonly DateOnly startDate = new DateOnly(2024, 1, 1);
    public static readonly DateOnly endDate = new DateOnly(2024, 1, 10);
}
