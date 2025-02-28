using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ProjectA_Phase1_Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            int numTransactions = 50; // Increase for stress/concurrency testing
            var threads = new List<Thread>();

            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 1; i <= numTransactions; i++)
            {
                Thread t = new Thread((object transactionId) =>
                {
                    Console.WriteLine($"Thread {transactionId} started.");
                    Thread.Sleep(100); // Simulate work
                    Console.WriteLine($"Thread {transactionId} finished.");
                });
                threads.Add(t);
                t.Start(i);
            }

            // Wait for all threads
            foreach (Thread t in threads)
                t.Join();

            sw.Stop();
            Console.WriteLine($"All {numTransactions} threads completed in {sw.ElapsedMilliseconds} ms");
        }
    }
}

