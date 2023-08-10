using AutoMapper;
using Client.Models;
using Client.Models.Dtos;
using Common.Sms;
using Common.Utilities;
using Data.Repositories;
using Entities;
using Entities.TempUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace Client.Controllers
{
    public class AuthController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<Role> roleManager;
        private readonly IRepository<TempUser> tempUserRepo;
        private readonly IToastNotification notification;

        public AuthController(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, IToastNotification notification, RoleManager<Role> roleManager, IRepository<TempUser> tempUserRepo)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.notification = notification;
            this.roleManager = roleManager;
            this.tempUserRepo = tempUserRepo;
        }
        #region Shopcard Auth
        public async Task<IActionResult> LoginShopCard(string ClientId)
        {
            TempData["ClientId"] = ClientId;
            TempData["Send"] = "a";
            return View();
        }
        public async Task<IActionResult> LoginOtpShopCard(string? PhoneNumber, string? ClientId, bool Resend)
        {
            string message = OtpGenerator.GenerateRandomToken(6);
            //#OTP
            if (TempData["Send"] != null || Resend)
            {
                TempData["OTP"] = message;
                //notification.AddSuccessToastMessage(message);
                MeliPayamak.Simple_Rest(PhoneNumber, "رمز موقت ورود شما به فروشگاه نگین: " + " " + message + "\n " + "Gilhyper.com");
            }
            TempData["UserName1"] = PhoneNumber;
            if (ClientId != null)
            {
                TempData["ClientId"] = ClientId;
            }
            else
            {
                TempData["ClientId"] = TempData["ClientId"].ToString();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginOtpShopCard(LoginOtpDto dto, CancellationToken cancellationToken)
        {
            //For Recovery password
            var User = await userManager.FindByNameAsync(dto.PhoneNumber);
            if (User != null)
            {
                if (dto.OTP == dto.Code.ToString())
                {
                    await signInManager.SignInAsync(User, true);
                    notification.AddSuccessToastMessage("ورود با موفقیت انجام شد");

                    return RedirectToAction("shopCard_detail2", "shopCard", new { ClientId = dto.ClientId });
                }
                else
                {
                    TempData["UserName1"] = dto.PhoneNumber;
                    notification.AddErrorToastMessage("کد وارد شده اشتباه است");
                    return RedirectToAction(nameof(LoginOtpShopCard));
                }
            }
            else
            {
                User = new User()
                {
                    Fname = "فروشگاه نگینی",
                    PhoneNumber = dto.PhoneNumber,
                    UserName = dto.PhoneNumber,
                    Email = dto.PhoneNumber + "-user@Gilhyper.com"
                };
                var Status = await userManager.CreateAsync(User);
                if (Status.Succeeded)
                {
                    var FindRole = await roleManager.FindByNameAsync("Client");
                    if (FindRole == null)
                    {
                        var Role = new Role()
                        {
                            Name = "Client",
                            Description = "this is a Client"
                        };
                        await roleManager.CreateAsync(Role);
                    }
                    await userManager.AddToRoleAsync(User, "Client");
                    await signInManager.SignInAsync(User, true);
                    notification.AddSuccessToastMessage("ورود با موفقیت انجام شد");
                    return RedirectToAction("shopCard_detail2", "shopCard", new { ClientId = TempData["ClientId"].ToString() });
                }
                notification.AddErrorToastMessage("هنگام ثبت خطایی رخ داد");
                return RedirectToAction(nameof(LoginShopCard));
            }
        }
        #endregion

        public IActionResult Login(string returnUrl, LoginDto loginDto)
        {
            //MeliPayamak.Simple_Rest("09032423214", "HI");
            if (returnUrl != null)
            {
                notification.AddWarningToastMessage("ابتدا وارد حساب کاربری خود شوید", new NotyOptions
                {
                    Timeout = 500,
                    ProgressBar = true,
                    Modal = true,
                    Layout = "topCenter",
                    Theme = "metroui",
                });
            }
            returnUrl = returnUrl ?? Url.Content("~/Home/index");
            TempData["returnUrl"] = returnUrl;
            return View(loginDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto, string returnUrl = null)
        {
            if (TempData["returnUrl"] != null)
            {
                returnUrl = TempData["returnUrl"].ToString();
            }
            returnUrl = returnUrl ?? Url.Content("~/home/Index");

            var findUser = await userManager.FindByNameAsync(loginDto.UserName);
            if (findUser == null)
            {
                notification.AddErrorToastMessage("شما ثبت نام نکرده اید");
                return RedirectToAction(nameof(Login), new { loginDto });
            }
            if (!findUser.IsActive)
            {
                notification.AddErrorToastMessage("اکانت کاربری شما غیر فعال است");
                return View(loginDto);
            }
            var SignIn = await signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password, true, lockoutOnFailure: false);
            if (SignIn.Succeeded)
            {
                notification.AddSuccessToastMessage("خوش آمدید");
                await signInManager.PasswordSignInAsync(findUser, loginDto.Password, true, false);
                return Redirect(returnUrl);
            }
            else
            {
                notification.AddErrorToastMessage("رمز عبور اشتباه است");
                return View(loginDto);
            }
        }

        public IActionResult SignUp(string Username)
        {
            var model = new SignUpDto();
            model.PhoneNumber = Username;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            var FindUser = await userManager.FindByNameAsync(dto.PhoneNumber);
            if (FindUser == null)
            {
                var OTP = OtpGenerator.GenerateRandomToken(6);
                var FindTempUser = await tempUserRepo.TableNoTracking.FirstOrDefaultAsync(x => x.UserName == dto.PhoneNumber);
                if (FindTempUser is not null)
                {
                    FindTempUser.Otp = OTP;
                    await tempUserRepo.UpdateAsync(FindTempUser, cancellationToken);
                }
                else
                {
                    var Tempuser = dto.ToEntity(mapper);
                    Tempuser.Otp = OTP;
                    Tempuser.UserName = dto.PhoneNumber;
                    Tempuser.Password = dto.Password;
                    await tempUserRepo.AddAsync(Tempuser, cancellationToken);
                }

                //send otp with sms
                string Message = "رمز موقت ورود شما به فروشگاه نگین:" + $"\n {OTP}" + "\n " + "Gilhyper.Com";
                MeliPayamak.Simple_Rest(dto.PhoneNumber, Message);

                TempData["UserName"] = dto.PhoneNumber;
                return RedirectToAction(nameof(LoginOtp));
            }
            notification.AddErrorToastMessage($"کاربر با این شماره تلفن قبلا ثبت نام شده است!");
            ModelState.AddModelError("PhoneNumber", "کاربر با این شماره تلفن قبلا ثبت نام شده است.");
            return View();
        }

        public async Task<IActionResult> RecoveryPassword()
        {
            TempData["Sms"] = 1;
            return View();
        }

        public async Task<IActionResult> LoginOtp(string? PhoneNumber)
        {
            //for recovery password
            if (PhoneNumber != null)
            {
                string Code = OtpGenerator.GenerateRandomToken(6);
                string message = "کد اعتبار سنجی \n" + "Code:" + Code + "\n Gilhyper.com";
                if (TempData["sms"] != null)
                {
                    MeliPayamak.Simple_Rest(PhoneNumber, message);
                }
                TempData["RecoveryOTP"] = Code;
                //#OTP
                TempData["UserName1"] = PhoneNumber;
            }
            //for SignUp
            else if (TempData["UserName"] != null)
            {
                TempData["UserName1"] = TempData["UserName"].ToString();
                TempData["UserName"] = TempData["UserName"].ToString();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginOtp(LoginOtpDto dto, CancellationToken cancellationToken)
        {
            //For Recovery password
            var User = await userManager.FindByNameAsync(dto.PhoneNumber);
            if (User != null && dto.OTP != null)
            {
                if (dto.Code.ToString() == dto.OTP)
                {
                    await signInManager.SignInAsync(User, true);
                    notification.AddSuccessToastMessage("موفقیت آمیز بود .رمز جدید را وارد کنید");
                    return RedirectToAction("ChangePass", "Auth", new { PhoneNumber = dto.PhoneNumber });
                }
                else
                {
                    notification.AddErrorToastMessage("کد وارد شده اشتباه است");
                    return RedirectToAction(nameof(LoginOtp), new { PhoneNumber = dto.PhoneNumber });
                }
            }
            else if (User == null && TempData["RecoveryOTP"] != null)
            {
                notification.AddErrorToastMessage("این شماره تلفن ثبت نشده است");
                return RedirectToAction(nameof(RecoveryPassword));
            }
            else
            {

                var FindTempUser = await tempUserRepo.TableNoTracking.FirstOrDefaultAsync(x => x.UserName == dto.PhoneNumber);
                if (FindTempUser is not null)
                {
                    if (int.Parse(FindTempUser.Otp) == dto.Code)
                    {
                        var CreateUser = new User()
                        {
                            Fname = FindTempUser.FName,
                            Lname = FindTempUser.LName,
                            PhoneNumber = FindTempUser.UserName,
                            UserName = FindTempUser.UserName,
                            Email = FindTempUser.UserName + "-user@iliawash.com"
                        };
                        var Status = await userManager.CreateAsync(CreateUser, FindTempUser.Password);
                        if (Status.Succeeded)
                        {
                            await tempUserRepo.DeleteAsync(FindTempUser, cancellationToken);

                            var FindRole = await roleManager.FindByNameAsync("Client");
                            if (FindRole == null)
                            {
                                var Role = new Role()
                                {
                                    Name = "Client",
                                    Description = "this is a Client"
                                };
                                await roleManager.CreateAsync(Role);
                            }

                            await userManager.AddToRoleAsync(CreateUser, "Client");
                            await signInManager.SignInAsync(CreateUser, true);
                            notification.AddSuccessToastMessage("ورود با موفقیت انجام شد");

                            return RedirectToAction("Index", "Home");
                        }
                        notification.AddErrorToastMessage("هنگام ثبت خطایی رخ داد");
                        return RedirectToAction(nameof(Login));
                    }

                    TempData["UserName"] = dto.PhoneNumber;
                    notification.AddErrorToastMessage("کد وارد شده اشتباه است");
                    return RedirectToAction(nameof(LoginOtp));
                }
                notification.AddErrorToastMessage($"کاربر قبلا ثبت نام نشده است!");
                return RedirectToAction(nameof(Login));
            }
        }

        public async Task<IActionResult> ChangePass(string PhoneNumber)
        {
            TempData["PhoneNumber"] = PhoneNumber;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePass(SignUpDto Dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                TempData["PhoneNumber"] = Dto.PhoneNumber;
                return View(Dto);
            }
            var FindUser = await userManager.FindByNameAsync(Dto.PhoneNumber);
            var Token = await userManager.GeneratePasswordResetTokenAsync(FindUser);
            var status = await userManager.ResetPasswordAsync(FindUser, Token, Dto.Password);
            if (status.Succeeded)
            {
                notification.AddSuccessToastMessage("تغییر رمز با موفقیت انجام شد");
            }
            else
            {
                notification.AddErrorToastMessage("رمز نامعتبر(رمز ورود باید بیش از 6 کارکتر باشد)");
                TempData["PhoneNumber"] = Dto.PhoneNumber;
                return View(Dto); ;
            }
            return RedirectToAction("index", "Home");
        }

        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "Home");
        }

        public async Task<IActionResult> EditUser(CancellationToken cancellationToken)
        {
            TempData["Edit"] = "Edit";
            return RedirectToAction("Dashboard", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UserDto dto)
        {
            var User = await userManager.Users.FirstOrDefaultAsync(x => x.Id == dto.Id);
            User = dto.ToEntity(mapper, User);
            var status = await userManager.UpdateAsync(User);
            if (dto.RuturnUrl == "ProductPay")
            {
                return RedirectToAction("ProductPay", "Payment", new { ShopCardId = dto.ShopcardId });

            }
            return RedirectToAction("Dashboard", "Home");
        }

    }
}
