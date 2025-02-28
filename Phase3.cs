using System;
using System.Collections.Generic;
using System.Threading;

namespace ProjectA_Phase3
{
    class Program
    {
        // Two separate accounts with their own locks
        private static int account1Balance = 1000;
        private static int account2Balance = 1000;

        private static readonly object lockAccount1 = new object();
        private static readonly object lockAccount2 = new object();

        // Simulates transferring money from Account 1 to Account 2
        static void TransferFromAccount1ToAccount2(object threadId)
        {
            Console.WriteLine($"[Thread {threadId}] Attempting to lock Account 1...");
            lock (lockAccount1)
            {
                Console.WriteLine($"[Thread {threadId}] Locked Account 1. " +
                                  $"(Balance1 = {account1Balance}, Balance2 = {account2Balance})");
                
                // Simulate some work before locking Account 2
                Thread.Sleep(100);

                Console.WriteLine($"[Thread {threadId}] Attempting to lock Account 2...");
                lock (lockAccount2)
                {
                    Console.WriteLine($"[Thread {threadId}] Locked Account 2.");
                    
                    // Transfer a random amount
                    int amount = new Random().Next(1, 100);
                    if (account1Balance >= amount)
                    {
                        account1Balance -= amount;
                        account2Balance += amount;
                        Console.WriteLine($"[Thread {threadId}] Transferred {amount} from Account 1 to Account 2. " +
                                          $"(Balance1 = {account1Balance}, Balance2 = {account2Balance})");
                    }
                    else
                    {
                        Console.WriteLine($"[Thread {threadId}] Insufficient funds to transfer {amount} " +
                                          $"(Balance1 = {account1Balance}).");
                    }
                }
            }
        }

        // Simulates transferring money from Account 2 to Account 1
        static void TransferFromAccount2ToAccount1(object threadId)
        {
            Console.WriteLine($"[Thread {threadId}] Attempting to lock Account 2...");
            lock (lockAccount2)
            {
                Console.WriteLine($"[Thread {threadId}] Locked Account 2. " +
                                  $"(Balance1 = {account1Balance}, Balance2 = {account2Balance})");

                // Simulate some work before locking Account 1
                Thread.Sleep(100);

                Console.WriteLine($"[Thread {threadId}] Attempting to lock Account 1...");
                lock (lockAccount1)
                {
                    Console.WriteLine($"[Thread {threadId}] Locked Account 1.");

                    // Transfer a random amount
                    int amount = new Random().Next(1, 100);
                    if (account2Balance >= amount)
                    {
                        account2Balance -= amount;
                        account1Balance += amount;
                        Console.WriteLine($"[Thread {threadId}] Transferred {amount} from Account 2 to Account 1. " +
                                          $"(Balance1 = {account1Balance}, Balance2 = {account2Balance})");
                    }
                    else
                    {
                        Console.WriteLine($"[Thread {threadId}] Insufficient funds to transfer {amount} " +
                                          $"(Balance2 = {account2Balance}).");
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            // Create multiple threads: half transfer one way, half transfer the other.
            // This often leads to deadlock if timing aligns.
            int numThreads = 4;
            List<Thread> threads = new List<Thread>();

            // First half: Transfer from Account 1 to Account 2
            for (int i = 1; i <= numThreads / 2; i++)
            {
                Thread t = new Thread(TransferFromAccount1ToAccount2);
                threads.Add(t);
                t.Start($"A{i}");  // Label threads A1, A2, etc.
            }

            // Second half: Transfer from Account 2 to Account 1
            for (int i = 1; i <= numThreads / 2; i++)
            {
                Thread t = new Thread(TransferFromAccount2ToAccount1);
                threads.Add(t);
                t.Start($"B{i}");  // Label threads B1, B2, etc.
            }

            // Wait for all threads to complete (if they ever do!)
            foreach (Thread t in threads)
            {
                t.Join();
            }

            // If a deadlock occurs, the program never reaches this point.
            Console.WriteLine("All transfers completed successfully (no deadlock occurred this run).");
            Console.WriteLine($"Final Balances => Account1: {account1Balance}, Account2: {account2Balance}");
        }
    }
}

