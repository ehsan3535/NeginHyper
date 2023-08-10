using AutoMapper;
using AutoMapper.QueryableExtensions;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Secretary.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Secretary.Controllers.Users
{
    public class UserController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;

        public UserController(UserManager<User> userManager, IMapper mapper)
        {
            this.userManager = userManager;
            this.mapper = mapper;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UserList()
        {
            var Users = userManager.Users.ProjectTo<UserDto>(mapper.ConfigurationProvider).ToList();

            return View(Users);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Active_Deactive_Users(Guid UserId, bool Active)
        {
            var User = await userManager.FindByIdAsync(UserId.ToString());
            User.IsActive = Active;
            await userManager.UpdateAsync(User);
            return RedirectToAction(nameof(UserList));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangePassword(ChangePssDto Dto)
        {
            var User = await userManager.FindByIdAsync(Dto.UserId.ToString());

            string Token = await userManager.GeneratePasswordResetTokenAsync(User);

            await userManager.ResetPasswordAsync(User, Token, Dto.Pssword);

            return RedirectToAction(nameof(UserList));
        }

    }
}
