using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatchAllLogger.Controllers
{
    public class CatchAllController : Controller
    {
        [Route("{*url}")]
        public IActionResult CatchMeIfYouCan()
        {
            return Ok(Request.HttpContext.Items["RequestRawContent"] as string);
        }
    }
}
