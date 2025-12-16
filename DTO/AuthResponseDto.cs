using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using clientAPP.Models;

namespace clientAPP.DTO
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public User User { get; set; } = null!;
    }
}
