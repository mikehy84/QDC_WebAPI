
namespace QDC_DML.Portfolio
{
    public class PortfolioDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public byte[]? Image { get; set; }
        public bool IsArchive { get; set; }
    }
}
