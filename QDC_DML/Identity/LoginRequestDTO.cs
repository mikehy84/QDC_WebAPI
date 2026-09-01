using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDC_DML.Identity
{
    public class LoginRequestDTO
    {
        [DisplayName("Email")]
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RecaptchaToken { get; set; }
    }
}
