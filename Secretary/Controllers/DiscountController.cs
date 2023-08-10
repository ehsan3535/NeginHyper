using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Secretary.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Secretary.Controllers
{
    [Authorize(Roles = "Admin")]

    public class DiscountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IRepository<Discount> disCountRepo;
        private readonly IRepository<ProductCategory> Productcategoryrepo;
        private readonly IMapper mapper;

        public DiscountController(UserManager<User> userManager, IRepository<Discount> disCountRepo, IRepository<ProductCategory> productcategoryrepo, IMapper mapper)
        {
            this.userManager = userManager;
            this.disCountRepo = disCountRepo;
            Productcategoryrepo = productcategoryrepo;
            this.mapper = mapper;
        }


        #region ProductCategory
        public async Task<IActionResult> Add_Edit_Discount(Guid? DiscountId)
        {
            if (DiscountId != Guid.Empty)
            {
                var model = disCountRepo.TableNoTracking.Where(x => x.Id == DiscountId).ProjectTo<DiscountDto>(mapper.ConfigurationProvider).FirstOrDefault();
                return View(model);
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Add_Edit_Discount(DiscountDto dto, CancellationToken cancellationToken)
        {
            if (dto.Id != Guid.Empty)
            {
                var model = await disCountRepo.TableNoTracking.Where(x => x.Id == dto.Id).FirstOrDefaultAsync(cancellationToken);
                var Discount = dto.ToEntity(mapper, model);
                await disCountRepo.UpdateAsync(Discount, cancellationToken);
            }
            else
            {
                var model = dto.ToEntity(mapper);
                await disCountRepo.AddAsync(model, cancellationToken);
            }
            return RedirectToAction("DiscountList");
        }
        public async Task<IActionResult> DiscountList(CancellationToken cancellationToken)
        {
            var model = disCountRepo.TableNoTracking.ProjectTo<DiscountDto>(mapper.ConfigurationProvider).ToList();
            return View(model);
        }
        public async Task<IActionResult> DeleteDiscount(Guid DiscountId, CancellationToken cancellationToken)
        {
            var model = disCountRepo.GetById(DiscountId);
            if (model != null)
            {
                model.Active = false;
                await disCountRepo.UpdateAsync(model, cancellationToken);
                return RedirectToAction("DiscountList");
            }
            return View();
        }
        #endregion
    }
}
