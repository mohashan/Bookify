using Bookify.Domain.Apartments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Infrastructure.Repositories;
internal sealed class ApartmentRepository: Repository<Apartment>,IApartmentRepository
{
    public ApartmentRepository(ApplicationDbContext context) : base(context)
    {
        
    }
}
