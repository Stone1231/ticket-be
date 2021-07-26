using Backend.Models;

namespace Backend.Repositories
{
    public class TicketRepository : RepositoryBase<Ticket>, ITicketRepository
    {
        public TicketRepository(MyContext context)
            : base(context)
        {
        }
    }

    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(MyContext context)
            : base(context)
        {
        }
    } 
}