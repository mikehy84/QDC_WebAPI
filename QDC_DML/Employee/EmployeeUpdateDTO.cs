using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QDC_DAL.Models
{
    public class EmployeeUpdateDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Job { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public bool IsArchive { get; set; }
    }
}
