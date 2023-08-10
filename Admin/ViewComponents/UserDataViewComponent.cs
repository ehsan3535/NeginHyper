using AutoMapper;
using AutoMapper.QueryableExtensions;
using Client.Models;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.ViewComponents
{
    public class UserDataViewComponent : ViewComponent
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        public UserDataViewComponent(UserManager<User> userManager, IMapper mapper)
        {
            this.userManager = userManager;
            this.mapper = mapper;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var User = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var model = await userManager.Users.Where(x => x.Id == User.Id).ProjectTo<UserDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync();
            return View("UserData", model);
        }
    }
}
