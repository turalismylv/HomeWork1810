using front_to_back.DAL;
using front_to_back.ViewModels.About;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace front_to_back.Controllers
{
    public class AboutController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public AboutController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<IActionResult> Index()
        {
            var model = new AboutIndexViewModel
            {
                TeamMembers = await _appDbContext.TeamMembers.ToListAsync()
            };
            return View(model);
        }
    }
}
