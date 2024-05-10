using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pictoflow_Backend.Models;

namespace pictoflow_Backend.Services
{
    public class UploadsManager
    {
        private readonly ApplicationDbContext _context;

        public UploadsManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Watermark> SaveWatermarkAsync(int photographerId, string imagePath)
        {
            var watermark = new Watermark
            {
                PhotographerId = photographerId,
                ImagePath = imagePath
            };

            _context.Watermarks.Add(watermark);
            await _context.SaveChangesAsync();
            return watermark;
        }
    }
}
