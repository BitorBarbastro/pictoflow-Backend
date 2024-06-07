using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pictoflow_Backend.Services;
using System;
using System.Security.Claims;
using pictoflow_Backend.DTOs;
using Microsoft.EntityFrameworkCore;
using pictoflow_Backend.Models;
namespace pictoflow_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly UploadsManager _uploadsManager;
        private readonly ApplicationDbContext _context;

        public UploadController(UploadsManager uploadsManager, AuthenticationService authService, ApplicationDbContext context)
        {
            _uploadsManager = uploadsManager;
            _context = context;
        }
        [HttpPost("uploadWatermark")]
        [Authorize]
        public async Task<IActionResult> UploadWatermark([FromForm] IFormFile file, [FromForm] int photographerId, [FromForm] string name)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", $"photographer_{photographerId}", "watermarks");

            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var filePath = Path.Combine(uploadsFolderPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var watermark = await _uploadsManager.SaveWatermarkAsync(photographerId, filePath, name);
            return Ok(new { watermark.Id, Message = "Marca de agua subida correctamente" });
        }



        [HttpPut("upload/{photographerId}/watermarks/{fileName}")]
        public async Task<IActionResult> RenameWatermark(int photographerId, string fileName, [FromBody] WatermarkUpdateDto updateDto)
        {
            // Buscar la marca de agua en la base de datos
            var watermark = await _context.Watermarks.FirstOrDefaultAsync(w => w.PhotographerId == photographerId && w.Name == fileName);
            if (watermark == null)
            {
                return NotFound("Watermark not found.");
            }

            // Actualizar el nombre de la marca de agua
            watermark.Name = updateDto.NewName;
            _context.Watermarks.Update(watermark);
            await _context.SaveChangesAsync();

            return Ok("Watermark name updated successfully.");
        }

        [HttpGet("upload/{photographerId}/watermarks")]
        public async Task<IActionResult> GetWatermarks(int photographerId)
        {
            var watermarks = await _context.Watermarks
                .Where(w => w.PhotographerId == photographerId)
                .Select(w => new
                {
                    FileName = Path.GetFileName(w.ImagePath), // Extraer el nombre del archivo desde la ruta
                    w.Name
                })
                .ToListAsync();

            if (watermarks == null || !watermarks.Any())
            {
                return NotFound("No watermarks found for the given photographer.");
            }

            return Ok(watermarks);
        }


        [HttpGet("watermarks/{photographerId}")]
        public async Task<IActionResult> GetWatermarkList(int photographerId)
        {
            try
            {
                if (_context == null)
                {
                    return StatusCode(500, "Database context is not initialized.");
                }

                var watermarks = await _context.Watermarks
                    .Where(w => w.PhotographerId == photographerId)
                    .Select(w => new
                    {
                        w.Id,
                        w.Name,
                        ImageUrl = Path.Combine("uploads", $"photographer_{photographerId}", "watermarks", w.ImagePath)
                    })
                    .ToListAsync();

                if (watermarks == null || !watermarks.Any())
                {
                    return NotFound("No watermarks found for the given photographer.");
                }

                return Ok(watermarks);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here for debugging
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpPost("createGallery")]
        [Authorize]
        public async Task<IActionResult> CreateGallery([FromBody] GalleryDto galleryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Obtener el ID del usuario autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            // Verificar si el usuario autenticado es el fotógrafo que está creando la galería
            if (galleryDto.PhotographerId.ToString() != userId)
            {
                return Forbid("You are not authorized to create a gallery for this photographer.");
            }

            // Verificar si el WatermarkId proporcionado existe en la base de datos
            if (galleryDto.WatermarkId.HasValue && galleryDto.WatermarkId != 0)
            {
                var watermark = await _context.Watermarks.FindAsync(galleryDto.WatermarkId.Value);
                if (watermark == null)
                {
                    return BadRequest("Invalid WatermarkId.");
                }
            }
            else
            {
                // Asignar la marca de agua predeterminada si no se proporciona un WatermarkId
                galleryDto.WatermarkId = 0; // ID de la marca de agua predeterminada
            }

            var gallery = new Gallery
            {
                PhotographerId = galleryDto.PhotographerId,
                Name = galleryDto.Name,
                Description = galleryDto.Description,
                CreationDate = DateTime.UtcNow,
                ExpirationDate = galleryDto.ExpirationDate,
                WatermarkStyle = galleryDto.WatermarkStyle,
                WatermarkId = galleryDto.WatermarkId,
                IndividualPrice = (decimal)galleryDto.IndividualPrice, // Conversión explícita a decimal
                TotalPrice = (decimal)galleryDto.TotalPrice // Conversión explícita a decimal
            };

            try
            {
                _context.Galleries.Add(gallery);
                await _context.SaveChangesAsync();
                return Ok(new { gallery.Id, Message = "Gallery created successfully" });
            }
            catch (Exception ex)
            {
                // Registra el error completo
                return StatusCode(500, "Ocurrió un error al crear la galería.");
            }
        }

    }
}
