using Bookify.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Infrastructure.Repositories;
internal abstract class Repository<T>
    where T : Entity
{
    protected readonly ApplicationDbContext context;

    protected Repository(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<T?> GetByIdAsync(Guid id,CancellationToken cancellationToken = default)
    {
        return await context.Set<T>().FirstOrDefaultAsync(c=>c.Id == id,cancellationToken);
    }

    public virtual void Add(T entity)
    {
        context.Add(entity);
    }
}
