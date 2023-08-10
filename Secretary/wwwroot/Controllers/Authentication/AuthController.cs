using Secretary.Models;
using Secretary.Models.EditUser;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace Secretary.Controllers.Users
{
    public class AuthController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly SignInManager<User> signInManager;
        private readonly IToastNotification notification;
        public AuthController(UserManager<User> userManager, IMapper mapper, SignInManager<User> signInManager, RoleManager<Role> roleManager, IToastNotification notification)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.notification = notification;
        }
        //برای افزودن کاربر توسط ادمین در لیست ادمین میباشد که برای ادمین کردن یک شخص جدید نیاز به رفتن به بخش کلاینت و ساخت یوزر در آن قسمت نباشد
        public IActionResult AddUser(string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Content("~/DashBoard/DashBoard");
            TempData["returnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddUser(UserDto Dto, string returnUrl, CancellationToken cancellationToken)
        {
            var FindUser = await userManager.Users.Where(x => x.UserName == Dto.UserName).FirstOrDefaultAsync(cancellationToken);
            if (FindUser != null)
            {
                ModelState.AddModelError("UserName", "نام کاربری تکراری است.");
                return View();
            }
            Dto.IsActive = true;
            var User = Dto.ToEntity(mapper);
            var status = await userManager.CreateAsync(User, Dto.Password);
            if (status.Succeeded)
            {
                var FindRole = await roleManager.FindByNameAsync("User");
                if (FindRole == null)
                {
                    var Role = new Role()
                    {
                        Name = "User",
                        Description = "this is a normal user"
                    };
                    await roleManager.CreateAsync(Role);
                }
            }
            await userManager.AddToRoleAsync(User, "User");
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                await signInManager.SignInAsync(User, isPersistent: false);
            }
            returnUrl = TempData["returnUrl"].ToString();
            return Redirect(returnUrl);
        }
        public async Task<IActionResult> UserDetail(Guid id, CancellationToken cancellationToken)
        {
            var model = await userManager.Users.Where(x => x.Id == id).ProjectTo<UserDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync();
            return View(model);
        }
        public async Task<IActionResult> EditUser(Guid UserId, string returnUrl, CancellationToken cancellationToken)
        {
            returnUrl = returnUrl ?? Url.Content("~/DashBoard/DashBoard");
            //خط بالا رو گذاشتیم که اگه از یه جایی ادیت یوزر رو صدا زدیم که ریترن یو ار ال رو بهش
            //پاس نداده بودیم،اون موقع از ادرس دیفالت بریزه تو ریترن یو ار ال تا ارور نال بودنش رو نده
            TempData["returnUrl"] = returnUrl;
            if (HttpContext.User.Identity.Name == null)
            {
                return RedirectToAction("AddUser", "Auth");
            }
            var User = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var model = userManager.Users.ProjectTo<EditUserDto>(mapper.ConfigurationProvider).Where(x => x.Id == User.Id).FirstOrDefault();
            if (model != null)
            {
                return View(model);
            }
            return RedirectToAction("DashBoard", "DashBoard");
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserDto dto, string returnUrl)
        {
            var User = await userManager.Users.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (User != null)
            {

                returnUrl = TempData["returnUrl"].ToString();
                //برای اپدیت یک انتیتی باید سایر اطلاعات مثل ای دیش رو یا توسط اینپوت هیدن داخل ویو پاس بدیم یا به شکل زیر وگرنه ارور نال بود میاد
                User = dto.ToEntity(mapper, User);
                if (dto.File != null)
                {
                    User.ImageLink = UploadImage.SaveImage(dto.File, "UsersImage");
                }
                await userManager.UpdateAsync(User);
                return Redirect(returnUrl);
            }
            return View(dto);
        }
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserDto dto, string password, CancellationToken cancellationToken)
        {
            var Users = await userManager.FindByNameAsync(dto.UserName);
            if (Users != null)
            {
                var Status = await signInManager.PasswordSignInAsync(Users, password, true, true);
                if (Status.Succeeded)
                {
                    notification.AddSuccessToastMessage("ورود موفقیت آمیز بود");
                    return RedirectToAction("DashBoard", "DashBoard");
                }
                ModelState.AddModelError("Password", "پسورداشتباه است");
                return View();
            }
            notification.AddSuccessToastMessage("نام کاربری ثبت نشده است.");
            return View();
        }
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("DashBoard", "DashBoard");
        }
        public async Task<IActionResult> Promote(Guid UserId, CancellationToken cancellationToken)
        {
            var User = await userManager.FindByIdAsync(UserId.ToString());
            if (User != null)
            {
                var Role = await roleManager.FindByNameAsync("Admin");
                if (Role == null)
                {
                    Role = new Role()
                    {
                        Name = "Admin",
                        Description = "این کاربر ادمین میباشد"
                    };
                    await roleManager.CreateAsync(Role);
                }
                await userManager.AddToRoleAsync(User, Role.Name);
                notification.AddSuccessToastMessage("کاربر مورد نظر ادمین شد ");
                return RedirectToAction("UserList");
            }
            return View();
        }
        public async Task<IActionResult> UserList(CancellationToken cancellationToken)
        {
            var Users = await userManager.Users.Where(x => x.IsActive == true).ProjectTo<UserDto>(mapper.ConfigurationProvider).ToListAsync();
            foreach (var item in Users)
            {
                var User = mapper.Map<User>(item);
                if (await userManager.IsInRoleAsync(User, "Admin"))
                {
                    item.Role = "Admin";
                }
                else if (await userManager.IsInRoleAsync(User, "User"))
                {
                    item.Role = "User";
                }
            }
            return View(Users);
        }
        public async Task<IActionResult> Active_Deactive_Users(Guid UserId, bool Active, CancellationToken cancellationToken)
        {
            var User = await userManager.FindByIdAsync(UserId.ToString());
            User.IsActive = Active;
            await userManager.UpdateAsync(User);
            return RedirectToAction(nameof(UserList));
        }
        public async Task<IActionResult> ForgetPassword(ChangePssDto Dto, CancellationToken cancellationToken)
        {
            var User = await userManager.FindByIdAsync(Dto.UserId.ToString());

            string Token = await userManager.GeneratePasswordResetTokenAsync(User);

            await userManager.ResetPasswordAsync(User, Token, Dto.Pssword);

            return RedirectToAction(nameof(UserList));
        }
    }
}
