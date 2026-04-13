using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers
{
    public class Login : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
