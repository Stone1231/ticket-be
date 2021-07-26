using Microsoft.EntityFrameworkCore.Storage;

namespace Backend.Repositories
{
    public interface IUnitOfWork
    {
        ITicketRepository Ticket { get; }
        IUserRepository User { get; }
        void Save(); 
        IDbContextTransaction Transaction();
    }
}