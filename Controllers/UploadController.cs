using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using pictoflow_Backend.Services;
using System;
using System.Security.Claims;

namespace pictoflow_Backend.Controllers
{
    [Route("api/uploadWatermark")]
    [ApiController]
    [Authorize]
    public class UploadController : ControllerBase
    {
        private readonly UploadsManager _uploadsManager;
        private readonly AuthenticationService _authService;

        public UploadController(UploadsManager uploadsManager, AuthenticationService authService)
        {
            _uploadsManager = uploadsManager;
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadWatermark(IFormFile watermark)
        {
            if (watermark == null || watermark.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            try
            {
                var principal = _authService.ValidateToken(token);
                var userId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var userFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", userId.ToString(), "watermarks");
                if (!Directory.Exists(userFolder))
                {
                    Directory.CreateDirectory(userFolder);
                }

                var filePath = Path.Combine(userFolder, watermark.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await watermark.CopyToAsync(stream);
                }

                var savedWatermark = await _uploadsManager.SaveWatermarkAsync(userId, filePath);
                return Ok(new { path = savedWatermark.ImagePath });
            }
            catch (Exception ex)
            {
                return Unauthorized($"Invalid token: {ex.Message}");
            }
        }
    }
}
