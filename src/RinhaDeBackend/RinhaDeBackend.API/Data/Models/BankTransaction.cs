namespace RinhaDeBackend.API.Data.Models
{
    public class BankTransaction
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int Amount { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public BankTransaction() { }

        public BankTransaction(int customerId, int amount, string type, string description, DateTime createdAt)
        {
            CustomerId = customerId;
            Amount = amount;
            Type = type;
            Description = description;
            CreatedAt = createdAt;
        }
    }
}
