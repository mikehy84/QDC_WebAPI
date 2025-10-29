namespace QDC_BLL.Interfaces
{
    public interface IUnitOfWork
    {
        IPortfolio Portfolio { get; }
        IEmployee Employee { get; }
        IUserRep User { get; }

        Task Save();
    }
}
