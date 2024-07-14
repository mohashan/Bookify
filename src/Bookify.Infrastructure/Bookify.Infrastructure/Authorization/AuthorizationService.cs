using Bookify.Application.Abstractions.Caching;
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
    private readonly ICacheService _cacheService;

    public AuthorizationService(ApplicationDbContext context, ICacheService cacheService)
    {
        this.context = context;
        _cacheService = cacheService;
    }

    public async Task<UserRoleResponse> GetRolesForUserAsync(string identityId)
    {
        var cacheKey = $"auth:roles-{identityId}";
        var cacheRoles = await _cacheService.GetAsync<UserRoleResponse>(cacheKey);

        if(cacheRoles is not null)
        {
            return cacheRoles;
        }

        var roles = await context.Set<User>().Include(c=>c.Roles).Where(c => c.IdentityId == identityId)
            .Select(c=>new UserRoleResponse
            {
                UserId = c.Id,
                Roles = c.Roles.ToList()
            }).FirstOrDefaultAsync();

        await _cacheService.SetAsync(cacheKey, roles);

        return roles;
    }

    public async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        var cacheKey = $"auth:permissions-{identityId}";
        var cachePermissions = await _cacheService.GetAsync<HashSet<string>>(cacheKey);

        if(cachePermissions is not null)
            { return cachePermissions; }


        var permissions = await context.Set<User>()
            .Where(u => u.IdentityId == identityId)
            .SelectMany(u => u.Roles.Select(r => r.Permissions))
            .FirstAsync();

        var permissionHashSet = permissions.Select(p => p.Name).ToHashSet();

        await _cacheService.SetAsync(cacheKey, permissionHashSet);

        return permissionHashSet;
    }
}
