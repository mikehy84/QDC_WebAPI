using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace QDC_DAL.Models
{
    public class EmployeeCreateDTO
    {
        [Required]
        [MaxLength(60)]
        public string? Name { get; set; }
        public string? Job { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public bool IsArchive { get; set; }
    }
}
