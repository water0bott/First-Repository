using System;
using System.Collections.Generic;
using System.Threading;

namespace ProjectA_Phase1
{
    class Program
    {
        // This method simulates processing a transaction.
        // It accepts an object parameter that represents the transaction ID.
        static void ProcessTransaction(object transactionId)
        {
            Console.WriteLine($"Starting transaction {transactionId} on thread {Thread.CurrentThread.ManagedThreadId}");
            // Simulate work (e.g., processing a transaction) by sleeping for 100 milliseconds.
            Thread.Sleep(100);
            Console.WriteLine($"Completed transaction {transactionId} on thread {Thread.CurrentThread.ManagedThreadId}");
        }

        static void Main(string[] args)
        {
            int numTransactions = 10; // The number of transactions (threads) to simulate.
            List<Thread> threads = new List<Thread>();

            // Create and start threads.
            for (int i = 1; i <= numTransactions; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(ProcessTransaction));
                threads.Add(t);
                t.Start(i); // Pass a unique transaction ID to each thread.
            }

            // Wait for all threads to complete.
            foreach (Thread t in threads)
            {
                t.Join();
            }

            Console.WriteLine("All transactions have been processed.");
        }
    }
}

