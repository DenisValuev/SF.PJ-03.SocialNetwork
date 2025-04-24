using Microsoft.AspNetCore.Mvc;
using SF.PJ_03.SocialNetwork.ViewModels.Account;

namespace SF.PJ_03.SocialNetwork.Controllers.Account
{
    public class RegisterController : Controller
    {
        [Route("Register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View("Home/Register");
        }

        [Route("RegisterPart2")]
        [HttpGet]
        public IActionResult RegisterPart2(RegisterViewModel model)
        {
            return View("RegisterPart2", model);
        }
    }
}
