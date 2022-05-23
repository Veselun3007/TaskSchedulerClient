using Microsoft.AspNetCore.Mvc;

namespace TaskSchedulerClient.Controllers
{
    public class StatusCodeController : Controller
    {
        public IActionResult Error404()
        {
            return View();
        }

    public IActionResult Error500()
        {
            return View();
}
    }
}
