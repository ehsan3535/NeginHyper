using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Sms;
using Common.Utilities;
using Data.Repositories;
using Entities;
using Entities.Address;
using Entities.FreeTime;
using Entities.Orders;
using Entities.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Secretary.Models;
using Secretary.Models.Orders;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    [Authorize(Roles = "Admin")]

    public class OrderController : Controller
    {
        private readonly IMapper mapper;
        private readonly IRepository<Order> Orderripo;
        private readonly IRepository<Address> AddressRepo;
        private readonly IRepository<Setting> settingrepo;
        private readonly IRepository<FreeTime> freeTimeRepo;
        private readonly IRepository<OrderDetail> OrderDetailripo;
        private readonly IRepository<Products> Productripo;
        private readonly UserManager<Entities.User> userManager;
        private readonly IToastNotification notification;

        public OrderController(IMapper mapper, IRepository<Order> orderripo, IRepository<Address> addressRepo, IRepository<Setting> settingrepo, IRepository<FreeTime> freeTimeRepo, IRepository<OrderDetail> orderDetailripo, IRepository<Products> productripo, UserManager<Entities.User> userManager, IToastNotification notification)
        {
            this.mapper = mapper;
            Orderripo = orderripo;
            AddressRepo = addressRepo;
            this.settingrepo = settingrepo;
            this.freeTimeRepo = freeTimeRepo;
            OrderDetailripo = orderDetailripo;
            Productripo = productripo;
            this.userManager = userManager;
            this.notification = notification;
        }

        public async Task<IActionResult> OrderList(CancellationToken cancellationToken)

        {
            var model = await Orderripo.TableNoTracking.ProjectTo<OrderDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            return View(model);
        }

        public async Task<IActionResult> OrderDetail(Guid UserId, CancellationToken cancellationToken)
        {
            var model = await Orderripo.TableNoTracking.Where(x => x.UserId == UserId).ProjectTo<OrderDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            if (model != null)
            {
                model.OrderDetails = await OrderDetailripo.TableNoTracking.Where(x => x.OrderId == model.Id).ProjectTo<OrderDetailDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
                return View(model);
            }
            return View();
        }
        public async Task<IActionResult> DeleteOrder(Guid Id, CancellationToken cancellationToken)
        {
            var User = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var model = await Orderripo.TableNoTracking.Where(x => x.UserId == User.Id).FirstOrDefaultAsync(cancellationToken);
            var orderdateillist = OrderDetailripo.Entities.ToList();

            var NewOrderDetailList = new List<OrderDetail>();
            foreach (var item in orderdateillist)
            {
                NewOrderDetailList.Add(item);
            }
            OrderDetailripo.DeleteRange(NewOrderDetailList);
            model.TotalPrice = 0;
            await Orderripo.UpdateAsync(model, cancellationToken);
            return RedirectToAction(nameof(OrderList));
        }
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(OrderDto Dto, CancellationToken cancellationToken)
        {
            
            var Order = Orderripo.GetById(Dto.Id);
            Order.PaymentStatus = Dto.PaymentStatus;
            await Orderripo.UpdateAsync(Order, cancellationToken);
            var User = userManager.Users.FirstOrDefault(x => x.Id == Dto.UserId);
            MeliPayamak.Simple_Rest(User.PhoneNumber, "سفارش" + User.Fname + " " + User.Lname + "\n" + Dto.PaymentStatus.ToDisplay() + "\n" + "است");

            return RedirectToAction("OrderList");
        }

        public async Task<IActionResult> Invoice(Guid OrderId, CancellationToken cancellationToken)
        {
            var order = await Orderripo.TableNoTracking.Where(x => x.Id == OrderId).ProjectTo<OrderDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            var User = await userManager.Users.FirstOrDefaultAsync(x=>x.Id == order.UserId);
            var model = await OrderDetailripo.TableNoTracking.Where(x => x.OrderId == OrderId).ProjectTo<OrderDetailDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);

            var Address = await AddressRepo.TableNoTracking.FirstOrDefaultAsync(t => t.Id == order.AddressId, cancellationToken);
            var setting = await settingrepo.TableNoTracking.FirstOrDefaultAsync();
            var freeTime = freeTimeRepo.GetById(order.FreeTimeId);
            order.PostPrice = setting.PostPrice;
            order.AddressLocation = Address.Location;
            order.Addresses = await AddressRepo.TableNoTracking.Where(x => x.ClientId == User.Id).ProjectTo<AddressDto>(mapper.ConfigurationProvider).ToListAsync();
            order.AddressId = Address.Id;
            order.FreeTimeId = freeTime.Id;
            order.Day = freeTime.Day;
            order.CreationDateTime = freeTime.DateTime;
            order.Hour = freeTime.FromHour + "تا" + freeTime.ToHour;
            return View(order);
        }

    }
}
