using AutoMapper;
using AutoMapper.QueryableExtensions;
using Client.Models;
using Client.Models.Orders;
using Data.Repositories;
using Entities;
using Entities.Address;
using Entities.FreeTime;
using Entities.Orders;
using Entities.Product;
using Entities.ShopCards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace Client.Controllers
{
    [AllowAnonymous]

    public class OrderController : Controller
    {
        private readonly IMapper mapper;
        private readonly IRepository<Order> Orderripo;
        private readonly IRepository<OrderDetail> OrderDetailripo;
        private readonly IRepository<Products> Productripo;
        private readonly IRepository<Address> AddressRepo;
        private readonly IRepository<FreeTime> freeTimeRepo;
        private readonly IRepository<Setting> settingrepo;
        private readonly IRepository<ShopCard> shopcardrepo;
        private readonly UserManager<Entities.User> userManager;
        private readonly IToastNotification notification;

        public OrderController(IMapper mapper, IRepository<Order> orderripo, IRepository<OrderDetail> orderDetailripo, IRepository<Products> productripo, IRepository<Address> addressRepo, IRepository<FreeTime> freeTimeRepo, IRepository<Setting> settingrepo, IRepository<ShopCard> shopcardrepo, UserManager<User> userManager, IToastNotification notification)
        {
            this.mapper = mapper;
            Orderripo = orderripo;
            OrderDetailripo = orderDetailripo;
            Productripo = productripo;
            AddressRepo = addressRepo;
            this.freeTimeRepo = freeTimeRepo;
            this.settingrepo = settingrepo;
            this.shopcardrepo = shopcardrepo;
            this.userManager = userManager;
            this.notification = notification;
        }

        public async Task<IActionResult> OrderList(CancellationToken cancellationToken)
        {
            if (!User.Identity.IsAuthenticated)
            {
                notification.AddErrorToastMessage("ابتدا وارد شوید");
                return RedirectToAction("index", "home");
            }
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            TempData["Name"] = user.Fname + " " + user.Lname;
            TempData["Phone"] = user.PhoneNumber;
            var model = await Orderripo.TableNoTracking.ProjectTo<OrderDto>(mapper.ConfigurationProvider).Where(x=>x.UserId== user.Id).ToListAsync(cancellationToken);
            if (model != null)
            {
                return View(model);
            }
            model = new();
          
            return View(model);
        }
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Invoice(Guid OrderId, CancellationToken cancellationToken)
        {
            var order = await Orderripo.TableNoTracking.Where(x => x.Id == OrderId).ProjectTo<OrderDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            var model = await OrderDetailripo.TableNoTracking.Where(x => x.OrderId == OrderId).ProjectTo<OrderDetailDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);

            var Address = await AddressRepo.TableNoTracking.ProjectTo<AddressDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(t => t.Id == order.AddressId, cancellationToken);
            var freeTime = freeTimeRepo.GetById(order.FreeTimeId);
            order.Address = Address;
            order.FreeTimeId = freeTime.Id;
            order.CreationDateTime = order.CreationDateTime;
            return View(order);
        }
    }
}
