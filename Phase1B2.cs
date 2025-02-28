using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace ProjectB_IPC_NamedPipes
{
    class Program
    {
        // Entry point: choose mode based on the command-line argument.
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: dotnet run -- <server|client>");
                return;
            }

            if (args[0].Equals("server", StringComparison.OrdinalIgnoreCase))
            {
                await RunServer();
            }
            else if (args[0].Equals("client", StringComparison.OrdinalIgnoreCase))
            {
                await RunClient();
            }
            else
            {
                Console.WriteLine("Unknown argument. Use 'server' or 'client'.");
            }
        }

        /// <summary>
        /// The server creates a named pipe, waits for a client to connect,
        /// reads a message, processes it (converts to uppercase), and then
        /// sends a response back to the client.
        /// </summary>
        static async Task RunServer()
        {
            // "mypipe" is the name of the pipe; the same name is used by the client.
            using (var pipeServer = new NamedPipeServerStream("mypipe", PipeDirection.InOut, 
                                                               1, PipeTransmissionMode.Byte, 
                                                               PipeOptions.Asynchronous))
            {
                Console.WriteLine("Server: Waiting for client connection...");
                await pipeServer.WaitForConnectionAsync();
                Console.WriteLine("Server: Client connected.");

                // Use StreamReader and StreamWriter for convenient text I/O.
                using (var reader = new StreamReader(pipeServer))
                using (var writer = new StreamWriter(pipeServer) { AutoFlush = true })
                {
                    // Read a line sent by the client.
                    string clientMessage = await reader.ReadLineAsync();
                    Console.WriteLine("Server: Received from client: " + clientMessage);

                    // Process the message (convert to uppercase in this example).
                    string response = clientMessage.ToUpper();

                    // Send the processed message back to the client.
                    await writer.WriteLineAsync("Server processed: " + response);
                    Console.WriteLine("Server: Sent response to client.");
                }
            }
        }

        /// <summary>
        /// The client connects to the named pipe server, sends a message,
        /// and waits for the response.
        /// </summary>
        static async Task RunClient()
        {
            // Connect to the pipe on the local machine (".") using the same pipe name "mypipe".
            using (var pipeClient = new NamedPipeClientStream(".", "mypipe", PipeDirection.InOut, PipeOptions.Asynchronous))
            {
                Console.WriteLine("Client: Connecting to server...");
                await pipeClient.ConnectAsync();
                Console.WriteLine("Client: Connected to server.");

                using (var reader = new StreamReader(pipeClient))
                using (var writer = new StreamWriter(pipeClient) { AutoFlush = true })
                {
                    // Prepare a message to send.
                    string message = "Hello from client on Linux!";
                    await writer.WriteLineAsync(message);
                    Console.WriteLine("Client: Sent message to server: " + message);

                    // Read the response from the server.
                    string response = await reader.ReadLineAsync();
                    Console.WriteLine("Client: Received response: " + response);
                }
            }
        }
    }
}

