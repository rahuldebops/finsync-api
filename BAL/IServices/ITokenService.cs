using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using finsyncapi.DAL.Entities;
using finsyncapi.Models;

namespace finsyncapi.BAL.IServices
{
    public interface ITokenService
    {
        string GenerateAccessToken(ClaimModel reqClaims);
        string GenerateRefreshToken();
    }
}