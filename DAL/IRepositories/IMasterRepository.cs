using System.Linq.Expressions;
using finsyncapi.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using static Dapper.SqlMapper;

namespace finsyncapi.DAL.IRepositories
{
    public interface IMasterRepository : IRepository<Currency>
    {
    }
}
