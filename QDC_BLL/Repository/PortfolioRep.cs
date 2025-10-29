using QDC_DAL;
using QDC_BLL.Interfaces;
using QDC_DAL.Models;
using QDC_DAL.Data;

namespace QDC_BLL.Repository
{
    // In "inheritance" we are using ":" form another class
    public class PortfolioRep : Repository<Portfolio>, IPortfolio
    {
        // In "Composition" we just add the other class as a private field
        // and in the constructor of this class we get an object of the other class 
        // to initialize that private field
        // also using dependency injection here
        private readonly ApplicationDbContext _db;

        public PortfolioRep(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }  


        public async Task<Portfolio> UpdateAsync(Portfolio entity)
        {
            _db.Portfolios.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}