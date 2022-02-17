using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace TaskSchedulerClient.Controllers
{
    public class UserController : Controller
    {

        [HttpGet]
        public IActionResult Index()
        {
           
            return View();
        }

    }
}
