using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Security.Claims;
using Backend.Repositories;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.Net.Http.Headers;

namespace Backend.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly UserService _service;
        public UserController(UserService service)
        {
            _service = service;
        }

        // GET: api/User
        // [EnableCors]
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            if (!checkAdmin())
            {
                return Unauthorized();
            }
            return _service.GetAll();
        }

        // GET: api/User/5
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetSingle([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if (!checkAdmin())
            {
                return Unauthorized();
            }

            var data = _service.GetSingle(id);

            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        // PUT: api/User/5
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Put([FromRoute] int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if (!checkAdmin())
            {
                return Unauthorized();
            }

            user.Id = id;
            // if (id != user.Id)
            // {
            //     return BadRequest();
            // }

            // _context.Entry(user).State = EntityState.Modified;

            _service.Update(user);

            return Ok();
        }

        // POST: api/User
        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if (!checkAdmin())
            {
                return Unauthorized();
            }

            _service.Create(user);

            return CreatedAtAction("GetSingle", new { id = user.Id }, user);
        }

        // DELETE: api/User/5
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if (!checkAdmin())
            {
                return Unauthorized();
            }

            _service.Delete(id);

            return Ok();
        }
        
        [Authorize]
        [HttpPost("query")]
        public IActionResult Query([FromBody] string name)
        {
            if (!checkAdmin())
            {
                return Unauthorized();
            }
            
            return Ok(_service.Query(name));
        }
        
        private bool checkAdmin()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var roleStr = claimsIdentity.Claims.Single(m => m.Type == ClaimTypes.Role).Value;
            UserRole role;
            var res = Enum.TryParse(roleStr, true, out role);
            if (!res)
            {
                return false;
            }
            return role == UserRole.Admin;
        }
    }
}