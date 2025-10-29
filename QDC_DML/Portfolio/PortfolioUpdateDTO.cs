
using Microsoft.AspNetCore.Http;

namespace QDC_DML.Portfolio
{
    public class PortfolioUpdateDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public bool IsArchive { get; set; }
    }
}
