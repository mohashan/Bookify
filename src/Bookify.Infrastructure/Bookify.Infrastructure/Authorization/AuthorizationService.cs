using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Infrastructure.Authorization;
internal sealed class AuthorizationService
{
    private readonly ApplicationDbContext context;

    public AuthorizationService(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<UserRoleResponse> GetRolesForUserAsync(string identityId)
    {
        var roles = await context.Set<User>().Include(c=>c.Roles).Where(c => c.IdentityId == identityId)
            .Select(c=>new UserRoleResponse
            {
                UserId = c.Id,
                Roles = c.Roles.ToList()
            }).FirstOrDefaultAsync();
        return roles;
    }

    public async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        var permissions = await context.Set<User>()
            .Where(u => u.IdentityId == identityId)
            .SelectMany(u => u.Roles.Select(r => r.Permissions))
            .FirstAsync();

        return permissions.Select(p => p.Name).ToHashSet();
    }
}
