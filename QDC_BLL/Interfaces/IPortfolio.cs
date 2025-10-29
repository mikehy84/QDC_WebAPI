using QDC_DAL.Models;

namespace QDC_BLL.Interfaces
{
    public interface IPortfolio : IRepository<Portfolio>
    {
        Task<Portfolio> UpdateAsync(Portfolio entity);
    }
}
