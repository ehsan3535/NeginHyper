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
        public async Task<IActionResult> OrderList(string? Sort, CancellationToken cancellationToken)
        {
            var model = new List<OrderDto>();
            if (Sort != null)
            {
                if (Sort == "0" )
                {
                model = await Orderripo.TableNoTracking.Where(x=>x.PaymentStatus == Entities.Constants.PaymentStatus.Waiting).ProjectTo<OrderDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);

                }
                else if(Sort == "1")
                {
                    model = await Orderripo.TableNoTracking.Where(x => x.PaymentStatus == Entities.Constants.PaymentStatus.Payed).ProjectTo<OrderDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
                }
                else if (Sort == "2")
                {
                    model = await Orderripo.TableNoTracking.Where(x => x.PaymentStatus == Entities.Constants.PaymentStatus.Sending).ProjectTo<OrderDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
                }
                else if (Sort == "3")
                {
                    model = await Orderripo.TableNoTracking.Where(x => x.PaymentStatus == Entities.Constants.PaymentStatus.Sent).ProjectTo<OrderDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
                }
                else if (Sort == "4")
                {
                    model = await Orderripo.TableNoTracking.Where(x => x.PaymentStatus == Entities.Constants.PaymentStatus.Gived).ProjectTo<OrderDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
                }
                else if (Sort == "5")
                {
                    model = await Orderripo.TableNoTracking.Where(x => x.PaymentStatus == Entities.Constants.PaymentStatus.Cash).ProjectTo<OrderDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
                }
            }
            else
            {
                model = await Orderripo.TableNoTracking.ProjectTo<OrderDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            }
            return View(model);
        }

        public async Task<IActionResult> OrderDetail(Guid OrderId, CancellationToken cancellationToken)
        {
            var model = await Orderripo.TableNoTracking.Where(x => x.Id == OrderId).ProjectTo<OrderDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            model.OrderDetails = await OrderDetailripo.TableNoTracking.Where(x => x.OrderId == OrderId).ProjectTo<OrderDetailDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            return View(model);
        }
        /* public async Task<IActionResult> DeleteOrder(Guid Id, CancellationToken cancellationToken)
         {
             var model = await Orderripo.TableNoTracking.Where(x => x.Id == Id).FirstOrDefaultAsync(cancellationToken);
             var orderdateillist = await OrderDetailripo.TableNoTracking.Where(x => x.OrderId == model.Id).ToListAsync(cancellationToken);

             foreach (var item in orderdateillist)
             {
                 item.Add(item);
             }
             OrderDetailripo.DeleteRange(NewOrderDetailList);
             model.TotalPrice = 0;
             await Orderripo.UpdateAsync(model, cancellationToken);
             return RedirectToAction(nameof(OrderList));
         }*/
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
            var model = await OrderDetailripo.TableNoTracking.Where(x => x.OrderId == OrderId).ProjectTo<OrderDetailDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            var Address = await AddressRepo.TableNoTracking.ProjectTo<AddressDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(t => t.Id == order.AddressId, cancellationToken);

            order.Address = Address;
            order.FreeTimeId = order.FreeTimeId;
            order.CreationDateTime = order.CreationDateTime;
            return View(order);
        }

    }
}
