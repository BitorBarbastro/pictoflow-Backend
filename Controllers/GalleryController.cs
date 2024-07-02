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

                    System.IO.File.Copy(highResFilePath, watermarkedFilePath, true);

                    var photo = new Photo
                    {
                        GalleryId = galleryId,
                        Title = Path.GetFileNameWithoutExtension(file.FileName), 
                        HighResImagePath = highResFilePath,
                        WatermarkedImagePath = watermarkedFilePath,
                        UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) 
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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("{galleryId}/highres")]
        public async Task<IActionResult> GetHighResPhotos(int galleryId)
        {
            try
            {
                var photos = await _context.Photos
                    .Where(p => p.GalleryId == galleryId)
                    .Select(p => new
                    {
                        p.Id,
                        p.Title,
                        HighresImageUrl= Url.Content($"~/uploads/photographer_{p.UserId}/gallery_{galleryId}/highres/{Path.GetFileName(p.HighResImagePath)}")
                    })
                    .ToListAsync();

                if (photos == null || !photos.Any())
                {
                    return NotFound("No photos found for the given gallery.");
                }

                return Ok(photos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // Endpoint para renombrar una fotografía
        [HttpPut("renamePhoto/{photoId}")]
            [Authorize]
            public async Task<IActionResult> RenamePhoto(int photoId, [FromBody] string newName)
            {
                var photo = await _context.Photos.FindAsync(photoId);
                if (photo == null)
                {
                    return NotFound("Photo not found.");
                }

                photo.Title = newName;
                _context.Photos.Update(photo);
                await _context.SaveChangesAsync();

                return Ok("Photo renamed successfully.");
            }

            // Endpoint para eliminar una fotografía
            [HttpDelete("deletePhoto/{photoId}")]
            [Authorize]
            public async Task<IActionResult> DeletePhoto(int photoId)
            {
                var photo = await _context.Photos.FindAsync(photoId);
                if (photo == null)
                {
                    return NotFound("Photo not found.");
                }

                // Eliminar archivos físicos
                var highResFile = new FileInfo(photo.HighResImagePath);
                if (highResFile.Exists)
                {
                    highResFile.Delete();
                }

                var watermarkedFile = new FileInfo(photo.WatermarkedImagePath);
                if (watermarkedFile.Exists)
                {
                    watermarkedFile.Delete();
                }

                _context.Photos.Remove(photo);
                await _context.SaveChangesAsync();

                return Ok("Photo deleted successfully.");

            }


        [HttpGet("photographer/{photographerId}")]
        [Authorize]
        public async Task<IActionResult> GetGalleries(int photographerId)
        {
            try
            {
                var galleries = await _context.Galleries
                    .Where(g => g.PhotographerId == photographerId)
                    .Select(g => new
                    {
                        g.Id,
                        g.Name,
                        FirstImage = _context.Photos
                            .Where(p => p.GalleryId == g.Id)
                            .Select(p => p.HighResImagePath)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                if (galleries == null || !galleries.Any())
                {
                    return NotFound("No galleries found for the given photographer.");
                }

                return Ok(galleries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
