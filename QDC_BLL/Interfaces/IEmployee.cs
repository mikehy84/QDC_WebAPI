using QDC_DAL.Models;

namespace QDC_BLL.Interfaces
{
    public interface IEmployee : IRepository<Employee>
    {
        Task<Employee> UpdateAsync(Employee entity);
    }
}
