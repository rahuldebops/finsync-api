using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using finsyncapi.DAL.Entities;
using finsyncapi.DAL.IRepositories;

namespace finsyncapi.DAL.Repositories
{
    public class UserRepository : Repository<User, DB1Context>, IUserRepository
    {
        private readonly DB1Context _context;

        public UserRepository(DB1Context context) : base(context)
        {
            _context = context;
        }

    }
}