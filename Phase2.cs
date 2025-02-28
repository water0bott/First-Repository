using System;
using System.Collections.Generic;
using System.Threading;

namespace ProjectA_Phase2
{
    class Program
    {
        // Shared resource: the account balance
        private static int accountBalance = 1000;

        // Lock object for synchronization
        private static readonly object lockObject = new object();

        // Each thread simulates a transaction (deposit or withdrawal).
        static void ProcessTransaction(object transactionId)
        {
            // Create a random amount and decide deposit/withdrawal
            Random random = new Random();
            int amount = random.Next(1, 100);   // random amount between 1 and 99
            bool isDeposit = random.Next(0, 2) == 0; // 50% chance to deposit or withdraw

            // Protect the shared resource with a lock
            lock (lockObject)
            {
                if (isDeposit)
                {
                    // Deposit
                    accountBalance += amount;
                    Console.WriteLine($"Transaction {transactionId} (Thread {Thread.CurrentThread.ManagedThreadId}): " +
                                      $"Deposited {amount}. New balance: {accountBalance}");
                }
                else
                {
                    // Withdrawal
                    if (accountBalance >= amount)
                    {
                        accountBalance -= amount;
                        Console.WriteLine($"Transaction {transactionId} (Thread {Thread.CurrentThread.ManagedThreadId}): " +
                                          $"Withdrew {amount}. New balance: {accountBalance}");
                    }
                    else
                    {
                        Console.WriteLine($"Transaction {transactionId} (Thread {Thread.CurrentThread.ManagedThreadId}): " +
                                          $"Insufficient funds to withdraw {amount}. Balance remains: {accountBalance}");
                    }
                }
            }

            // Simulate some processing time
            Thread.Sleep(100);
        }

        static void Main(string[] args)
        {
            int numTransactions = 10;  // Number of concurrent threads
            List<Thread> threads = new List<Thread>();

            // Create and start each transaction thread
            for (int i = 1; i <= numTransactions; i++)
            {
                Thread t = new Thread(ProcessTransaction);
                threads.Add(t);
                t.Start(i);
            }

            // Wait for all threads to finish
            foreach (Thread t in threads)
            {
                t.Join();
            }

            Console.WriteLine($"All transactions completed. Final balance: {accountBalance}");
        }
    }
}

