using Bookify.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Infrastructure.Repositories;
internal sealed class UserRepository:Repository<User>,IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
        
    }
    public override void Add(User user)
    {
        foreach (var role in user.Roles)
        {
            context.Attach(role);
        }

        context.Add(user);    
    }
}
