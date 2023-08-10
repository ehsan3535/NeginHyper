using AutoMapper;
using AutoMapper.QueryableExtensions;
using Client.Models;
using Client.Models.ProductDto;
using Client.Models.ShopCards;
using Common.Utilities;
using Data.Repositories;
using Entities;
using Entities.Address;
using Entities.CityProvince;
using Entities.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace Client.Controllers
{

    public class HomeController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly IRepository<Products> productripo;
        private readonly IRepository<Setting> settingrepo;
        private readonly IRepository<OurClient> ourclientRepo;
        private readonly IRepository<Province> provinceRepo;
        private readonly IRepository<Address> AddressRepo;
        private readonly IRepository<newsletter> newsletterRepo;
        private readonly IToastNotification notification;


        public HomeController(IMapper mapper, UserManager<User> userManager, IRepository<Products> productripo, IRepository<Address> addressRepo, IRepository<Province> provinceRepo, IRepository<newsletter> newsletterRepo, IToastNotification notification, IRepository<Setting> settingrepo, IRepository<OurClient> ourclientRepo)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.productripo = productripo;
            AddressRepo = addressRepo;
            this.provinceRepo = provinceRepo;
            this.newsletterRepo = newsletterRepo;
            this.notification = notification;
            this.settingrepo = settingrepo;
            this.ourclientRepo = ourclientRepo;
        }
        public async Task<IActionResult> Index()
        {

            if (TempData["FirstTime"] == null)
            {
                TempData["FirstTime"] = "FirstTime";
            }

            //latest products
            var model = await productripo.TableNoTracking.ProjectTo<ProductDto>(mapper.ConfigurationProvider).OrderByDescending(x => x.CreationDateTime).ToListAsync();
            #region Our Client 

            model.First().OurClient = await ourclientRepo.TableNoTracking.ProjectTo<OurClientDto>(mapper.ConfigurationProvider).ToListAsync();

            #endregion
            model.First().SettingDto = await settingrepo.TableNoTracking.ProjectTo<SettingDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync();
            if (model.First().SettingDto.SeggestActivation == true)
            {
                if (model.First().SettingDto.SeggestDate.Day > DateTime.Now.Day)
                {
                    var hour = 0;
                    var Day = model.First().SettingDto.SeggestDate.Day - DateTime.Now.Day;
                    if (model.First().SettingDto.SeggestHour.ToInt() > DateTime.Now.Hour)
                    {
                        hour += (model.First().SettingDto.SeggestHour.ToInt() - DateTime.Now.Hour) - 1; // دلیل منهای یک اینه که 7اخرین ساعت ممکنه کمتراز یک ساعت باشه مثلا بیست دقیقه ولی اگر هم یک ساعت باشه توی چند خط پایین تر که جلوش کامنت گذاشتم شصت دقیقه بر میگردونه
                    }
                    else
                    {
                        hour += 24 - (DateTime.Now.Hour - model.First().SettingDto.SeggestHour.ToInt()) - 1;
                        Day--; //یه روز باید کم بشه چون اون روز کامل نبوده و توی محاسبات خط بالا ساعتش حساب میشه
                    }
                    hour += Day * 24;
                    var min = hour * 60;
                    min += 60 - DateTime.Now.Minute; //دلیل منهای یک دستور محاسبه ساعت

                    model.First().SettingDto.FinalTimer = min * 60;

                }
                else if (model.First().SettingDto.SeggestDate.Day == DateTime.Now.Day)
                {
                    var hour = 0;
                    if (model.First().SettingDto.SeggestHour.ToInt() > DateTime.Now.Hour)
                    {
                        hour += (model.First().SettingDto.SeggestHour.ToInt() - DateTime.Now.Hour) - 1; // دلیل منهای یک اینه که اخرین ساعت ممکنه کمتراز یک ساعت باشه مثلا بیست دقیقه ولی اگر هم یک ساعت باشه توی چند خط پایین تر که جلوش کامنت گذاشتم شصت دقیقه بر میگردونه
                    }
                    else
                    {
                        hour += 24 - (DateTime.Now.Hour - model.First().SettingDto.SeggestHour.ToInt()) - 1;
                    }
                    var min = hour * 60;
                    min += 60 - DateTime.Now.Minute; //دلیل منهای یک دستور محاسبه ساعت
                    model.First().SettingDto.FinalTimer = min * 60;
                }
                else
                {
                    var setting = await settingrepo.TableNoTracking.FirstOrDefaultAsync();
                    setting.SeggestActivation = false;
                    settingrepo.Update(setting);
                    model.First().SettingDto.SeggestActivation = false;
                }
            }
            //Topest Products
            if (productripo.TableNoTracking.Where(x => x.Rate >= 4).Count() > 6)
            {
                model.First().Products = await productripo.TableNoTracking.Where(x => x.Rate >= 4).ProjectTo<ProductDto>(mapper.ConfigurationProvider).Take(6).OrderByDescending(x => x.Rate).ToListAsync();
            }
            else
            {
                model.First().Products = await productripo.TableNoTracking.Where(x => x.Rate >= 4).ProjectTo<ProductDto>(mapper.ConfigurationProvider).OrderByDescending(x => x.Rate).ToListAsync();
            }
            //Most Discount Product
            if (productripo.TableNoTracking.Where(x => x.Discount != x.Price).Count() > 6)
            {
                model.First().Products2 = await productripo.TableNoTracking.Where(x => x.Discount != x.Price).ProjectTo<ProductDto>(mapper.ConfigurationProvider).Take(6).OrderByDescending(x => x.Percent).ToListAsync();
            }
            else
            {
                model.First().Products2 = await productripo.TableNoTracking.Where(x => x.Discount != x.Price).ProjectTo<ProductDto>(mapper.ConfigurationProvider).OrderByDescending(x => x.Percent).ToListAsync();
            }

            return View(model);
        }
        [Authorize(Roles = "Client")]

        public async Task<IActionResult> AddressProfile()
        {
            var User = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var model = new AddressDto();
            model.Addresses = AddressRepo.TableNoTracking.Where(x => x.ClientId == User.Id).ProjectTo<AddressDto>(mapper.ConfigurationProvider).ToList();
            model.provinces = provinceRepo.TableNoTracking.ProjectTo<ProvinceDto>(mapper.ConfigurationProvider).ToList();
            TempData["Name"] = User.Fname + " " + User.Lname;
            TempData["PhoneNumber"] = User.PhoneNumber;
            return View(model);
        }
        public IActionResult AboutGilhyper()
        {
            return View();
        }
        public IActionResult Partnership()
        {
            return View();
        }
        public IActionResult newsletter(string PhoneNumber, string ReturnUrl)
        {
            var model = newsletterRepo.TableNoTracking.FirstOrDefault(x => x.PhoneNumber == PhoneNumber);
            if (model != null)
            {
                notification.AddWarningToastMessage("قبلا عضو خبرنامه شده اید!");
            }
            else
            {
                var news = new newsletter()
                {
                    PhoneNumber = PhoneNumber
                };
                newsletterRepo.Add(news);
                notification.AddSuccessToastMessage("با موفقیت انجام شد");
            }
            return Redirect(ReturnUrl);
        }
    }
}
