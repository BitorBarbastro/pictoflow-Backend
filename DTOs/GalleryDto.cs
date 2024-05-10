using Microsoft.AspNetCore.Mvc;
using pictoflow_Backend.DTOs;

namespace pictoflow_Backend.DTOs
{
    public class GalleryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string WatermarkStyle { get; set; }
        public decimal IndividualPrice { get; set; }
        public decimal GalleryPrice { get; set; }
        public List<IFormFile> Images { get; set; }
    }
}//Añadir a la bbdd los campos necesarios para gallery
//[ApiController]
//[Route("api/[controller]")]
//public class GalleryController : ControllerBase
//{
//    [HttpPost]
//    public async Task<IActionResult> PostGallery([FromForm] GalleryDto galleryDto)
//    {
//        try
//        {
//            // Aquí puedes procesar los datos recibidos y guardarlos en la base de datos
//            // Accede a los datos de la galería a través de galleryDto

//            // Ejemplo de acceso a los datos:
//            string name = galleryDto.Name;
//            string description = galleryDto.Description;
//            string galleryId = galleryDto.GalleryId;
//            string watermarkStyle = galleryDto.WatermarkStyle;
//            decimal individualPrice = galleryDto.IndividualPrice;
//            decimal galleryPrice = galleryDto.GalleryPrice;
//            List<IFormFile> images = galleryDto.Images;

//            // Aquí puedes guardar los datos en la base de datos y realizar otras operaciones necesarias

//            return Ok("Gallery created successfully");
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, $"Error creating gallery: {ex.Message}");
//        }
//    }
//}
