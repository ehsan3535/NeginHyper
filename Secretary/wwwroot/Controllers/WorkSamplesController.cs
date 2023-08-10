using Admin.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    public class WorkSampleController : Controller
    {

        private readonly UserManager<User> userManager;
        private readonly IRepository<WorkSample> worksample;
        private readonly IMapper mapper;
        public WorkSampleController(UserManager<User> userManager, IMapper mapper, IRepository<WorkSample> worksample)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.worksample = worksample;
        }
        public async Task<IActionResult> Add_Edit_WorkSample(Guid? WorkSampleId, CancellationToken cancellationToken)
        {
            if (WorkSampleId is not null)
            {
                var model = await worksample.TableNoTracking.Where(x => x.Id == WorkSampleId).ProjectTo<WorkSampleDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add_Edit_WorkSample(WorkSampleDto Dto, CancellationToken cancellationToken)
        {
            if (Dto.Id == Guid.Empty)
            {
                var WorkSample = Dto.ToEntity(mapper);
                WorkSample.ImageLink = UploadImage.SaveImage(Dto.Image, "WorkSampeImage");
                WorkSample.PdfLink = UploadImage.SaveImage(Dto.Pdf, "WorkSampeImage");
                await worksample.AddAsync(WorkSample, cancellationToken);
            }
            else
            {
                var model = await worksample.TableNoTracking.Where(x => x.Id == Dto.Id).FirstOrDefaultAsync(cancellationToken);
                var WorkSample = Dto.ToEntity(mapper, model);
                WorkSample.ImageLink = UploadImage.SaveImage(Dto.Image, "WorkSampeImage");
                WorkSample.PdfLink = UploadImage.SaveImage(Dto.Pdf, "WorkSampePdf");
                await worksample.UpdateAsync(WorkSample, cancellationToken);
            }
            return RedirectToAction("WorkSampleList");
        }

        public async Task<IActionResult> WorkSampleList(Guid ContactId, CancellationToken cancellationToken)
        {
            var model = await worksample.TableNoTracking.ProjectTo<WorkSampleDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            return View(model);
        }
        public async Task<IActionResult> DeleteWorkSample(Guid WorkSampleId, CancellationToken cancellationToken)
        {
            var WorkSample = await worksample.TableNoTracking.Where(x => x.Id == WorkSampleId).FirstOrDefaultAsync();
            WorkSample.Active = false;
            await worksample.UpdateAsync(WorkSample, cancellationToken);
            return RedirectToAction("WorkSampleList");
        }
        public async Task<IActionResult> WorkSampleDetail(Guid WorkSampleId, CancellationToken cancellationToken)
        {
            var model = await worksample.TableNoTracking.Where(x => x.Id == WorkSampleId).ProjectTo<WorkSampleDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            return View(model);
        }
    }
}
