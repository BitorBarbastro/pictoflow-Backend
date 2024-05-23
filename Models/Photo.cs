using System.Collections.Generic;

namespace pictoflow_Backend.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string HighResImagePath { get; set; } // Ruta para la imagen en alta resolución
        public string WatermarkedImagePath { get; set; } // Ruta para la imagen con marca de agua
        public int UserId { get; set; }
        public User User { get; set; }
        public int GalleryId { get; set; }
        public Gallery Gallery { get; set; }

        public List<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }
}
