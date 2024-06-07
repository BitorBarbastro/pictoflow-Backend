using pictoflow_Backend.Models;
using System;
using System.ComponentModel.DataAnnotations;

public class GalleryDto
{
    [Required]
    public int PhotographerId { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The name must be less than 100 characters.")]
    public string Name { get; set; }

    [StringLength(500, ErrorMessage = "The description must be less than 500 characters.")]
    public string Description { get; set; }

    [Required]
    public DateTime ExpirationDate { get; set; }

    [Required]
    public WatermarkStyle WatermarkStyle { get; set; }

    public int? WatermarkId { get; set; }

    // Añadir los campos que faltan
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "The individual price must be a positive number.")]
    public double IndividualPrice { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "The total price must be a positive number.")]
    public double TotalPrice { get; set; }
}