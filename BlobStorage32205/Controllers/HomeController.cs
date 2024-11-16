using BlobStorage32205.Models;
using BlobStorage32205.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlobStorage32205.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlobStorageService _blobStorageService;

        public HomeController(ILogger<HomeController> logger, IBlobStorageService blobStorageService)
        {
            _logger = logger;
            _blobStorageService = blobStorageService;
        }

        public async Task<IActionResult> Index()
        {
            var imageUrls = await _blobStorageService.ListFilesAsync();
            return View(imageUrls);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty.");
            }

            using var stream = file.OpenReadStream();
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var imageUrl = await _blobStorageService.UploadAsync(stream, fileName);
            return RedirectToAction("Index");
        }

        [HttpGet("downlod/{fileName}")]
        public async Task<IActionResult> DownloadImage(string fileName)
        {
            var stream = await _blobStorageService.DownloadAsync(fileName);
            return File(stream, "image/jpeg", fileName);
        }


        public async Task<IActionResult> DeleteImage(string fileName)
        {
            try
            {
                await _blobStorageService.DeleteAsync(fileName);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting file: {fileName}", ex);
                return BadRequest("Failed to delete image.");
            }
        }
        [HttpPost("upload-video")]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            // Faylın olub-olmamasını yoxlayaq
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Faylın tipini yoxlayaq (yalnız video)
            if (!file.ContentType.StartsWith("video/"))
            {
                return BadRequest("Only video files are allowed.");
            }

            // Fayl yükləmə əməliyyatını yerinə yetirək
            using var stream = file.OpenReadStream();
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var videoUrl = await _blobStorageService.UploadAsync(stream, fileName);

            // Videoların siyahısını yeniləyirik
            var videoUrls = await _blobStorageService.ListFilesAsync();
            return View("Index", videoUrls);
        }


        [HttpGet("download-video/{fileName}")]
        public async Task<IActionResult> DownloadVideo(string fileName)
        {
            var stream = await _blobStorageService.DownloadAsync(fileName);
            return File(stream, "video/mp4", fileName);  // video mime tipini qeyd edirik
        }


        [HttpPost("delete-video")]
        public async Task<IActionResult> DeleteVideo(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("Invalid file name.");
            }

            try
            {
                // BlobStorage xidmətində videonu silirik
                await _blobStorageService.DeleteAsync(fileName);

                // Silindikdən sonra videoların siyahısını yeniləyirik
                var videoUrls = await _blobStorageService.ListFilesAsync();
                return View("Index", videoUrls);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting file: {fileName}", ex);
                return BadRequest("Failed to delete video.");
            }
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}