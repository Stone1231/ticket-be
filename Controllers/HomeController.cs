using System;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api")]
    public class HomeController : ControllerBase
    {
        [HttpGet("index")]
        public IActionResult Get()
        {
            return Ok("ASP.NET webapi");
        }

        [HttpGet("error")]
        public IActionResult Error()
        {
            var isError = true;
            if (isError)
            {
                // 模擬無法在 Controller 中捕捉到的例外
                throw new Exception("test error!");

                // 模擬在 Controller 中捕捉到的例外
                // return StatusCode(
                //     (int)HttpStatusCode.InternalServerError,
                //     new
                //     {
                //         state = 1,
                //         msg = "test error!"
                //     }
                // );
            }

            return Ok("Hello World");
        }
    }
}