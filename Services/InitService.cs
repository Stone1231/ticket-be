using Backend.Repositories;
using Backend.Models;
using System;

namespace Backend.Services
{
    public class InitService : ServiceBase
    {
        private readonly TicketService _ticketService;
        private readonly UserService _userService;
        public InitService(
            IUnitOfWork unitofwork,
            TicketService ticketService,
            UserService userService
            ) : base(unitofwork)
        {
            _ticketService = ticketService;
            _userService = userService;
        }

        public void Init()
        {
            using (var transaction = _unitofwork.Transaction())
            {
                _userService.Init();
                _ticketService.Init();
                transaction.Commit();
            }
        }

        public void Clear()
        {
            using (var transaction = _unitofwork.Transaction())
            {
                _ticketService.Clear();
                _userService.Clear();
                transaction.Commit();
            }
        }
    }
}