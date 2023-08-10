using Data.Repositories;
using Entities;
using Entities.Address;
using Entities.BlogComnent;
using Entities.CityProvince;
using Entities.Constants;
using Entities.FreeTime;
using Entities.Notifications;
using Entities.Orders;
using Entities.Product;
using Entities.ProductComment;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;


namespace Services.DataInitializer
{
    public class GeneralDataInitializer : IDataInitializer
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly IRepository<BlogCategory> Blogcategoryrepo;
        private readonly IRepository<ProductCategory> Productcategoryrepo;
        private readonly IRepository<Products> Productrepo;
        private readonly IRepository<FreeTime> freeTimeRepo;
        private readonly IRepository<ProductImage> ProductImagerepo;
        private readonly IRepository<Blog> Blogrepo;
        private readonly IRepository<Contact> Contactrepo;
        private readonly IRepository<Order> Orderrepo;
        private readonly IRepository<Address> addressRepo;
        private readonly IRepository<OrderDetail> Orderdetailrepo;
        private readonly IRepository<Notification> Notificationrepo;
        private readonly IRepository<BlogComment> BlogCommentrepo;
        private readonly IRepository<ProductComments> ProducCommenttrepo;
        private readonly IRepository<Setting> settingrepo;
        private readonly IRepository<Discount> discountRipo;
        private readonly IRepository<Province> RepoProvince;
        private readonly IRepository<City> RepoCity;

