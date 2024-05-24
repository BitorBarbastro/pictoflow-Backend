using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pictoflow_Backend.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace pictoflow_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GalleryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GalleryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("addImages/{galleryId}")]
        [Authorize]
        public async Task<IActionResult> AddImages(int galleryId, [FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("No files uploaded.");
            }

            var gallery = await _context.Galleries.FindAsync(galleryId);
            if (gallery == null)
            {
                return NotFound("Gallery not found.");
            }

            var photographerId = gallery.PhotographerId; // Asumiendo que Gallery tiene una propiedad PhotographerId

            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", $"photographer_{photographerId}", $"gallery_{galleryId}");

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var highResFilePath = Path.Combine(uploadsFolderPath, "highres", file.FileName);
                    var watermarkedFilePath = Path.Combine(uploadsFolderPath, "watermarked", file.FileName);

                    // Crear directorios si no existen
                    Directory.CreateDirectory(Path.GetDirectoryName(highResFilePath));
                    Directory.CreateDirectory(Path.GetDirectoryName(watermarkedFilePath));

                    // Guardar la imagen en alta resolución
                    using (var stream = new FileStream(highResFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Aquí puedes agregar lógica para aplicar la marca de agua y guardar la imagen con marca de agua
                    // Por ahora, simplemente copiaremos la imagen original a la ruta de la imagen con marca de agua
                    System.IO.File.Copy(highResFilePath, watermarkedFilePath, true);

                    var photo = new Photo
                    {
                        GalleryId = galleryId,
                        Title = Path.GetFileNameWithoutExtension(file.FileName), // Usar el nombre del archivo como título
                        HighResImagePath = highResFilePath,
                        WatermarkedImagePath = watermarkedFilePath,
                        UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) // Asumiendo que el UserId está en el token
                    };
                    _context.Photos.Add(photo);
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Images uploaded successfully.");
        }

        [HttpGet("{galleryId}/watermarkedPhotos")]
        public async Task<IActionResult> GetWatermarkedPhotos(int galleryId)
        {
            try
            {
                var photos = await _context.Photos
                    .Where(p => p.GalleryId == galleryId)
                    .Select(p => new
                    {
                        p.Id,
                        p.Title,
                        WatermarkedImageUrl = Url.Content($"~/uploads/photographer_{p.UserId}/gallery_{galleryId}/watermarked/{Path.GetFileName(p.WatermarkedImagePath)}")
                    })
                    .ToListAsync();

                if (photos == null || !photos.Any())
                {
                    return NotFound("No watermarked photos found for the given gallery.");
                }

                return Ok(photos);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here for debugging
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }

}
