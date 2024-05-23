using System;

namespace pictoflow_Backend.Models
{
    public class Feedback
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int PhotoId { get; set; }
        public Photo Photo { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }
        public bool AddCart { get; set; } = false;
    }
}
