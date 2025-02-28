using System;
using System.Threading;

namespace ProjectA_Phase4
{
    // Account class that includes an ID, a balance, and its own lock object.
    class Account
    {
        public int Id { get; }
        public int Balance { get; set; }
        public readonly object LockObject = new object();

        public Account(int id, int initialBalance)
        {
            Id = id;
            Balance = initialBalance;
        }
    }

    class Program
    {
        // Transfer money between two accounts using a strict lock ordering.
        static void Transfer(Account from, Account to, int amount, string threadName)
        {
            // Determine the lock order: always lock the account with the lower ID first.
            Account firstLock = from.Id < to.Id ? from : to;
            Account secondLock = from.Id < to.Id ? to : from;

            // Acquire the first lock.
            lock (firstLock.LockObject)
            {
                Console.WriteLine($"{threadName}: Locked Account {firstLock.Id}");
                // Simulate processing delay.
                Thread.Sleep(50);

                // Acquire the second lock.
                lock (secondLock.LockObject)
                {
                    Console.WriteLine($"{threadName}: Locked Account {secondLock.Id}");
                    
                    // Perform the transfer if funds are sufficient.
                    if (from.Balance >= amount)
                    {
                        from.Balance -= amount;
                        to.Balance += amount;
                        Console.WriteLine($"{threadName}: Transferred {amount} from Account {from.Id} to Account {to.Id}");
                    }
                    else
                    {
                        Console.WriteLine($"{threadName}: Insufficient funds in Account {from.Id} to transfer {amount}");
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            // Create two accounts with unique IDs.
            Account account1 = new Account(1, 1000);
            Account account2 = new Account(2, 1000);

            // Create threads that perform transfers between accounts.
            Thread thread1 = new Thread(() => Transfer(account1, account2, 200, "Thread1"));
            Thread thread2 = new Thread(() => Transfer(account2, account1, 300, "Thread2"));

            // Start the threads.
            thread1.Start();
            thread2.Start();

            // Wait for both threads to complete.
            thread1.Join();
            thread2.Join();

            // Display final account balances.
            Console.WriteLine($"Final Balances: Account 1 = {account1.Balance}, Account 2 = {account2.Balance}");
        }
    }
}

