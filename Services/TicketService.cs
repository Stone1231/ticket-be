using System;
using Backend.Repositories;
using Backend.Models;
using System.Linq;
using System.Collections.Generic;

namespace Backend.Services
{
    public class TicketService : ServiceBase
    {
        public TicketService(IUnitOfWork unitofwork) : base(unitofwork)
        {
        }

        public List<Ticket> GetAll()
        {
            return _unitofwork.Ticket.FindAll().ToList();
        }

        public Ticket GetSingle(int id)
        {
            var entity = _unitofwork.Ticket
                .Include(m => m.User)
                .SingleOrDefault(m => m.Id == id);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = _unitofwork.Ticket.Single(m => m.Id == id);
            if (entity != null)
            {
                _unitofwork.Ticket.Delete(entity);
                _unitofwork.Save();
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

        public void Create(Ticket entity)
        {
            _unitofwork.Ticket.Create(entity);
            _unitofwork.Save();
        }

        public void Update(Ticket entity)
        {
            var ori = _unitofwork.Ticket.Single(m => m.Id == entity.Id);
            _unitofwork.Ticket.Entry(ori).CurrentValues.SetValues(entity);
            _unitofwork.Save();
        }

        public void Init()
        {
            _unitofwork.Ticket.DeleteAll();
            var item = new Ticket();
            item.Summary = "Feature1";
            item.Description = "Feature1 Description";
            item.Type = TicketType.Feature;
            item.Status = StatusType.Start;
            item.Level = SeverityLevel.Critical;
            item.Update = DateTime.Now;
            var pm = _unitofwork.User.FindByCondition(m => m.Role == UserRole.PM).First();
            // item.User = pm;
            item.UserId = pm.Id; 
            _unitofwork.Ticket.Create(item);

            item = new Ticket();
            item.Summary = "Bug1";
            item.Description = "Bug1 Description";
            item.Type = TicketType.Bug;
            item.Status = StatusType.Start;
            item.Level = SeverityLevel.High;
            item.Update = DateTime.Now;
            var qa = _unitofwork.User.FindByCondition(m => m.Role == UserRole.QA).First();
            // item.User = qa;
            item.UserId = qa.Id;
            _unitofwork.Ticket.Create(item);
            
            item = new Ticket();
            item.Summary = "TestCase1";
            item.Description = "TestCase1 Description";
            item.Type = TicketType.TestCase;
            item.Status = StatusType.Start;
            item.Level = SeverityLevel.Medium;
            item.Update = DateTime.Now;
            // item.User = qa;
            item.UserId = qa.Id;
            _unitofwork.Ticket.Create(item);

            _unitofwork.Save();
        }

        public void Clear()
        {
            _unitofwork.Ticket.DeleteAll();
            _unitofwork.Save();
        }
    }
}