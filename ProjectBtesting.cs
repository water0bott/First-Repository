using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace ProjectB_IPC_Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            // Adjust the path to point to your Project B executable.
            string ipcExePath = GetIpcExecutablePath();

            // 1. Data Integrity Test
            TestDataIntegrity(ipcExePath);

            // 2. Error Handling Test
            TestErrorHandling(ipcExePath);

            // 3. Performance Test
            TestPerformance(ipcExePath);

            Console.WriteLine("All tests complete.");
        }

        /// <summary>
        /// Example 1: Data Integrity Testing
        /// Ensures that data passed through the pipe remains intact (or is correctly transformed).
        /// </summary>
        static void TestDataIntegrity(string exePath)
        {
            Console.WriteLine("=== Data Integrity Test ===");

            // Start the server
            Process server = StartProcess(exePath, "server");
            Thread.Sleep(500); // Give the server time to initialize

            // Start the client with test data. For example, we can simulate a JSON payload.
            // This depends on how your client is coded to accept data (arguments, standard input, etc.).
            Process client = StartProcess(exePath, "client");
            
            // Read client output
            string clientOutput = client.StandardOutput.ReadToEnd();
            client.WaitForExit();

            // Optional: read server output if needed
            // string serverOutput = server.StandardOutput.ReadToEnd();

            // Terminate the server if it’s still running
            if (!server.HasExited)
            {
                server.Kill();
            }

            // Validate the output
            // For instance, if the server is supposed to echo or uppercase the data,
            // check that the output matches the expectation.
            if (clientOutput.Contains("Server processed:"))
            {
                Console.WriteLine("Data Integrity Test PASSED. Output was as expected.");
            }
            else
            {
                Console.WriteLine("Data Integrity Test FAILED. Unexpected output:");
                Console.WriteLine(clientOutput);
            }
        }

        /// <summary>
        /// Example 2: Error Handling Validation
        /// Forces an error scenario to see how gracefully the IPC code handles it.
        /// </summary>
        static void TestErrorHandling(string exePath)
        {
            Console.WriteLine("=== Error Handling Test ===");

            // Start the server
            Process server = StartProcess(exePath, "server");
            Thread.Sleep(500);

            // Start the client
            Process client = StartProcess(exePath, "client");

            // Immediately kill the server to simulate a broken pipe scenario
            Thread.Sleep(300); // Let client attempt connection
            if (!server.HasExited)
            {
                server.Kill();
                Console.WriteLine("Server forcibly terminated to simulate error scenario.");
            }

            // Read client output (the client may crash, throw, or handle the error gracefully)
            string clientOutput = client.StandardOutput.ReadToEnd();
            client.WaitForExit();

            // Check how the client responded
            if (clientOutput.Contains("Error") || clientOutput.Contains("Exception") || client.ExitCode != 0)
            {
                Console.WriteLine("Error Handling Test PASSED. Client reported an error or non-zero exit code.");
            }
            else
            {
                Console.WriteLine("Error Handling Test: Check logs or code to confirm graceful handling.");
                Console.WriteLine("Client output:\n" + clientOutput);
            }
        }

        /// <summary>
        /// Example 3: Performance Benchmarking
        /// Measures throughput/latency by sending large data from client to server.
        /// </summary>
        static void TestPerformance(string exePath)
        {
            Console.WriteLine("=== Performance Test ===");

            // Start the server
            Process server = StartProcess(exePath, "server");
            Thread.Sleep(500);

            // Generate a large payload (e.g., repeated lines of text).
            // The server might read/echo or process this data. Adjust to your scenario.
            StringBuilder largePayload = new StringBuilder();
            for (int i = 0; i < 10000; i++)
            {
                largePayload.AppendLine($"Line {i} - Some sample data for performance testing.");
            }
            string payload = largePayload.ToString();

            // Start the client in a way that it reads data from standard input, if your code supports that.
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = "client",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            Process client = Process.Start(psi);

            // Send the large payload to client
            var stopwatch = Stopwatch.StartNew();
            using (var writer = client.StandardInput)
            {
                writer.Write(payload);
            }

            // Read the output
            string output = client.StandardOutput.ReadToEnd();
            client.WaitForExit();
            stopwatch.Stop();

            // Kill server if needed
            if (!server.HasExited)
            {
                server.Kill();
            }

            // Log the time
            Console.WriteLine($"Performance Test: Sent {payload.Length} chars in {stopwatch.ElapsedMilliseconds} ms.");

            // You might check that the server processed all lines, or that output contains certain markers.
            if (output.Contains("processed") || output.Length > 0)
            {
                Console.WriteLine("Performance Test PASSED. Data was sent and received successfully.");
            }
            else
            {
                Console.WriteLine("Performance Test FAILED. Output was not as expected.");
            }
        }

        /// <summary>
        /// Helper method to start a process with redirected output.
        /// </summary>
        static Process StartProcess(string exePath, string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            return Process.Start(psi);
        }

        /// <summary>
        /// Helper method to locate your IPC executable.
        /// Adjust this path to match your project's build output.
        /// </summary>
        static string GetIpcExecutablePath()
        {
            // Example path if you have a project "ProjectB_IPC_NamedPipes"
            // that builds into bin/Debug/net7.0/ProjectB_IPC_NamedPipes.dll or .exe
            // If you're on Linux and using a DLL with "dotnet", you'd do:
            // return "dotnet " + pathToDll;

            string projectDir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string exePath = Path.Combine(projectDir, "ProjectB_IPC_NamedPipes", "bin", "Debug", "net7.0", "ProjectB_IPC_NamedPipes");
            if (OperatingSystem.IsWindows())
            {
                exePath += ".exe";
            }
            return exePath;
        }
    }
}

