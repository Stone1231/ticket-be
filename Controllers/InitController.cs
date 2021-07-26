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
using Backend.Services;

namespace Backend.Controllers
{
    public class InitController: ControllerBase
    {
        private readonly TicketService _ticketService;
        private readonly UserService _userService;
        private readonly InitService _initService;

        public InitController(
            TicketService ticketService,
            UserService userService,
            InitService initService
            )
        {
            _ticketService = ticketService;
            _userService = userService;
            _initService = initService;
        }  
        
        [HttpGet("ticket")]
        public IActionResult InitTicket()
        {
            _ticketService.Init();
            return Ok();
        }

        [HttpDelete("ticket")]
        public IActionResult ClearTicket()
        {
            _ticketService.Clear();
            return Ok();
        }   
        
        [HttpGet("user")]
        public IActionResult InitUser()
        {
            _userService.Init();
            return Ok();
        }

        [HttpDelete("user")]
        public IActionResult ClearUser()
        {
            _userService.Clear();
            return Ok();
        }

        [HttpGet("all")]
        public IActionResult InitAll()
        {
            _initService.Init();
            return Ok();
        }

        [HttpDelete("all")]
        public IActionResult ClearAll()
        {
            _initService.Clear();
            return Ok();
        }                
    }
}