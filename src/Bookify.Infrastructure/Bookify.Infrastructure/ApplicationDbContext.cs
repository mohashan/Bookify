using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Infrastructure;
public sealed class ApplicationDbContext : DbContext,IUnitOfWork
{
    private readonly IPublisher publisher;
    public ApplicationDbContext(DbContextOptions options, IPublisher publisher) : base(options)
    {
        this.publisher = publisher;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            await PublishDomainEventAsync();

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Concurrency exception occured", ex);
        }
        
    }

    private async Task PublishDomainEventAsync()
    {
        var domainEvents = ChangeTracker.Entries<Entity>()
            .Select(c=>c.Entity)
            .SelectMany(entity=>
            {
                var de = entity.GetDomainEvents();
                entity.ClearDomainEvents();
                return de;
            }).ToList();

        foreach (var item in domainEvents)
        {
            await publisher.Publish(item);
        }
    }
}
