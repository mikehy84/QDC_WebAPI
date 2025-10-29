using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDC_DAL.Models
{
    public class AppUser : IdentityUser
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string? Status { get; set; }
    }
}
