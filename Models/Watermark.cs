namespace pictoflow_Backend.Models
{
    public class Watermark
    {
        public int Id { get; set; }
        public int PhotographerId { get; set; }
        public User Photographer { get; set; }
        public string ImagePath { get; set; }
    }
}
