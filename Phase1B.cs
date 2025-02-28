using System;
using System.Diagnostics;
using System.IO;

namespace ProjectB_IPC
{
    class Program
    {
        static void Main(string[] args)
        {
            // If the program is launched with the argument "child",
            // run the child process code; otherwise, run the parent code.
            if (args.Length > 0 && args[0] == "child")
            {
                ChildProcess();
            }
            else
            {
                ParentProcess();
            }
        }

        /// <summary>
        /// This method contains the parent process logic.
        /// It launches the child process, sends data to it,
        /// and then reads the processed output back.
        /// </summary>
        static void ParentProcess()
        {
            Console.WriteLine("Parent process started. Launching child process...");

            // Get the full path of the current executable.
            string executablePath = Process.GetCurrentProcess().MainModule.FileName;

            // Configure the process start info for the child.
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = executablePath,     // Launch the same executable.
                Arguments = "child",           // Pass an argument to indicate child mode.
                UseShellExecute = false,       // Must be false to redirect streams.
                RedirectStandardInput = true,  // Redirect child's standard input.
                RedirectStandardOutput = true, // Redirect child's standard output.
                CreateNoWindow = true          // Optional: Do not create a new window.
            };

            // Start the child process.
            Process childProcess = new Process { StartInfo = psi };
            childProcess.Start();

            // Write data to the child's standard input.
            using (StreamWriter writer = childProcess.StandardInput)
            {
                writer.WriteLine("Hello from parent process!");
                writer.WriteLine("This is a test of IPC using pipes.");
                // Closing the writer signals to the child that input is complete.
            }

            // Read all the output from the child process.
            string childOutput = childProcess.StandardOutput.ReadToEnd();
            childProcess.WaitForExit();

            Console.WriteLine("Received output from child process:");
            Console.WriteLine(childOutput);
        }

        /// <summary>
        /// This method contains the child process logic.
        /// It reads data from standard input, processes it, and writes the result to standard output.
        /// </summary>
        static void ChildProcess()
        {
            Console.WriteLine("Child process started. Reading input from parent...");

            // Read all the data from the parent's input.
            string inputData = Console.In.ReadToEnd();

            // Process the input: In this example, convert the text to uppercase.
            string processedData = inputData.ToUpper();

            // Write the processed data to standard output.
            Console.WriteLine("Child process processed input:");
            Console.WriteLine(processedData);
        }
    }
}

