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
                Id = c.Id,
                Roles = c.Roles.ToList()
            }).FirstOrDefaultAsync();
        return roles;
    }
}
