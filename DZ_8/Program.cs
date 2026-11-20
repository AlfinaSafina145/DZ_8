using System;
using System.Collections.Generic;
using System.IO;

namespace BankApp
{
    public enum AccountType
    {
        Current,
        Savings
    }
    public class BankTransaction
    {
        private readonly DateTime transactionDate;
        private readonly decimal amount;
        public BankTransaction(decimal amount)
        {
            this.transactionDate = DateTime.Now;
            this.amount = amount;
        }
        public DateTime TransactionDate
        {
            get { return transactionDate; }
        }

        public decimal Amount
        {
            get { return amount; }
        }

        public void PrintTransaction()
        {
            string operation;
            if (amount > 0)
            {
                operation = "Пополнение";
            }
            else
            {
                operation = "Снятие";
            }
            decimal absoluteAmount = Math.Abs(amount);
            Console.WriteLine($"[{transactionDate}] {operation} на сумму: {absoluteAmount} руб.");
        }
    }
    public class BankAccount
    {
        private static int lastAccountNumber = 0;

        private static int GenerateAccountNumber()
        {
            lastAccountNumber++;
            return lastAccountNumber;
        }

        private string account_number;
        private decimal balance;
        private AccountType type;
        private Queue<BankTransaction> transactionHistory;

        public BankAccount()
        {
            account_number = GenerateAccountNumber().ToString();
            transactionHistory = new Queue<BankTransaction>();
        }
        public BankAccount(decimal initialBalance)
        {
            account_number = GenerateAccountNumber().ToString();
            transactionHistory = new Queue<BankTransaction>();
            Balance = initialBalance;
        }
        public BankAccount(AccountType accType)
        {
            account_number = GenerateAccountNumber().ToString();
            transactionHistory = new Queue<BankTransaction>();
            Type = accType;
        }
        public BankAccount(decimal initialBalance, AccountType accType)
        {
            account_number = GenerateAccountNumber().ToString();
            transactionHistory = new Queue<BankTransaction>();
            Balance = initialBalance;
            Type = accType;
        }

        public string AccountNumber { get { return account_number; } }

        public decimal Balance
        {
            get { return balance; }
            private set
            {
                if (value >= 0) balance = value;
                else Console.WriteLine("Ошибка: баланс не может быть отрицательным!");
            }
        }
        public AccountType Type
        {
            get { return type; }
            private set { type = value; }
        }
        public void Deposit(decimal amount)
        {
            if (amount > 0)
            {
                Balance += amount;
                transactionHistory.Enqueue(new BankTransaction(amount));
                Console.WriteLine($"Счет {AccountNumber} пополнен на {amount} руб.");
            }
            else
            {
                Console.WriteLine("Ошибка: сумма пополнения должна быть больше нуля.");
            }
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("Ошибка: сумма снятия должна быть больше нуля.");
            }
            else if (amount > Balance)
            {
                Console.WriteLine("Ошибка: недостаточно средств на счете.");
            }
            else
            {
                Balance -= amount;
                transactionHistory.Enqueue(new BankTransaction(-amount));
                Console.WriteLine($"Со счета {AccountNumber} успешно снято {amount} руб.");
            }
        }

        public void Transfer(BankAccount targetAccount, decimal amount)
        {
            Console.WriteLine($"\n--- Попытка перевода {amount} руб. со счета {this.AccountNumber} на счет {targetAccount.AccountNumber} ---");

            if (amount <= 0)
            {
                Console.WriteLine("Ошибка: сумма перевода должна быть больше нуля.");
                return;
            }

            if (amount > Balance)
            {
                Console.WriteLine("Ошибка: недостаточно средств для перевода.");
                return;
            }

            this.Withdraw(amount);
            targetAccount.Deposit(amount);

            Console.WriteLine($"Перевод {amount} руб. выполнен успешно.");
        }

        public void PrintInfo()
        {
            Console.WriteLine($"[Счет {AccountNumber}] Баланс: {Balance} руб. | Тип: {Type}");
        }
        public void PrintHistory()
        {
            Console.WriteLine($"\n--- История транзакций по счету {AccountNumber} ---");
            if (transactionHistory.Count == 0)
            {
                Console.WriteLine("История пуста.");
            }
            foreach (var trans in transactionHistory)
            {
                trans.PrintTransaction();
            }
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);

            string fileName = $"history_{account_number}.txt";

            List<string> lines = new List<string>();
            lines.Add($"История операций по счету: {account_number}");

            foreach (var trans in transactionHistory)
            {
                string operation;
                if (trans.Amount > 0)
                {
                    operation = "Пополнение";
                }
                else
                {
                    operation = "Снятие";
                }
                lines.Add($"{trans.TransactionDate} | {operation} | {Math.Abs(trans.Amount)} руб.");
            }
            File.WriteAllLines(fileName, lines);
            Console.WriteLine($"\n[Dispose] История по счету {account_number} сохранена в файл {fileName}");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            BankAccount acc1 = new BankAccount(1000m, AccountType.Savings);
            BankAccount acc2 = new BankAccount(500m);

            acc1.PrintInfo();

            Console.WriteLine("\n--- Совершаем операции ---");
            acc1.Deposit(500m);
            acc1.Withdraw(200m);
            acc1.Transfer(acc2, 300m);

            acc1.PrintInfo();
            acc2.PrintInfo();

            acc1.PrintHistory();

            Console.WriteLine("\n--- Завершаем работу со счетом acc1 ---");
            acc1.Dispose();
        }
    }
}