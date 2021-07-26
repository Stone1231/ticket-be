using Backend.Repositories;
using Backend.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class UserService : ServiceBase
    {
        public UserService(IUnitOfWork unitofwork) : base(unitofwork)
        {

        }

        public List<User> GetAll()
        {
            return _unitofwork.User.FindAll().ToList();
        }

        public User GetSingle(int id)
        {
            var entity = _unitofwork.User
                .Single(m => m.Id == id);
            return entity;
        }
        
        public User GetSingleUserName(string name)
        {
            var entity = _unitofwork.User
                .Single(m => m.Name == name);
            return entity;
        }

        public void Create(User entity)
        {
            _unitofwork.User.Create(entity);
            _unitofwork.Save();
        }

        public void Update(User entity)
        {
            var ori = _unitofwork.User.Single(m => m.Id == entity.Id);
            _unitofwork.User.Entry(ori).CurrentValues.SetValues(entity);
            _unitofwork.Save();
        }

        public void Delete(int id)
        {
            var entity = _unitofwork.User.Single(m => m.Id == id);
            if (entity != null)
            {
                _unitofwork.User.Delete(entity);
                _unitofwork.Save();
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

        public void Init()
        {
            _unitofwork.User.DeleteAll();

            var item = new User();
            item.Name = "admin";
            item.Role = UserRole.Admin;
            _unitofwork.User.Create(item);
            
            item = new User();
            item.Name = "pm";
            item.Role = UserRole.PM;
            _unitofwork.User.Create(item);
            
            item = new User();
            item.Name = "qa";
            item.Role = UserRole.QA;
            _unitofwork.User.Create(item);
            
            item = new User();
            item.Name = "rd";
            item.Role = UserRole.RD;
            _unitofwork.User.Create(item);
            
            _unitofwork.Save();
        }

        public void Clear()
        {
            _unitofwork.User.DeleteAll();
            _unitofwork.Save();
        }

        public List<User> Query(string name)
        {
            return _unitofwork.User.FindByCondition(m => m.Name.Contains(name)).ToList();
        }
    }
}