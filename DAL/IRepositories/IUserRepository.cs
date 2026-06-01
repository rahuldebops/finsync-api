using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using finsyncapi.DAL.Entities;

namespace finsyncapi.DAL.IRepositories
{
    public interface IUserRepository : IRepository<User>
    {
    }
}