namespace pictoflow_Backend.Services
{
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;

    public interface IWatermarkService
    {
        Task<string> ApplyWatermarkAsync(string imagePath, string watermarkId, string style);
    }

    public class WatermarkService : IWatermarkService
    {
        private readonly Cloudinary _cloudinary;

        public WatermarkService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }
        public async Task<string> ApplyWatermarkAsync(string imagePath, string watermarkId, string style)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(imagePath),
                Transformation = new Transformation().Overlay(watermarkId).Gravity("south_east").X(10).Y(10).Opacity(50).Width(100).Crop("scale")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("Error uploading image to Cloudinary: " + uploadResult.Error.Message);
            }

            return uploadResult.SecureUrl.ToString();
        }
    }
}
