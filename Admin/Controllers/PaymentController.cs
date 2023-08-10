using AutoMapper;
using AutoMapper.QueryableExtensions;
using Client.Models;
using Client.Models.ShopCards;
using Common.Sms;
using Common.Utilities;
using Data.Repositories;
using Dto.Payment;
using Entities;
using Entities.Address;
using Entities.Orders;
using Entities.Product;
using Entities.ShopCards;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Sentry;
using ZarinPal.Class;
using zarinpalasp.netcorerest.Models;

namespace Client.Controllers
{
    public class PaymentController : Controller
    {
        private readonly UserManager<Entities.User> userManager;
        private readonly IToastNotification notification;
        private readonly IRepository<Setting> settingrepo;
        private readonly IRepository<Address> addressrepo;
        private readonly IMapper mapper;
        private readonly IRepository<Order> OrderRepo;
        private readonly IRepository<OrderDetail> OrderDetailRepo;
        private readonly IRepository<ShopCard> ShopCardRepo;
        private readonly IRepository<Products> productrepo;
        private readonly IRepository<ShopCardDetail> ShopCardDetailRepo;
        private readonly Payment _payment;
        private readonly Authority _authority;
        private readonly Transactions _transactions;
        public PaymentController(UserManager<Entities.User> userManager, IMapper mapper, IRepository<Setting> settingrepo, IToastNotification notification, IRepository<Order> orderRepo, IRepository<OrderDetail> orderDetailRepo, IRepository<ShopCard> shopCardRepo, IRepository<ShopCardDetail> shopCardDetailRepo, IRepository<Products> productrepo, IRepository<Address> addressrepo)
        {
            this.userManager = userManager;
            this.notification = notification;
            this.settingrepo = settingrepo;
            this.mapper = mapper;
            var expose = new Expose();
            _payment = expose.CreatePayment();
            _authority = expose.CreateAuthority();
            _transactions = expose.CreateTransactions();
            OrderRepo = orderRepo;
            OrderDetailRepo = orderDetailRepo;
            ShopCardRepo = shopCardRepo;
            ShopCardDetailRepo = shopCardDetailRepo;
            this.productrepo = productrepo;
            this.addressrepo = addressrepo;
        }

        public async Task<IActionResult> ProductPay(Guid ShopCardId, Guid AddressId, Guid FreeTimeId, CancellationToken cancellationToken)
        {
            var setting = await settingrepo.TableNoTracking.FirstOrDefaultAsync();
            var User = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var ShopCard = await ShopCardRepo.TableNoTracking.FirstOrDefaultAsync(x => x.Id == ShopCardId && x.UserId == User.Id);
            if (ShopCard.TotalPrice < 500000)
            {
                ShopCard.TotalPrice += setting.PostPrice;
            }

            if (ShopCard != null)
            {
                string BaseUrl = $"{base.Request.Scheme}://{base.Request.Host}{base.Request.PathBase}/";
                var result = await _payment.Request(new DtoRequest()
                {
                    Mobile = User.PhoneNumber,
                    CallbackUrl = BaseUrl + "Payment/ProductPaymentConfirmation?ShopCardId=" + ShopCardId.ToString() + "&UserId=" + User.Id + "&AddressId=" + AddressId.ToString() + "&FreeTimeId=" + FreeTimeId.ToString(),
                    Description = "پرداخت آنلاین",
                    Email = User.Email ?? "info@rahrovandanesh.ir",
                    Amount = ShopCard.TotalPrice,
                    MerchantId = setting.MerchantId,
                }, ZarinPal.Class.Payment.Mode.zarinpal);
                return Redirect($"{URLs.gateWayUrl}{result.Authority}");
            }
            notification.AddErrorToastMessage("متاسفانه پرداخت با موفقیت انجام نشد");
            return RedirectToAction("ShopCard_Detail", "ShopCard");
        }

