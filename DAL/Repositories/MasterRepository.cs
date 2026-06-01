using System.Linq.Expressions;
using finsyncapi.DAL.Entities;
using finsyncapi.DAL.IRepositories;
using Microsoft.EntityFrameworkCore;
using static Dapper.SqlMapper;

namespace finsyncapi.DAL.Repositories
{
    public class MasterRepository : Repository<Currency, DB1Context>, IMasterRepository
    {
        public MasterRepository(DB1Context context) : base(context)
        {
        }
    }
}
