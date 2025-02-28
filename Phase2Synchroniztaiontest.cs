using System;
using System.Collections.Generic;
using System.Threading;

namespace ProjectA_Phase2_Tests
{
    class Program
    {
        // Shared resource
        private static int accountBalance = 1000;
        private static readonly object lockObject = new object();

        static void Main(string[] args)
        {
            int numThreads = 50; // Increase for stress testing
            var threads = new List<Thread>();
            Random rand = new Random();

            // Start threads
            for (int i = 0; i < numThreads; i++)
            {
                Thread t = new Thread(() =>
                {
                    // Random deposit or withdrawal
                    bool isDeposit = rand.Next(2) == 0;
                    int amount = rand.Next(1, 100);

                    lock (lockObject)
                    {
                        if (isDeposit)
                        {
                            accountBalance += amount;
                            Console.WriteLine($"Deposited {amount}, new balance: {accountBalance}");
                        }
                        else
                        {
                            if (accountBalance >= amount)
                            {
                                accountBalance -= amount;
                                Console.WriteLine($"Withdrew {amount}, new balance: {accountBalance}");
                            }
                            else
                            {
                                Console.WriteLine($"Insufficient funds to withdraw {amount}, balance: {accountBalance}");
                            }
                        }
                    }

                    // Simulate some processing
                    Thread.Sleep(rand.Next(10, 50));
                });
                threads.Add(t);
                t.Start();
            }

            // Wait for all threads
            foreach (var t in threads)
                t.Join();

            Console.WriteLine($"All threads completed. Final balance: {accountBalance}");
        }
    }
}

