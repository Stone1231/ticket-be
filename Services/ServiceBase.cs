using Backend.Repositories;
using Backend.Models;

namespace Backend.Services
{
    public class ServiceBase
    {
        protected readonly IUnitOfWork _unitofwork;

        public ServiceBase(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }        
    }
}