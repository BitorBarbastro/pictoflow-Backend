using System;

namespace pictoflow_Backend.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int PhotoId { get; set; }
        public Photo Photo { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
