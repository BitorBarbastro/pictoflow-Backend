// Gallery.cs
using System;
using System.Collections.Generic;

namespace pictoflow_Backend.Models
{
    public class Gallery
    {
        public int Id { get; set; }
        public int PhotographerId { get; set; }
        public User Photographer { get; set; }
        public int? WatermarkId { get; set; }
        public Watermark Watermark { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public List<Photo> Photos { get; set; }
    }
}

