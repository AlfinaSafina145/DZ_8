using System;

namespace BankApp
{
    public enum AccountType
    {
        Current,
        Savings
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
        public BankAccount()
        {
            account_number = GenerateAccountNumber().ToString();
        }
        public BankAccount(decimal initialBalance)
        {
            account_number = GenerateAccountNumber().ToString();
            Balance = initialBalance;
        }
        public BankAccount(AccountType accType)
        {
            account_number = GenerateAccountNumber().ToString();
            Type = accType;
        }
        public BankAccount(decimal initialBalance, AccountType accType)
        {
            account_number = GenerateAccountNumber().ToString();
            Balance = initialBalance;
            Type = accType;
        }
        public string AccountNumber
        {
            get { return account_number; }
        }

        public decimal Balance
        {
            get { return balance; }
            private set
            {
                if (value >= 0)
                    balance = value;
                else
                    Console.WriteLine("Ошибка: баланс не может быть отрицательным!");
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
    }

    class Program
    {
        static void Main(string[] args)
        {
            BankAccount acc1 = new BankAccount();
            acc1.PrintInfo();

            BankAccount acc2 = new BankAccount(1000m);
            acc2.PrintInfo();

            BankAccount acc3 = new BankAccount(AccountType.Savings);
            acc3.PrintInfo();

            BankAccount acc4 = new BankAccount(5000m, AccountType.Current);
            acc4.PrintInfo();
        }
    }
}