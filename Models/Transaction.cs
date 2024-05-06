
namespace pictoflow_Backend.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public string Tier { get; set; }
        public string Frequency { get; set; }
    }
}
