using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using finsyncapi.Helper;

namespace finsyncapi.Dto
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? TimeZone { get; set; }
    }
}