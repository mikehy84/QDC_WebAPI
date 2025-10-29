
using System.ComponentModel.DataAnnotations.Schema;

namespace QDC_DML.Identity
{
    public class UserDTO
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string? Status { get; set; }
    }
}
