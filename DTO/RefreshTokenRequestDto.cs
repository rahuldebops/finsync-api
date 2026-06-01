using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using finsyncapi.Helper;

namespace finsyncapi.Dto
{
    public class TokenRequestDto
    {
        public string RefreshToken { get; set; }
        public string? TimeZone { get; set; }
    }
}