        public GeneralDataInitializer(UserManager<User> userManager, RoleManager<Role> roleManager, IRepository<BlogCategory> blogcategoryrepo, IRepository<ProductCategory> productcategoryrepo, IRepository<Products> productrepo, IRepository<Blog> blogrepo, IRepository<Contact> contactrepo, IRepository<Order> orderrepo, IRepository<OrderDetail> orderdetailrepo, IRepository<Notification> notificationrepo, IRepository<BlogComment> blogCommentrepo, IRepository<ProductComments> producCommenttrepo, IRepository<Setting> settingrepo, IRepository<ProductImage> productImagerepo, IRepository<FreeTime> freeTimeRepo, IRepository<Address> addressRepo, IRepository<Discount> discountRipo, IRepository<Province> repoProvince, IRepository<City> repoCity)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            Blogcategoryrepo = blogcategoryrepo;
            Productcategoryrepo = productcategoryrepo;
            Productrepo = productrepo;
            Blogrepo = blogrepo;
            Contactrepo = contactrepo;
            Orderrepo = orderrepo;
            Orderdetailrepo = orderdetailrepo;
            Notificationrepo = notificationrepo;
            BlogCommentrepo = blogCommentrepo;
            ProducCommenttrepo = producCommenttrepo;
            this.settingrepo = settingrepo;
            ProductImagerepo = productImagerepo;
            this.freeTimeRepo = freeTimeRepo;
            this.addressRepo = addressRepo;
            this.discountRipo = discountRipo;
            RepoProvince = repoProvince;
            RepoCity = repoCity;
        }

        public void InitializeData()
        {
            if (!roleManager.RoleExistsAsync("Client").GetAwaiter().GetResult())
            {
                roleManager.CreateAsync(new Role { Name = "Admin", Description = "Admin role" }).GetAwaiter().GetResult();
                roleManager.CreateAsync(new Role { Name = "Client", Description = "Client role" }).GetAwaiter().GetResult();
            }
            if (!userManager.Users.AsNoTracking().Any(p => p.UserName == "admin"))
            {
                var user = new User
                {
                    Code = "1000",
                    Fname = "mohammad",
                    Lname = "razavi",
                    PhoneNumber = "09385859688",
                    Gender = GenderType.Male,
                    UserName = "admin",
                    Email = "admin@site.com"
                };
                userManager.CreateAsync(user, "123456").GetAwaiter().GetResult();
                userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
            }
            if (!userManager.Users.AsNoTracking().Any(p => p.UserName == "09387285366"))
            {
                var user2 = new User
                {
                    Code = "1000",
                    Fname = "mohammad",
                    Lname = "razavi",
                    PhoneNumber = "09387285366",
                    Gender = GenderType.Male,
                    UserName = "09387285366",
                    Email = "admin2@site.com"
                };
                userManager.CreateAsync(user2, "123456").GetAwaiter().GetResult();
                userManager.AddToRoleAsync(user2, "Client").GetAwaiter().GetResult();
            }

            #region Product

            //if (!Blogcategoryrepo.TableNoTracking.Any())
            //{
            //    Blogcategoryrepo.Add(new BlogCategory
            //    {
            //        Name = "دسته بندی دیفالت",
            //        Code = "1",
            //    });
            //}
            if (!Productcategoryrepo.TableNoTracking.Any())
            {
                Productcategoryrepo.Add(new ProductCategory
                {
                    Name = "ادکلن و اسپری",
                    Code = "1001",
                    ImageUrl = "http://Admin.neginhyper.Com/assets/media/icons/bag-min-q2jrrojeaihmvl73irizu1y21it7ejeanmadobbn54.png",
                    Description = " This is a wider card with supporting text below as a natural\r\n           " +
                    "                     lead-in to additional content. This content is a little bit\r\n      " +
                    "            longer.a natural lead-in to additional content. This content is\r\n           " +
                    "             a little bit longer.a natural lead-in to additional content.\r\n             " +
                    "           This content is a little bit longer.a natural lead-in to\r\n                   " +
                    "           additional content. This content is a little bit longer.a\r\n                  " +
                    "           natural lead-in to additional content. This content is a little\r\n            " +
                    "                 bit longer.a natural lead-in to additional content. This content\r\n     " +
                    "                      is a little bit longer.",
                });
                Productcategoryrepo.Add(new ProductCategory
                {
                    Name = "دکمه",
                    Code = "1001",
                    ImageUrl = "http://Admin.neginhyper.Com/assets/media/icons/bag.png",
                    Description = " This is a wider card with supporting text below as a natural\r\n           " +
                    "                     lead-in to additional content. This content is a little bit\r\n      " +
                    "            longer.a natural lead-in to additional content. This content is\r\n           " +
                    "             a little bit longer.a natural lead-in to additional content.\r\n             " +
                    "           This content is a little bit longer.a natural lead-in to\r\n                   " +
                    "           additional content. This content is a little bit longer.a\r\n                  " +
                    "           natural lead-in to additional content. This content is a little\r\n            " +
                    "                 bit longer.a natural lead-in to additional content. This content\r\n     " +
                    "                      is a little bit longer.",


                });
                Productcategoryrepo.Add(new ProductCategory
                {
                    Name = "گیر و کش مو",
                    Code = "1001",
                    ImageUrl = "http://Admin.neginhyper.Com/assets/media/icons/flour.png",
                    Description = " This is a wider card with supporting text below as a natural\r\n           " +
                    "                     lead-in to additional content. This content is a little bit\r\n      " +
                    "            longer.a natural lead-in to additional content. This content is\r\n           " +
                    "             a little bit longer.a natural lead-in to additional content.\r\n             " +
                    "           This content is a little bit longer.a natural lead-in to\r\n                   " +
                    "           additional content. This content is a little bit longer.a\r\n                  " +
                    "           natural lead-in to additional content. This content is a little\r\n            " +
                    "                 bit longer.a natural lead-in to additional content. This content\r\n     " +
                    "                      is a little bit longer.",

                });
               
            }
            if (!Productrepo.TableNoTracking.Any())
            {
                for (int i = 1; i < 14; i++)
                {
                    Productrepo.Add(new Products
                    {
                        Name = "محصول تست" + i,
                        Active = true,
                        Count = 100,
                        CategorysId = Productcategoryrepo.TableNoTracking.FirstOrDefault(x => x.Name == "دکمه").Id,
                        ImageCoverUrl = "http://Admin.neginhyper.Com/ProductImage/1.png",
                        Price = 50000,
                        Discount = 25000,
                        Percent = 50,
                        Rate = 5,
                        TotalRate = 5,
                        Pureweight = "300gr",
                        Allweight = "350gr",
                        Detail = "محصول از دسته بندی تست یک ",
                        Number = 1,
                        CreationDateTime = DateTime.Now,
                        GoodForWhat = "همه سنین",
                        KeepTime = "هشت ماه",
                        KeepWay = "در جای خنک نگهداری شود",
                    });
                }
            }
            //if (!ProductImagerepo.TableNoTracking.Any())
            //{
            //    for (int i = 1; i < 14; i++)
            //    {
            //        ProductImagerepo.Add(new ProductImage
            //        {
            //            ProductId = Productrepo.TableNoTracking.FirstOrDefault(x => x.Name == "محصول تست" + i).Id,
            //            ImageLink = "http://Admin.neginhyper.Com/ProductImage/09876787-123a-4063-a027-38131ecc6bb8on-glutamine-powder-600-g.jpg",
            //        });
            //        ProductImagerepo.Add(new ProductImage
            //        {
            //            ProductId = Productrepo.TableNoTracking.FirstOrDefault(x => x.Name == "محصول تست" + i).Id,
            //            ImageLink = "http://Admin.neginhyper.Com/ProductImage/f27fe86b-be0f-48b0-880f-c4bf438212c12135-2167-2022-06-22 54541111.jpg",
            //        });
            //        ProductImagerepo.Add(new ProductImage
            //        {
            //            ProductId = Productrepo.TableNoTracking.FirstOrDefault(x => x.Name == "محصول تست" + i).Id,
            //            ImageLink = "http://Admin.neginhyper.Com/ProductImage/7828c67d-51e3-49a8-b8e3-f6bf60ec19b1ed8c2b62e0c4c2be11867ea682e866c19b97a125_1624426907.jpg",
            //        });
            //    }

            //}
            //if (!ProducCommenttrepo.TableNoTracking.Any())
            //{
            //    ProducCommenttrepo.Add(new ProductComments
            //    {
            //        Name = "ehsan",
            //        Active = true,
            //        Rate = 5,
            //        CommentStatus = CommentStatus.Accepted,
            //        Text = "متن تست شماره ی بیست",
            //        ProductId = Productrepo.TableNoTracking.FirstOrDefault(x => x.Name == "محصول تست1").Id,
            //        Email = "a@b.com",
            //        UserId = userManager.Users.FirstOrDefault(x => x.Fname == "mohammad").Id,
            //        CreationDateTime = DateTime.Now
            //    });
            //}
            //if (!ProducCommenttrepo.TableNoTracking.Any())
            //{
            //    ProducCommenttrepo.Add(new ProductComments
            //    {
            //        Name = "رضا",
            //        Active = true,
            //        Rate = 4,
            //        CommentStatus = CommentStatus.Accepted,
            //        Text = "متن تست شماره ی بیست",
            //        ProductId = Productrepo.TableNoTracking.FirstOrDefault(x => x.Name == "محصول تست2").Id,
            //        Email = "a@b.com",
            //        UserId = userManager.Users.FirstOrDefault(x => x.Fname == "mohammad").Id,
            //        CreationDateTime = DateTime.Now
            //    });
            //}
            #endregion

            if (!Contactrepo.TableNoTracking.Any())
            {
                Contactrepo.Add(new Contact
                {
                    Description = "توضیحات",
                    Active = true,
                    Name = "mohsen",
                    Title = "working",
                    PhoneNumber = "09877899889",
                    CreationDateTime = DateTime.Now,
                });
            }
            if (!RepoProvince.AllTableNoTracking.Any())
            {
                foreach (var i in DNTPersianUtils.Core.IranCities.Iran.Provinces)
                {
                    var province = new Province()
                    {
                        Name = i.ProvinceName,
                    };
                    RepoProvince.Add(province);
                    foreach (var item in DNTPersianUtils.Core.IranCities.Iran.Cities)
                    {
                        if (item.ProvinceName == i.ProvinceName)
                        {
                            var city = new City()
                            {
                                Name = item.CityName,
                                ProvinceId = province.Id
                            };
                            RepoCity.Add(city);
                        }
                    }
                }
            }
            if (!addressRepo.TableNoTracking.Any())
            {
                addressRepo.Add(new Address
                {
                    Location = "شیراز-فرهنگ شهر - کوچه 33",
                    Name = "mohammad",
                    PhoneNumber = "09878788765",
                    CityId = RepoCity.TableNoTracking.FirstOrDefaultAsync().Result.Id,
                    ClientId = userManager.Users.FirstOrDefault(x => x.Fname == "mohammad").Id,
                });
            }
            if (!freeTimeRepo.TableNoTracking.Any())
            {
                for (int i = 0; i < 7; i++)
                {
                    DateTime date = DateTime.Now.AddDays(i);
                    for (int j = 10; j < 22; j += 3)
                    {
                        freeTimeRepo.Add(new FreeTime
                        {
                            Day = (i + 1).ToString(),
                            FromHour = j.ToString(),
                            ToHour = (j + 2).ToString(),
                            DateTime = date,
                            Free = true,
                        });
                    }
                }
            }
            if (!Orderrepo.TableNoTracking.Any())
            {
                Orderrepo.Add(new Order
                {
                    Active = true,
                    UserId = userManager.Users.FirstOrDefault(x => x.Fname == "mohammad").Id,
                    FreeTimeId = freeTimeRepo.TableNoTracking.FirstOrDefault(x => x.Day == "1").Id,
                    AddressId = addressRepo.TableNoTracking.FirstOrDefault(x => x.Name == "mohammad").Id,
                    CreationDateTime = DateTime.Now,
                    TotalPrice = 200,
                    PaymentStatus = PaymentStatus.Waiting,
                });
            }
            if (!Productrepo.TableNoTracking.Any())
            {
                Orderdetailrepo.Add(new OrderDetail
                {
                    Count = 3,
                    Price = 230000,
                    Active = true,
                    ProductsId = Productrepo.TableNoTracking.FirstOrDefault().Id,
                    OrderId = Orderrepo.TableNoTracking.FirstOrDefault(x => x.TotalPrice == 200).Id,
                    CreationDateTime = DateTime.Now
                });
            }
            if (!Orderdetailrepo.TableNoTracking.Any())
            {
                Orderdetailrepo.Add(new OrderDetail
                {
                    Count = 3,
                    Price = 230000,
                    Active = true,
                    ProductsId = Productrepo.TableNoTracking.FirstOrDefault().Id,
                    OrderId = Orderrepo.TableNoTracking.FirstOrDefault(x => x.TotalPrice == 200).Id,
                    CreationDateTime = DateTime.Now
                });
            }
            if (!Notificationrepo.TableNoTracking.Any())
            {
                Notificationrepo.Add(new Notification
                {
                    Topic = "تاییدیه",
                    Active = true,
                    UserId = userManager.Users.FirstOrDefault(x => x.UserName == "admin").Id,
                    DateTimes = DateTime.Now,
                    Text = "متن تست شماره یک",
                    CreationDateTime = DateTime.Now
                });
            }

            if (!settingrepo.TableNoTracking.Any())
            {
                settingrepo.Add(new Setting
                {
                    PostPrice = 30000,
                    FreePost = 200000,
                    MerchantId = "",
                    JoinCost = "200",
                    VerifyLink = "http://Admin.neginhyper.Com/dashboard/sitesetting",
                    Active = true,
                    CreationDateTime = DateTime.Now
                });
            }
            if (!discountRipo.TableNoTracking.Any())
            {
                discountRipo.Add(new Discount
                {
                    Name = "تخفیف برای خرید اول",
                    Code = "F-21-F",
                    DiscountPercent = 20,
                    CreationDateTime = DateTime.Now
                });
            }
        }
    }
}