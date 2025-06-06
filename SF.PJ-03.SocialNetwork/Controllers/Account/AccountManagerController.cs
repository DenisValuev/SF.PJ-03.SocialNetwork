﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SF.PJ_03.SocialNetwork.Data.Repository;
using SF.PJ_03.SocialNetwork.Data.UoW;
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

        private IUnitOfWork _unitOfWork;

        public AccountManagerController(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
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
        public async Task<IActionResult> MyPage()
        {
            var user = User;
            var result = await _userManager.GetUserAsync(user);
            var model = new UserViewModel(result);
            model.Friends = await GetAllFriend(model.User);
            return View("User", model);
        }

        private async Task<List<User>> GetAllFriend(User user)
        {
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;
            return repository.GetFriendsByUser(user);
        }

        private async Task<List<User>> GetAllFriend()
        {
            var user = User;
            var result = await _userManager.GetUserAsync(user);
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            return repository.GetFriendsByUser(result);
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

        public async Task<IActionResult> DeleteUser()
        {
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            var friendsRepository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;
            var messagesRepository = _unitOfWork.GetRepository<Message>() as MessageRepository;
            await _signInManager.SignOutAsync();

            friendsRepository.ClearFriendsAsync(result);
            messagesRepository.ClearMessagesAsync(result);

            await _userManager.DeleteAsync(result);

            return RedirectToAction("Index", "Home");
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

        [Route("UserList")]
        [HttpGet]
        public async Task<IActionResult> UserList(string search)
        {
            var currentuser = User;
            var result = await _userManager.GetUserAsync(currentuser);
            if (result == null)
            {
                return RedirectToAction("Login", "AccountManager");
            }
            else
            {
                var model = await CreateSearch(search);
                return View("UserList", model);
            }
        }


        [Route("AddFriend")]
        [HttpPost]
        public async Task<IActionResult> AddFriend(string id)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);

            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            await repository.AddFriendAsync(result, friend);


            return RedirectToAction("MyPage", "AccountManager");
        }


        [Route("DeleteFriend")]
        [HttpPost]
        public async Task<IActionResult> DeleteFriend(string id)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);

            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            await repository.DeleteFriendAsync(result, friend);

            return RedirectToAction("MyPage", "AccountManager");

        }
        private async Task<SearchViewModel> CreateSearch(string search)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);
            var list = _userManager.Users.AsEnumerable().Where(x => x.Id != result.Id).ToList();
            if (!string.IsNullOrEmpty(search))
            {
                list = _userManager.Users.AsEnumerable()
                    .Where(x => x.Id != result.Id)
                    .Where(x => x.GetFullName().ToLower().Contains(search.ToLower())).ToList();
            }

            var withfriend = await GetAllFriend();

            var data = new List<UserWithFriendExt>();
            list.ForEach(x =>
            {
                var t = _mapper.Map<UserWithFriendExt>(x);
                t.IsFriendWithCurrent = withfriend.Where(y => y.Id == x.Id || x.Id == result.Id).Count() != 0;
                data.Add(t);
            });

            var model = new SearchViewModel()
            {
                UserList = data
            };

            return model;
        }

        [Route("Chat")]
        [HttpPost]
        public async Task<IActionResult> Chat(string id)
        {
            var model = await GenerateChat(id);
            return View("Chat", model);
        }

        private async Task<ChatViewModel> GenerateChat(string id)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);
            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;

            var mess = repository.GetMessages(result, friend);

            var model = new ChatViewModel()
            {
                You = result,
                ToWhom = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };

            return model;
        }

        [Route("Chat")]
        [HttpGet]
        public async Task<IActionResult> Chat()
        {
            var id = Request.Query["id"];

            var model = await GenerateChat(id);
            return View("Chat", model);
        }

        [Route("NewMessage")]
        [HttpPost]
        public async Task<IActionResult> NewMessage(string id, ChatViewModel chat)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);
            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;

            var item = new Message()
            {
                Sender = result,
                Recipient = friend,
                Text = chat.NewMessage.Text,
            };
            await repository.Create(item);
            ModelState.Clear();
            var model = await GenerateChat(id);
            return View("Chat", model);
        }
    }
}
