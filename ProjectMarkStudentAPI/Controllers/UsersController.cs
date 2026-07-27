using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using ProjectMarkStudentAPI.Constants;
using ProjectMarkStudentAPI.DTOs;
using ProjectMarkStudentAPI.Models;
using System.Security.Claims;

namespace ProjectMarkStudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ProjectStudentMarkContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UsersController(ProjectStudentMarkContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet]
        [EnableQuery]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _context.Users.Include(u => u.Role)
                                            .Where(u => u.Role.RoleName != RoleNames.Admin)
                                            .ToListAsync();
            return Ok(_mapper.Map<IEnumerable<UserDTO>>(users));
        }

        [HttpGet("{id}")]
        [EnableQuery]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<UserDTO>(user));
        }

        [HttpGet("me")]
        [EnableQuery]
        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Manager + "," + RoleNames.Teacher)]
        public async Task<ActionResult<UserDTO>> GetMe()
        {
            var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return NotFound();

            return Ok(_mapper.Map<UserDTO>(user));
        }

        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<ActionResult<UserDTO>> PostUser(CreateUserDTO userDto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == userDto.Username))
            {
                return BadRequest("Username already exists.");
            }

            var user = _mapper.Map<User>(userDto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            user.Status = true; // Tài khoản mới được kích hoạt mặc định
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var createdUser = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == user.UserId);
            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, _mapper.Map<UserDTO>(createdUser));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Manager + "," + RoleNames.Teacher)]
        public async Task<IActionResult> PutUser(int id, UpdateUserDTO userDto)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.Role?.RoleName == RoleNames.Admin)
            {
                var currentUsername = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (user.Username != currentUsername)
                {
                    return Forbid();
                }
            }

            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.PhoneNumber = userDto.PhoneNumber;
            // Nếu AvatarUrl là đường dẫn file local thì copy vào assets và đổi thành URL chuẩn
            user.AvatarUrl = ResolveAvatarUrl(userDto.AvatarUrl, user.AvatarUrl);
            user.Address = userDto.Address;
            user.Gender = userDto.Gender;
            user.RoleId = userDto.RoleId;
            // Status KHÔNG được thay đổi qua PUT — dùng PATCH /{id}/status
            user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Kiểm tra nếu <paramref name="newUrl"/> là đường dẫn file local tồn tại trên disk
        /// thì copy vào thư mục assets của WebClient và trả về URL chuẩn.
        /// Nếu đã là URL web (bắt đầu bằng / hoặc http) thì giữ nguyên.
        /// </summary>
        private string? ResolveAvatarUrl(string? newUrl, string? oldUrl)
        {
            if (string.IsNullOrWhiteSpace(newUrl))
                return newUrl;

            // Đã là URL web — không cần xử lý thêm
            if (newUrl.StartsWith("/") || newUrl.StartsWith("http://") || newUrl.StartsWith("https://"))
                return newUrl;

            // Không phải đường dẫn local hợp lệ — giữ nguyên
            if (!Path.IsPathRooted(newUrl) || !System.IO.File.Exists(newUrl))
                return newUrl;

            // --- Đây là đường dẫn file local: copy vào assets ---
            var configuredPath = _configuration["AvatarStorage:PhysicalPath"];
            var folderPath = Path.IsPathRooted(configuredPath!)
                ? configuredPath!
                : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, configuredPath!));

            Directory.CreateDirectory(folderPath);

            // Xóa avatar cũ (nếu cùng thư mục quản lý)
            if (!string.IsNullOrEmpty(oldUrl))
            {
                var urlBase = _configuration["AvatarStorage:UrlBase"] ?? "/assets/pictures/profile";
                if (oldUrl.StartsWith(urlBase))
                {
                    var oldFileName = Path.GetFileName(oldUrl);
                    var oldFilePath = Path.Combine(folderPath, oldFileName);
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }
            }

            var ext = Path.GetExtension(newUrl).ToLowerInvariant();
            var newFileName = $"{Guid.NewGuid()}{ext}";
            var destPath = Path.Combine(folderPath, newFileName);

            System.IO.File.Copy(newUrl, destPath, overwrite: true);

            var urlBaseConfig = _configuration["AvatarStorage:UrlBase"] ?? "/assets/pictures/profile";
            return $"{urlBaseConfig}/{newFileName}";
        }


        /// <summary>
        /// Upload ảnh đại diện cho user, lưu vào wwwroot/assets/pictures/profile của WebClient.
        /// Xóa avatar cũ (nếu có) để tránh rác file.
        /// Trả về: { "avatarUrl": "/assets/pictures/profile/filename.ext" }
        /// </summary>
        [HttpPost("{id}/avatar")]
        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Manager + "," + RoleNames.Teacher)]
        public async Task<IActionResult> UploadAvatar(int id, IFormFile file)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
                return NotFound();

            // Chỉ Admin của chính mình mới được sửa
            if (user.Role?.RoleName == RoleNames.Admin)
            {
                var currentUsername = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (user.Username != currentUsername)
                    return Forbid();
            }

            // --- Validate file ---
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "Không có file được gửi lên." });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
                return BadRequest(new { error = "Định dạng ảnh không hợp lệ. Chỉ chấp nhận JPG, PNG, GIF, WEBP." });

            var maxSize = _configuration.GetValue<long>("AvatarStorage:MaxFileSizeBytes", 5 * 1024 * 1024);
            if (file.Length > maxSize)
                return BadRequest(new { error = $"Ảnh quá lớn. Tối đa {maxSize / 1024 / 1024}MB." });

            // --- Xác định thư mục lưu file ---
            var configuredPath = _configuration["AvatarStorage:PhysicalPath"];
            var folderPath = Path.IsPathRooted(configuredPath!)
                ? configuredPath!
                : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, configuredPath!));

            Directory.CreateDirectory(folderPath);

            // --- Xóa avatar cũ (nếu có và nằm trong cùng thư mục) ---
            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                var urlBase = _configuration["AvatarStorage:UrlBase"] ?? "/assets/pictures/profile";
                if (user.AvatarUrl.StartsWith(urlBase))
                {
                    var oldFileName = Path.GetFileName(user.AvatarUrl);
                    var oldFilePath = Path.Combine(folderPath, oldFileName);
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }
            }

            // --- Lưu file mới ---
            var newFileName = $"{Guid.NewGuid()}{ext}";
            var newFilePath = Path.Combine(folderPath, newFileName);

            using (var stream = new FileStream(newFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // --- Cập nhật DB ---
            var urlBaseConfig = _configuration["AvatarStorage:UrlBase"] ?? "/assets/pictures/profile";
            user.AvatarUrl = $"{urlBaseConfig}/{newFileName}";
            user.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(new { avatarUrl = user.AvatarUrl });
        }

        [HttpPut("{id}/password")]
        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Manager + "," + RoleNames.Teacher)]
        public async Task<IActionResult> ChangePassword(int id, ChangePasswordDTO dto)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.Role?.RoleName == RoleNames.Admin)
            {
                var currentUsername = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (user.Username != currentUsername)
                {
                    return Forbid();
                }
            }

            // Issue #1: Only verify via BCrypt — never compare plain-text passwords
            bool isOldPasswordValid = BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash);

            if (!isOldPasswordValid)
            {
                return BadRequest("Mật khẩu cũ không chính xác.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.Role?.RoleName == RoleNames.Admin)
            {
                return BadRequest("Không thể vô hiệu hóa tài khoản Admin.");
            }

            // Thay vì xóa, chỉ vô hiệu hóa tài khoản
            user.Status = false;
            user.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.Role?.RoleName == RoleNames.Admin)
            {
                return BadRequest("Không thể thay đổi trạng thái tài khoản Admin.");
            }

            user.Status = !user.Status;
            user.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(new { userId = user.UserId, status = user.Status });
        }

        [HttpGet("roles")]
        [EnableQuery]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            return await _context.Roles.ToListAsync();
        }
    }
}
