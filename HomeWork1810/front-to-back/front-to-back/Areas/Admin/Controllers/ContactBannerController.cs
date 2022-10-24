using front_to_back.Areas.Admin.ViewModels;
using front_to_back.DAL;
using front_to_back.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace front_to_back.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactBannerController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ContactBannerController(AppDbContext appDbContext,IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var model = new ContactBannerIndexViewModel
            {
                ContactBanner = await _appDbContext.ContactBanners.ToListAsync()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ContactBanner contactBanner)
        {
            if (!ModelState.IsValid) return View(contactBanner);
            if (!contactBanner.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "Zehmet olmasa image formatinda secin");
                return View(contactBanner);
            }
            if (contactBanner.Photo.Length/1024>60)
            {
                ModelState.AddModelError("Photo", "Sekilin olcusu 60 kb dan coxdur");
                return View(contactBanner);
            }

            var fileName = $"{Guid.NewGuid()}_{contactBanner.Photo.FileName}";

            var path = Path.Combine(_webHostEnvironment.WebRootPath, "assets/img", fileName);

            using (FileStream fileStream=new FileStream(path,FileMode.Create,FileAccess.ReadWrite))
            {
                await contactBanner.Photo.CopyToAsync(fileStream);
            }
            contactBanner.PhotoPath = fileName;

            await _appDbContext.ContactBanners.AddAsync(contactBanner);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var contactBanner = await _appDbContext.ContactBanners.FindAsync(id);
            if (contactBanner == null) return NotFound();

            return View(contactBanner);

        }
    }
}
