using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using finsyncapi.DAL.Entities;
using finsyncapi.DAL.IRepositories;

namespace finsyncapi.DAL.Repositories
{
    public class TokenRepository : Repository<RefreshToken, DB1Context>, ITokenRepository
    {
        private readonly DB1Context _context;

        public TokenRepository(DB1Context context) : base(context)
        {
            _context = context;
        }


        public async Task InvalidateAsync(RefreshToken token)
        {
            token.IsUsed = true;
            token.IsRevoked = true;
            //_context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }
    }

}