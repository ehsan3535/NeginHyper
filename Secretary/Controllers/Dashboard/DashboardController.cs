using AutoMapper;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Secretary.Controllers.Dashboard
{
    [Authorize(Roles = "Admin")]

    public class DashboardController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;

        public DashboardController(UserManager<User> userManager, IMapper mapper)
        {
            this.userManager = userManager;
            this.mapper = mapper;
        }
        public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
        {
            return View();
        }
    }
}
