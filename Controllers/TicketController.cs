using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using System;

namespace Backend.Controllers
{
    public class TicketController : ControllerBase
    {
        private readonly TicketService _service;

        public TicketController(
            TicketService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<Ticket>> Get()
        {
            return _service.GetAll();
        }

        [Authorize]
        [HttpGet("{id}")]
        //public async Task<IActionResult> GetSingle([FromRoute] int id)
        public IActionResult GetSingle([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var data = _service.GetSingle(id);

            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ticket = _service.GetSingle(id);
            if (ticket == null)
            {
                return NotFound();
            }
            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (!checkTicketDelete(ticket, claimsIdentity))
            {
                return Unauthorized();
            }

            _service.Delete(id);

            return Ok();
        }

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Put([FromRoute] int id, [FromBody] Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (!checkTicketRole(ticket, claimsIdentity))
            {
                return Unauthorized();
            }

            ticket.Id = id;

            _service.Update(ticket);

            return Ok();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody] Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (!checkTicketRole(ticket, claimsIdentity))
            {
                return Unauthorized();
            }
            
            var userId = int.Parse(claimsIdentity.Claims.Single(m => m.Type == "id").Value);
            ticket.UserId = userId;
            ticket.Update = DateTime.Now;
            _service.Create(ticket);

            return CreatedAtAction("GetSingle", new {id = ticket.Id}, ticket);
        }

        private bool checkTicketRole(Ticket ticket, ClaimsIdentity claimsIdentity)
        {
            var roleStr = claimsIdentity.Claims.Single(m => m.Type == ClaimTypes.Role).Value;

            UserRole role;
            var res = Enum.TryParse(roleStr, true, out role);
            if (!res)
            {
                return false;
            }
            switch (ticket.Status)
            {
                case StatusType.Start:
                    switch (ticket.Type)
                    {
                        case TicketType.Feature:
                            return role == UserRole.Admin || role == UserRole.PM;
                            break;
                        case TicketType.Bug:
                        case TicketType.TestCase:
                            return role == UserRole.Admin || role == UserRole.QA;
                            break;
                    }
                    break;
                case StatusType.Finish:
                    switch (ticket.Type)
                    {
                        case TicketType.Feature:
                        case TicketType.Bug:
                            return role == UserRole.Admin || role == UserRole.RD;
                            break;
                        case TicketType.TestCase:
                            return role == UserRole.Admin || role == UserRole.QA;
                            break;
                    }
                    break;
            }
            return false;
        }

        private bool checkTicketDelete(Ticket ticket, ClaimsIdentity claimsIdentity)
        {
            var roleStr = claimsIdentity.Claims.Single(m => m.Type == ClaimTypes.Role).Value;
            UserRole role;
            var res = Enum.TryParse(roleStr, true, out role);
            if (!res)
            {
                return false;
            }
            switch (ticket.Type)
            {
                case TicketType.Feature:
                    return role == UserRole.Admin || role == UserRole.PM;
                    break;
                case TicketType.Bug:
                case TicketType.TestCase:
                    return role == UserRole.Admin || role == UserRole.QA;
                    break;
            }
            return false;
        }
    }
}