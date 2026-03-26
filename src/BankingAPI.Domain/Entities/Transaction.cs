namespace BankingAPI.Domain.Entities;

public enum TransactionStatus { Pending, Completed, Failed }

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SenderAccountId { get; set; }
    public Guid ReceiverAccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Completed;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Account? SenderAccount { get; set; }
    public Account? ReceiverAccount { get; set; }
}