        public async Task<IActionResult> ProductPaymentConfirmation(Guid ShopCardId, Guid UserId, Guid AddressId, Guid FreeTimeId, string Authority, string status, CancellationToken cancellationToken)
        {
            var setting = await settingrepo.TableNoTracking.FirstOrDefaultAsync();
            var ShopCard = await ShopCardRepo.TableNoTracking.FirstOrDefaultAsync(x => x.Id == ShopCardId && x.UserId == UserId);
            if (ShopCard.TotalPrice < 500000)
            {
                ShopCard.TotalPrice += setting.PostPrice;
            }
            if (ShopCard != null)
            {
                var ShopCardDetail = await ShopCardDetailRepo.TableNoTracking.Where(x => x.ShopCardId == ShopCardId).ToListAsync(cancellationToken);
                var Verification = _payment.Verification(new DtoVerification
                {
                    Amount = ShopCard.TotalPrice,
                    MerchantId = setting.MerchantId,
                    Authority = Authority
                }, ZarinPal.Class.Payment.Mode.zarinpal).Result;
                if (Verification.Status == 100 || status == "OK")
                {
                    var orderlistCount = OrderRepo.TableNoTracking.Count();
                    var order = new Order()
                    {
                        PaymentStatus = Entities.Constants.PaymentStatus.Payed,
                        TotalPrice = 0,
                        UserId = UserId,
                        AddressId = AddressId,
                        FreeTimeId = FreeTimeId,
                        DiscountPercent = ShopCard.DiscountPercent,
                        CreationDateTime = DateTime.Now,
                        FactorNumber = 1001 + orderlistCount
                    };
                    OrderRepo.Add(order);
                    //یکی از لیست های زیر برای اپدیت اوردر دیتیل ریپازیتوری هست و اون یکی برای اد کردنه
                    var Listorderdetail1 = new List<OrderDetail>();
                    var Listorderdetail2 = new List<OrderDetail>();
                    var Product = new Products();
                    var ProductList = new List<Products>();
                    string SMS_ProductList = "";
                    foreach (var item in ShopCardDetail)
                    {
                        Product = new();
                        Product = await productrepo.TableNoTracking.FirstOrDefaultAsync(x => x.Id == item.ProductsId);
                        Product.Count -= item.Count;
                        ProductList.Add(Product);
                        SMS_ProductList += item.Count + "عدد " + Product.Name + " , " + "\n";

                        var RepeatedOrderDetail = OrderDetailRepo.TableNoTracking.Where(x => x.OrderId == order.Id && x.ProductsId == item.ProductsId).FirstOrDefault();
                        if (RepeatedOrderDetail != null)
                        {
                            RepeatedOrderDetail.Count += item.Count;
                            RepeatedOrderDetail.CreationDateTime = item.CreationDateTime;
                            Listorderdetail1.Add(RepeatedOrderDetail);
                        }
                        else
                        {
                            var neworderdetail = new OrderDetail()
                            {
                                OrderId = order.Id,
                                ProductsId = item.ProductsId,
                                CreationDateTime = DateTime.Now,
                                Price = item.Price,
                                Count = item.Count,
                            };
                            Listorderdetail2.Add(neworderdetail);
                        }
                    }
                    await productrepo.UpdateRangeAsync(ProductList, cancellationToken);

                    order.TotalPrice += ShopCard.TotalPrice;

                    if (Listorderdetail1 != null)
                    {
                        OrderDetailRepo.UpdateRange(Listorderdetail1);
                    }
                    if (Listorderdetail2 != null)
                    {
                        OrderDetailRepo.AddRange(Listorderdetail2);
                    }
                    ShopCardDetailRepo.DeleteRange(ShopCardDetail);

                    ShopCard.TotalPrice = 0;
                    ShopCard.FinalTotalPrice = 0;
                    ShopCard.DiscountPercent = 0;
                    ShopCardRepo.Update(ShopCard);
                    OrderRepo.Update(order);
                    notification.AddSuccessToastMessage("پرداخت با موفقیت انجام شد");
                    var User = await userManager.FindByIdAsync(UserId.ToString());
                    var UserAddress = addressrepo.TableNoTracking.Where(x => x.ClientId == User.Id).ProjectTo<AddressDto>(mapper.ConfigurationProvider).FirstOrDefault();
                    var SMS_Address = "آدرس: \n" + UserAddress.CityName + "/" + UserAddress.Location + "/" + "کد پستی:" + UserAddress.PostalCode + "/" + "واحد:" + UserAddress.vahed + "/" + "پلاک:" + UserAddress.pelak;
                    //MeliPayamak.Simple_Rest(User.PhoneNumber, "سفارش " + User.Fname + " " + User.Lname + "\n" + SMS_ProductList + "\n" + "در حال پردازش است");
                    MeliPayamak.Simple_Rest(User.PhoneNumber,$"نگین هایپری عزیز \n سفارش شما به شماره فاکتور:{order.FactorNumber} به مبلغ {order.TotalPrice.ToNumeric()} تومان با شماره پیگیری {Verification.RefId} با موفقیت ثبت شد. \n" +
                        $" مشاهده وضعیت سفارش: gilhyper.com/Order/OrderList \n پایدار,محلی و آگاهانه \n با عشق \n www.GilHyper.com  ");
                    MeliPayamak.Simple_Rest(setting.PHoneNumber1, "سفارش " + User.Fname + " " + User.Lname + "\n" + SMS_ProductList + "\n" + SMS_Address + "\n" + "در حال پردازش است");
                    MeliPayamak.Simple_Rest(setting.PHoneNumber2, "سفارش " + User.Fname + " " + User.Lname + "\n" + SMS_ProductList + "\n" + SMS_Address + "\n" + "در حال پردازش است");
                    return RedirectToAction("OrderList", "Order");
                }
            }
            notification.AddErrorToastMessage("متاسفانه پرداخت با موفقیت انجام نشد");
            return RedirectToAction("index", "Home");
        }
        public async Task<IActionResult> PaymentDetail(CancellationToken cancellationToken)
        {
            var setting = await settingrepo.TableNoTracking.FirstOrDefaultAsync();
            var User = await userManager.Users.Where(x => x.UserName == HttpContext.User.Identity.Name).ProjectTo<UserDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync();
            var ShopCard = await ShopCardRepo.TableNoTracking.ProjectTo<ShopCardDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.UserId == User.Id);
            User.PostPrice = setting.PostPrice;
            User.ShopCardDetailCount = ShopCard.ShopCardDetails.Count();
            User.Price = ShopCard.TotalPrice;
            User.TotalPrice = setting.PostPrice + ShopCard.TotalPrice;
            User.ShopcardId = ShopCard.Id;
            return View(User);
        }
    }
}

