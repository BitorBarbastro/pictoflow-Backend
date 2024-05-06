// Photo.cs
using System.Collections.Generic;

namespace pictoflow_Backend.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public decimal Price { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int GalleryId { get; set; }
        public Gallery Gallery { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
