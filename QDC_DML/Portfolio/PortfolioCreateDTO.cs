using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace QDC_DML.Portfolio
{
    public class PortfolioCreateDTO
    {
        [Required]
        [MaxLength(60)]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public bool IsArchive { get; set; }
    }
}
