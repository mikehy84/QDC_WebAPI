using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDC_DML.Identity
{
    public class RegisterRequestDTO
    {
        public string FullName { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Role { get; set; }
    }
}
