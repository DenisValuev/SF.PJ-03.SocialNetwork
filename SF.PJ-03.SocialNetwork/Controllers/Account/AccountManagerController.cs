using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SF.PJ_03.SocialNetwork.Extentions;
using SF.PJ_03.SocialNetwork.Models.Users;
using SF.PJ_03.SocialNetwork.ViewModels.Account;

namespace SF.PJ_03.SocialNetwork.Controllers.Account
{
    public class AccountManagerController : Controller
    {
        private IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountManagerController(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Отображение формы авторизации
        /// </summary>
        /// <returns></returns>
        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View("Home/Login");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [Authorize]
        [Route("MyPage")]
        [HttpGet]
        public IActionResult MyPage()
        {
            var user = User;
            var result =  _userManager.GetUserAsync(user);
            return View("User", new UserViewModel(result.Result));
        }

        [Route("Edit")]
        [HttpGet]
        public IActionResult Edit()
        {
            var user = User;
            var result = _userManager.GetUserAsync(user);
            var editmodel = _mapper.Map<UserEditViewModel>(result.Result);
            return View("Edit", editmodel);
        }

        [Authorize]
        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> Update(UserEditViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);

                user.Convert(model);

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("MyPage", "AccountManager");
                }
                else
                {
                    return RedirectToAction("Edit", "AccountMAnager");
                }
            }
            else
            {
                ModelState.AddModelError("", "Некорректные данные");
                return View("Edit", model);
            }
        }

        [Route("AccountManager/Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                var result = await _signInManager.PasswordSignInAsync(user.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("MyPage", "AccountManager");
                }

                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [Route("Logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
