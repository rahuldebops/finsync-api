using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace finsyncapi.Dto
{
    public class TokenResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}