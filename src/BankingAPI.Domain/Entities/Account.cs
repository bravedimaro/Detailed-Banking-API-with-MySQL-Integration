namespace BankingAPI.Domain.Entities;

public class Account
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public ICollection<Transaction> SentTransactions { get; set; } = new List<Transaction>();
    public ICollection<Transaction> ReceivedTransactions { get; set; } = new List<Transaction>();
}
