using Microsoft.AspNetCore.Mvc;
using Portfolio;
using Portfolio.Models;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;




namespace Portfolio.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AdminController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public async Task <IActionResult> AdminIndex()
        {
            return View();
        }

        [HttpGet]
        public async Task <IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project nProject, List<IFormFile> projectMedia)
        {
            ModelState.Remove("ImageUrl");
            ModelState.Remove("VideoUrl");

            if (projectMedia.Count == 0 || projectMedia == null)
                ModelState.AddModelError("projectMedia", "Please upload at least one image or video");

            if (ModelState.IsValid == false)
                ModelState.AddModelError(string.Empty, "Invalid data.");

            foreach (var file in projectMedia)
            {
                if (file.Length > 0)
                {            
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))                    
                       await file.CopyToAsync(fileStream);
                    string dbPath = "/uploads/" + uniqueFileName;

                    if (file.ContentType.Contains("image"))
                        nProject.ImageUrl = dbPath;
                    else if (file.ContentType.Contains("video"))
                        nProject.VideoUrl = dbPath;
                }
            }

            _db.Projects.Add(nProject);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Project created successfully!";
            return RedirectToAction(nameof(AdminIndex));
        }

        public IActionResult Edit()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Delete()
        {
            var projectList = _db.Projects.ToList();
            return View(projectList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id)
        {
            var project = await _db.Projects.FindAsync(Id);
            if (project == null)
                return NotFound();

            if (!string.IsNullOrEmpty(project.ImageUrl))
            {
                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, project.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }
            if (!string.IsNullOrEmpty(project.VideoUrl))
            {
                var videoPath = Path.Combine(_hostEnvironment.WebRootPath, project.VideoUrl.TrimStart('/'));
                if (System.IO.File.Exists(videoPath))
                    System.IO.File.Delete(videoPath);
            }

            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Project deleted successfully!";
            return RedirectToAction(nameof(AdminIndex));
        }
    }
}
