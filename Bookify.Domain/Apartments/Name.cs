using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Domain.Apartments;

public record Name(string value);

public record Money(decimal Amount, Currency Currency)
{
    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
        {
            throw new InvalidOperationException("Currencies have to be equal");
        }
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money Zero() => new Money(0,Currency.None);
}