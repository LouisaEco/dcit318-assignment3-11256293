using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    // a) Core model using record
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // b) Interface
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // c) Concrete processors
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[BankTransfer] {transaction.Category}: {transaction.Amount:C} processed via Bank Transfer.");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[MobileMoney] {transaction.Category}: {transaction.Amount:C} processed via Mobile Money.");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[CryptoWallet] {transaction.Category}: {transaction.Amount:C} processed via Crypto Wallet.");
        }
    }

    // d) Base Account
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            Balance -= transaction.Amount;
            Console.WriteLine($"[Account] -{transaction.Amount:C}. New Balance: {Balance:C}");
        }
    }

    // e) Sealed SavingsAccount
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
                return;
            }

            Balance -= transaction.Amount;
            Console.WriteLine($"[SavingsAccount] Deducted {transaction.Amount:C}. Updated Balance: {Balance:C}");
        }
    }

    // f) FinanceApp
    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            // i. SavingsAccount
            var account = new SavingsAccount("SA-1000", 1000m);
            Console.WriteLine($"Created {account.AccountNumber} with {account.Balance:C}\n");

            // ii. Sample transactions
            var t1 = new Transaction(1, DateTime.Now, 120.50m, "Groceries");
            var t2 = new Transaction(2, DateTime.Now, 250m, "Utilities");
            var t3 = new Transaction(3, DateTime.Now, 800m, "Entertainment");

            // iii. Processors mapping
            ITransactionProcessor p1 = new MobileMoneyProcessor();   // -> t1
            ITransactionProcessor p2 = new BankTransferProcessor();  // -> t2
            ITransactionProcessor p3 = new CryptoWalletProcessor();  // -> t3

            // iv. Process + apply
            p1.Process(t1); account.ApplyTransaction(t1);
            p2.Process(t2); account.ApplyTransaction(t2);
            p3.Process(t3); account.ApplyTransaction(t3);

            // v. Store
            _transactions.AddRange(new[] { t1, t2, t3 });

            Console.WriteLine("\nStored Transactions:");
            foreach (var tx in _transactions)
                Console.WriteLine($"ID {tx.Id}: {tx.Category} {tx.Amount:C} on {tx.Date:g}");
        }
    }

    class Program
    {
        static void Main()
        {
            new FinanceApp().Run();
            Console.WriteLine("\nDone.");
        }
    }
}
