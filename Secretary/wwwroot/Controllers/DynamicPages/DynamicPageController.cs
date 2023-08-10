using Secretary.Models.DynamicPages;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Repositories;
using Entities.DynamicPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Secretary.Controllers.DynamicPages
{
    [Authorize(Roles = "Admin")]
    public class DynamicPageController : Controller
    {
        private readonly IMapper mapper;
        private readonly IRepository<IndexData> IndexDataRepo;
        private readonly IRepository<MasterPage> MasterPageRepo;
        private readonly IRepository<MasterpageGallery> MasterpageGalleryRepo;
        private readonly IRepository<ContactPage> ContactPageRepo;
        private readonly IRepository<HeaderFooterData> HeaderFooterDataRepo;

        public DynamicPageController(IMapper mapper, IRepository<IndexData> indexDataRepo,
            IRepository<MasterPage> masterPageRepo, IRepository<MasterpageGallery> masterpageGalleryRepo,
            IRepository<ContactPage> contactPageRepo, IRepository<HeaderFooterData> headerFooterDataRepo)
        {
            this.mapper = mapper;
            IndexDataRepo = indexDataRepo;
            MasterPageRepo = masterPageRepo;
            MasterpageGalleryRepo = masterpageGalleryRepo;
            ContactPageRepo = contactPageRepo;
            HeaderFooterDataRepo = headerFooterDataRepo;
        }
        #region Header_Footer

        public async Task<IActionResult> HeaderFooter()
        {
            var model = await HeaderFooterDataRepo.TableNoTracking.ProjectTo<HeaderFooterDataDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> HeaderFooter(HeaderFooterDataDto Dto, CancellationToken cancellationToken)
        {
            var model = await HeaderFooterDataRepo.TableNoTracking.FirstOrDefaultAsync();
            if (Dto.LogoFile != null)
            {
                Dto.LogoLink = UploadImage.SaveImage(Dto.LogoFile, "LogoFile");
            }
            else
            {
                Dto.LogoLink = model.LogoLink;
            }
            Guid id = model.Id;
            model = Dto.ToEntity(mapper, model);
            model.Id = id;
            await HeaderFooterDataRepo.UpdateAsync(model, cancellationToken);
            return View();
        }

        #endregion

        #region Index

        public async Task<IActionResult> IndexData()
        {
            var model = await IndexDataRepo.TableNoTracking.ProjectTo<IndexDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> IndexData(IndexDto Dto, CancellationToken cancellationToken)
        {
            var model = await IndexDataRepo.TableNoTracking.FirstOrDefaultAsync();
            Guid id = model.Id;

            model = Dto.ToEntity(mapper, model);
            model.Id = id;

            await IndexDataRepo.UpdateAsync(model, cancellationToken);
            return View();
        }
        #endregion

        #region MasterPage
        public async Task<IActionResult> MasterPage_List()
        {
            var model = await MasterPageRepo.TableNoTracking.ProjectTo<MasterPageDto>(mapper.ConfigurationProvider).ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> MasterPage(Guid Id)
        {
            var model = await MasterPageRepo.TableNoTracking.ProjectTo<MasterPageDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Id == Id);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> MasterPage(MasterPageDto Dto, CancellationToken cancellationToken)
        {
            var model = await MasterPageRepo.TableNoTracking.FirstOrDefaultAsync(x => x.Id == Dto.Id);
            Guid id = model.Id;
            if (model.PageType == MasterPageType.Programming)
            {
                Dto.PageType = MasterPageType.Programming;
            }
            else
            {
                Dto.PageType = MasterPageType.Architecture;
            }
            model = Dto.ToEntity(mapper, model);
            model.Id = id;

            await MasterPageRepo.UpdateAsync(model, cancellationToken);

            return RedirectToAction(nameof(MasterPage_List));
        }

        public async Task<IActionResult> MasterPageGallery_List(Guid MasterPageId)
        {
            var model = await MasterpageGalleryRepo.TableNoTracking.Where(x => x.MasterPageId == MasterPageId).ProjectTo<MasterpageGalleryDto>(mapper.ConfigurationProvider).ToListAsync();
            TempData["MasterPageId"] = MasterPageId;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> MasterPageGallery_Add(MasterpageGalleryDto Dto, CancellationToken cancellationToken)
        {
            if (Dto.ImageFile != null)
            {
                Dto.ImageLink = UploadImage.SaveImage(Dto.ImageFile, "MasterPageGalleryImages");
            }
            if (TempData["MasterPageId"] != null)
            {
                Dto.MasterPageId = Guid.Parse(TempData["MasterPageId"].ToString());
            }
            var model = Dto.ToEntity(mapper);
            await MasterpageGalleryRepo.AddAsync(model, cancellationToken);
            return LocalRedirect($"/Dynamicpage/MasterPageGallery_List?MasterPageId={Dto.MasterPageId}");
        }
        public async Task<IActionResult> MasterPageGallery_Delete(Guid Id, CancellationToken cancellationToken)
        {
            var model = await MasterpageGalleryRepo.TableNoTracking.FirstOrDefaultAsync(x => x.Id == Id);
            var MasterPageId = model.MasterPageId;
            await MasterpageGalleryRepo.DeleteAsync(model, cancellationToken);
            return LocalRedirect($"/Dynamicpage/MasterPageGallery_List?MasterPageId={MasterPageId}");
        }
        #endregion

        #region Contact

        public async Task<IActionResult> ContactData_List()
        {
            var model = await ContactPageRepo.TableNoTracking.ProjectTo<ContactPageDto>(mapper.ConfigurationProvider).ToListAsync();
            return View(model);
        }
        public async Task<IActionResult> ContactData(Guid Id)
        {
            var model = await ContactPageRepo.TableNoTracking.Where(x => x.Id == Id).ProjectTo<ContactPageDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ContactData(ContactPageDto Dto, CancellationToken cancellationToken)
        {
            var model = await ContactPageRepo.TableNoTracking.FirstOrDefaultAsync(x => x.Id == Dto.Id);
            Guid id = model.Id;
            if (model.PageType == ContactPageType.Contact)
            {
                Dto.PageType = ContactPageType.Contact;
            }
            else if (model.PageType == ContactPageType.Internship)
            {
                Dto.PageType = ContactPageType.Internship;
            }
            else
            {
                Dto.PageType = ContactPageType.Counseling;

            }
            model = Dto.ToEntity(mapper, model);
            model.Id = id;

            await ContactPageRepo.UpdateAsync(model, cancellationToken);
            return LocalRedirect($"/Dynamicpage/ContactData_List");
        }
        #endregion
    }
}
