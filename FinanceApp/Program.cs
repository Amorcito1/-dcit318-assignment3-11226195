using System;
using System.Collections.Generic;

// Define the Transaction record
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// Define the ITransactionProcessor interface
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// Implement the BankTransferProcessor class
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Bank Transfer: Processing a transaction of {transaction.Amount:C} for category '{transaction.Category}'.");
    }
}

// Implement the MobileMoneyProcessor class
public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Mobile Money: Processing a transaction of {transaction.Amount:C} for category '{transaction.Category}'.");
    }
}

// Implement the CryptoWalletProcessor class
public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Crypto Wallet: Processing a transaction of {transaction.Amount:C} for category '{transaction.Category}'.");
    }
}

// Define the base Account class
public class Account
{
    public string AccountNumber { get; private set; }
    protected decimal Balance { get; set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
    }
}

// Define the sealed SavingsAccount class
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds for transaction.");
        }
        else
        {
            base.ApplyTransaction(transaction);
            Console.WriteLine($"Transaction applied. New balance: {Balance:C}");
        }
    }
}

// Define the FinanceApp class
public class FinanceApp
{
    private List<Transaction> _transactions = new List<Transaction>();

    public void Run()
    {
        // Instantiate a SavingsAccount
        SavingsAccount savingsAccount = new SavingsAccount("SA123456", 1000m);

        // Create three Transaction records
        Transaction transaction1 = new Transaction(1, DateTime.Now, 150.75m, "Groceries");
        Transaction transaction2 = new Transaction(2, DateTime.Now, 200.00m, "Utilities");
        Transaction transaction3 = new Transaction(3, DateTime.Now, 300.00m, "Entertainment");

        // Process each transaction using the appropriate processor
        ITransactionProcessor mobileMoneyProcessor = new MobileMoneyProcessor();
        mobileMoneyProcessor.Process(transaction1);
        savingsAccount.ApplyTransaction(transaction1);
        _transactions.Add(transaction1);

        ITransactionProcessor bankTransferProcessor = new BankTransferProcessor();
        bankTransferProcessor.Process(transaction2);
        savingsAccount.ApplyTransaction(transaction2);
        _transactions.Add(transaction2);

        ITransactionProcessor cryptoWalletProcessor = new CryptoWalletProcessor();
        cryptoWalletProcessor.Process(transaction3);
        savingsAccount.ApplyTransaction(transaction3);
        _transactions.Add(transaction3);
    }
}

// Main application
class Program
{
    static void Main(string[] args)
    {
        FinanceApp financeApp = new FinanceApp();
        financeApp.Run();
    }
}