namespace BankingAPI.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string name, object key)
        : base($"{name} with key '{key}' was not found.") { }
}

public class InsufficientFundsException : DomainException
{
    public InsufficientFundsException()
        : base("Insufficient funds to complete this transaction.") { }
}

public class DuplicateEmailException : DomainException
{
    public DuplicateEmailException(string email)
        : base($"A user with email '{email}' already exists.") { }
}
