using Backend.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace Backend.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private MyContext _context;
        private ITicketRepository _ticket;
        private IUserRepository _user;

        public ITicketRepository Ticket
        {
            get
            {
                if (_ticket == null)
                {
                    _ticket = new TicketRepository(_context);
                }

                return _ticket;
            }
        }

        public IUserRepository User
        {
            get
            {
                if (_user == null)
                {
                    _user = new UserRepository(_context);
                }

                return _user;
            }
        }

        public UnitOfWork(MyContext context)
        {
            _context = context;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public IDbContextTransaction Transaction()
        {
            return _context.Database.BeginTransaction();
        }
    }
}