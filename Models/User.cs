using System.Collections.Generic;

namespace pictoflow_Backend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string? BillingAddress { get; set; }
        public string? Description { get; set; }
        public string? PhoneNumber { get; set; }
        public string? TaxId { get; set; }

        public List<Photo>? Photos { get; set; }

        public List<Gallery>? Galleries { get; set; }

        public List<Watermark>? Watermarks { get; set; }

        public List<Feedback>? Feedbacks { get; set; }

        public List<Transaction>? Transactions { get; set; }
    }
}
