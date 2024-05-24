using System;
using System.Collections.Generic;

namespace pictoflow_Backend.Models
{
    public enum WatermarkStyle
    {
        SinMarca,
        InferiorDerecha,
        Ampliada
    }

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
        public WatermarkStyle WatermarkStyle { get; set; } // Enum para estilo de marca de agua
        public List<Photo> Photos { get; set; }
        public decimal IndividualPrice { get; set; } // Precio individual
        public decimal TotalPrice { get; set; } // Precio total
    }
}