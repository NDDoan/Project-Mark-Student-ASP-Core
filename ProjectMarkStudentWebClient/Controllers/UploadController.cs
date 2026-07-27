using Microsoft.AspNetCore.Mvc;

namespace ProjectMarkStudentWebClient.Controllers
{
    [Route("[controller]")]
    public class UploadController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public UploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        /// <summary>
        /// Upload ảnh đại diện, lưu vào wwwroot/assets/pictures/profile
        /// Trả về: { "url": "/assets/pictures/profile/filename.ext" }
        /// </summary>
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "Không có file được gửi lên." });

            // Kiểm tra định dạng
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
                return BadRequest(new { error = "Định dạng ảnh không hợp lệ. Chỉ chấp nhận JPG, PNG, GIF, WEBP." });

            // Kiểm tra kích thước (tối đa 5MB)
            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { error = "Ảnh quá lớn. Tối đa 5MB." });

            // Tạo tên file duy nhất
            var fileName = $"{Guid.NewGuid()}{ext}";
            var folderPath = Path.Combine(_env.WebRootPath, "assets", "pictures", "profile");

            // Đảm bảo thư mục tồn tại
            Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativeUrl = $"/assets/pictures/profile/{fileName}";
            return Ok(new { url = relativeUrl });
        }
    }
}
