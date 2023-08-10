using Secretary.Models;
using AmootSMS;
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

namespace Secretary.Controllers
{
    public class EducationController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IRepository<Entities.Course> Courserepo;
        private readonly IRepository<UserCourse> UserCourseRepo;
        private readonly IRepository<Season> Seasonrepo;
        private readonly IRepository<Lesson> Lessonrepo;
        private readonly IMapper mapper;
        public EducationController(UserManager<User> userManager, IMapper mapper, IRepository<Entities.Course> Courserepo, IRepository<Season> Seasonrepo, IRepository<Lesson> Lessonrepo, IRepository<UserCourse> UserCourseRepo)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.Courserepo = Courserepo;
            this.Seasonrepo = Seasonrepo;
            this.Lessonrepo = Lessonrepo;
            this.UserCourseRepo = UserCourseRepo;
        }
        #region Course Crud

        public async Task<IActionResult> Add_Edit_Course(Guid? CourseId, CancellationToken cancellationToken)
        {
            var model = await Courserepo.TableNoTracking.Where(x => x.Id == CourseId).ProjectTo<CourseDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add_Edit_Course(CourseDto Dto, CancellationToken cancellationToken)
        {
            if (Dto.Id == Guid.Empty)
            {
                var Course = Dto.ToEntity(mapper);
                Course.ImageLink = UploadImage.SaveImage(Dto.Image, "CourseImage");
                await Courserepo.AddAsync(Course, cancellationToken);
            }
            else
            {
                var model = await Courserepo.TableNoTracking.Where(x => x.Id == Dto.Id).FirstOrDefaultAsync(cancellationToken);
                if (Dto.Image != null && Dto.Image.Length > 0)
                {
                    Dto.ImageLink = UploadImage.SaveImage(Dto.Image, "CourseImage");
                }
                else
                {
                    Dto.ImageLink = model.ImageLink;
                }
                var Course = Dto.ToEntity(mapper, model);
                await Courserepo.UpdateAsync(Course, cancellationToken);
            }
            return RedirectToAction("CourseList");
        }
        public async Task<IActionResult> CourseList(CancellationToken cancellationToken)
        {
            var model = await Courserepo.TableNoTracking.ProjectTo<CourseDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            return View(model);
        }
        public async Task<IActionResult> CourseDetail(Guid CourseId, CancellationToken cancellationToken)
        {
            var model = await Courserepo.TableNoTracking.Where(x => x.Id == CourseId).ProjectTo<CourseDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            return View(model);
        }
        public async Task<IActionResult> DeleteCourse(Guid CourseId, CancellationToken cancellationToken)
        {
            var Course = await Courserepo.TableNoTracking.Where(x => x.Id == CourseId).FirstOrDefaultAsync();
            Course.Active = false;
            await Courserepo.UpdateAsync(Course, cancellationToken);
            return RedirectToAction("CourseList");
        }
        #endregion

        #region Season Crud
        public async Task<IActionResult> Add_Edit_Season(Guid? SeasonId, CancellationToken cancellationToken)
        {
            var model = await Seasonrepo.TableNoTracking.Where(x => x.Id == SeasonId).ProjectTo<SeasonDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add_Edit_Season(SeasonDto Dto, CancellationToken cancellationToken)
        {
            Guid CourseId = (Guid)TempData["CourseId"];
            if (Dto.Id == Guid.Empty)
            {
                var Season = Dto.ToEntity(mapper);
                Season.CourseId = CourseId;
                await Seasonrepo.AddAsync(Season, cancellationToken);
            }
            else
            {
                var model = await Seasonrepo.TableNoTracking.Where(x => x.Id == Dto.Id).FirstOrDefaultAsync(cancellationToken);
                var Season = Dto.ToEntity(mapper, model);
                Season.CourseId = CourseId;
                await Seasonrepo.UpdateAsync(Season, cancellationToken);
            }
            return RedirectToAction("SeasonList", new { CourseId = CourseId });
        }
        public async Task<IActionResult> SeasonList(Guid CourseId, CancellationToken cancellationToken)
        {
            TempData["CourseId"] = CourseId;
            var model = await Seasonrepo.TableNoTracking.Where(x => x.CourseId == CourseId).ProjectTo<SeasonDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            return View(model);
        }
        public async Task<IActionResult> SeasonDetail(Guid SeasonId, CancellationToken cancellationToken)
        {
            var model = await Seasonrepo.TableNoTracking.Where(x => x.Id == SeasonId).ProjectTo<SeasonDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            return View(model);
        }
        public async Task<IActionResult> DeleteSeason(Guid SeasonId, Guid CourseId, CancellationToken cancellationToken)
        {
            var Season = await Seasonrepo.TableNoTracking.Where(x => x.Id == SeasonId).FirstOrDefaultAsync();
            Season.Active = false;
            await Seasonrepo.UpdateAsync(Season, cancellationToken);
            return RedirectToAction("SeasonList", new { CourseId = CourseId });
        }
        #endregion

        #region Lesson Crud
        public async Task<IActionResult> Add_Edit_Lesson(Guid? LessonId, Guid CourseId, Guid SeasonId, CancellationToken cancellationToken)
        {
            var model = await Lessonrepo.TableNoTracking.Where(x => x.Id == LessonId).ProjectTo<LessonDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add_Edit_Lesson(LessonDto Dto, CancellationToken cancellationToken)
        {
            Guid CourseId = (Guid)TempData["CourseId"];
            Guid SeasonId = (Guid)TempData["SeasonId"];
            if (Dto.Id == Guid.Empty)
            {
                var Lesson = Dto.ToEntity(mapper);
                Lesson.CourseId = CourseId;
                Lesson.SeasonId = SeasonId;
                Lesson.Link = UploadImage.SaveImage(Dto.Video, "LessonVideo");
                await Lessonrepo.AddAsync(Lesson, cancellationToken);
            }
            else
            {
                var model = await Lessonrepo.TableNoTracking.Where(x => x.Id == Dto.Id).FirstOrDefaultAsync(cancellationToken);
                var Lesson = Dto.ToEntity(mapper, model);
                Lesson.CourseId = CourseId;
                Lesson.SeasonId = SeasonId;
                if (Dto.Video != null && Dto.Video.Length > 0)
                {
                    Lesson.Link = UploadImage.SaveImage(Dto.Video, "LessonVideo");
                }
                else
                {
                    Lesson.Link = model.Link;
                }
                await Lessonrepo.UpdateAsync(Lesson, cancellationToken);
            }
            return RedirectToAction("LessonList", new { CourseId = CourseId, SeasonId = SeasonId });
        }
        public async Task<IActionResult> LessonList(Guid CourseId, Guid SeasonId, CancellationToken cancellationToken)
        {
            TempData["CourseId"] = CourseId;
            TempData["SeasonId"] = SeasonId;
            var model = await Lessonrepo.TableNoTracking.Where(x => x.CourseId == CourseId && x.SeasonId == SeasonId).ProjectTo<LessonDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            return View(model);
        }
        public async Task<IActionResult> LessonDetail(Guid LessonId, CancellationToken cancellationToken)
        {
            var model = await Lessonrepo.TableNoTracking.Where(x => x.Id == LessonId).ProjectTo<LessonDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            return View(model);
        }
        public async Task<IActionResult> DeleteLesson(Guid CourseId, Guid SeasonId, Guid LessonId, CancellationToken cancellationToken)
        {
            var Lesson = await Lessonrepo.TableNoTracking.Where(x => x.Id == LessonId).FirstOrDefaultAsync();
            Lesson.Active = false;
            await Lessonrepo.UpdateAsync(Lesson, cancellationToken);
            return RedirectToAction("LessonList", new { CourseId = CourseId, SeasonId = SeasonId });
        }
        #endregion

        #region Signed Up Course
        public async Task<IActionResult> SignedUpUserList(Guid CourseId, CancellationToken cancellationToken)
        {
            var UserCourseList = UserCourseRepo.TableNoTracking.Where(x => x.CourseId == CourseId).ProjectTo<UserCourseDto>(mapper.ConfigurationProvider).ToList();
            return View(UserCourseList);
        }
        public async Task<IActionResult> AllSignedUpUserList(CancellationToken cancellationToken)
        {

            var UserCourseList = UserCourseRepo.TableNoTracking.ProjectTo<UserCourseDto>(mapper.ConfigurationProvider).ToList();
            return View(UserCourseList);
        }

        #endregion

    }
}
