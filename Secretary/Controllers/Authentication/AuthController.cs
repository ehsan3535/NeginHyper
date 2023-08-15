using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Repositories;
using Entities;
using Entities.Address;
using Entities.CityProvince;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NToastNotify;
using Secretary.Models;
using Secretary.Models.ShopCards;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Secretary.Controllers.Authentication
{
    public class AuthController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly IRepository<Address> addressrepo;
        private readonly IRepository<Province> ProvinceRepo;
        private readonly IRepository<City> CityRepo;
        private readonly SignInManager<User> signInManager;
        private readonly IToastNotification notification;
        public AuthController(UserManager<User> userManager, IMapper mapper, SignInManager<User> signInManager, RoleManager<Role> roleManager, IToastNotification notification, IRepository<Address> addressrepo, IRepository<Province> provinceRepo, IRepository<City> cityRepo)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.notification = notification;
            this.addressrepo = addressrepo;
            ProvinceRepo = provinceRepo;
            CityRepo = cityRepo;
        }
        [Authorize(Roles = "Admin")]

        //برای افزودن کاربر توسط ادمین در لیست ادمین میباشد که برای ادمین کردن یک شخص جدید نیاز به رفتن به بخش کلاینت و ساخت یوزر در آن قسمت نباشد
        public IActionResult AddUser(string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Content("~/DashBoard/DashBoard");
            TempData["returnUrl"] = returnUrl;
            return View();
        }
        [Authorize(Roles = "Admin")]

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
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> UserDetail(Guid id, CancellationToken cancellationToken)
        {
            var model = await userManager.Users.Where(x => x.Id == id).ProjectTo<UserDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync();
            var Address = addressrepo.TableNoTracking.FirstOrDefault(x => x.ClientId == id);
            if (Address != null)
            {
                model.AddressLocation = Address.Location;
                var city = await CityRepo.TableNoTracking.Where(x => x.Id == Address.CityId).ProjectTo<CityDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync();
                var Province = await ProvinceRepo.TableNoTracking.Where(x => x.Id == city.ProvinceId).ProjectTo<ProvinceDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync();
                model.Province = Province.Name;
                model.City = city.Name;
                model.vahed = Address.vahed;
                model.Pelak = Address.pelak;
                model.PostalCode1 = Address.PostalCode;
                model.WhoGetIt = Address.Name;
                model.FromArian = Address.FromArian;
            }
            return View(model);
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
                if (await userManager.IsInRoleAsync(Users, "Admin"))
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
                else
                {
                    notification.AddErrorToastMessage("نام کاربری به عنوان ادمین ثبت نشده است.");
                    return View();
                }
            }
            notification.AddErrorToastMessage("نام کاربری ثبت نشده است.");
            return View();
        }
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("LogIn", "auth");
        }
        [Authorize(Roles = "Admin")]

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
        [Authorize(Roles = "Admin")]

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

        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> AdminList(CancellationToken cancellationToken)
        {
            var AdminList = new List<UserDto>();
            var Users = await userManager.Users.Where(x => x.IsActive == true).ProjectTo<UserDto>(mapper.ConfigurationProvider).ToListAsync();
            foreach (var item in Users)
            {
                var User = mapper.Map<User>(item);
                if (await userManager.IsInRoleAsync(User, "Admin"))
                {
                    AdminList.Add(item);
                }
            }
            return View(AdminList);
        }

        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Active_Deactive_Users(Guid UserId, bool Active, CancellationToken cancellationToken)
        {
            var User = await userManager.FindByIdAsync(UserId.ToString());
            User.IsActive = Active;
            await userManager.UpdateAsync(User);
            return RedirectToAction(nameof(UserList));
        }
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> ForgetPassword(ChangePssDto Dto, CancellationToken cancellationToken)
        {
            var User = await userManager.FindByIdAsync(Dto.UserId.ToString());

            string Token = await userManager.GeneratePasswordResetTokenAsync(User);

            await userManager.ResetPasswordAsync(User, Token, Dto.Pssword);

            return RedirectToAction(nameof(UserList));
        }
        /*public async Task<IActionResult> EditUser(Guid UserId, string returnUrl, CancellationToken cancellationToken)
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
        }*/

        //pakage NOPI
        [AllowAnonymous]
        public async Task<IActionResult> Export(CancellationToken cancellationToken)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot" + @"\Reports\");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var GuidFileName = DateTime.Now.ToShortDateString().Replace("/", "-").Trim() + ".xls";
            var root = Directory.GetCurrentDirectory();
            FileInfo file = new FileInfo(Path.Combine("wwwroot", GuidFileName));

            var memory = new MemoryStream();

            using (var fs = new FileStream(Path.Combine(root + @"\wwwroot", GuidFileName), FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                #region Meters
                if (userManager.Users.Any())
                {
                    ISheet Sheet = workbook.CreateSheet("Users");
                    IRow Row = Sheet.CreateRow(0);
                    Row.CreateCell(0).SetCellValue("PhoneNumber");
                    Row.CreateCell(1).SetCellValue("Name");

                    var CurrentRow = 1;
                    foreach (var item in userManager.Users.ToListAsync().Result)
                    {
                        IRow NewRow = Sheet.CreateRow(CurrentRow);
                        NewRow.CreateCell(0).SetCellValue((item.PhoneNumber).ToString());
                        NewRow.CreateCell(1).SetCellValue((item.Fname + item.Lname).ToString());

                        CurrentRow++;
                    }
                }
                #endregion

                workbook.Write(fs, false);

                using (var stream = new FileStream(Path.Combine(root + @"\wwwroot", GuidFileName), FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                FileStreamResult fileres = new FileStreamResult(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                fileres.FileDownloadName = GuidFileName;
                file.Delete();

                return fileres;
            }
        }

    }
}
