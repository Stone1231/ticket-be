using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [EnableCors]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerBase: Microsoft.AspNetCore.Mvc.ControllerBase
    {
        
    }
}