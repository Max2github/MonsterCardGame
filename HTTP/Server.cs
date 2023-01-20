using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonsterCardGame {
    namespace HTTP {
        // usings
        using System.Net;
        using System.Net.Sockets;

        internal class Server {
            // public attributes
            public int Port { get; private set; } = 80;
            // public uint bufferSize = 2048;

            // private attributes
            private TcpListener _listener;
            private Routing.IRouter _router;

            // constructor(s)
            public Server(Routing.IRouter router) {
                // we need to set it, because C# complains if we don't
                this._listener = new TcpListener(IPAddress.Any, this.Port);

                // Console.Write("Initialising Router... ");
                this._router = router;
                // Console.WriteLine("Done.");
            }

            // public functions
            // start sever
            public void Start(int port = 80) {
                this.Port = port;
                Console.Write("(Re-)Initialising Listener... ");
                this._listener = new TcpListener(IPAddress.Any, this.Port); // should catch exception
                Console.WriteLine("Done.");

                Console.Write("Starting Listener... ");
                _listener.Start(); // should catch exception
                Console.WriteLine("Done.");
                Console.WriteLine($"Server listening on port {this.Port}");
                this.Listen(); // process new Request endlessly, may do this in another thread
            }
            public void Stop() {
                _listener.Stop();
            }

            // private functions
            // receive & send
            private Request Receive(Socket client) {
                var buffer = new List<byte>();

                int bytesReceived = 0;

                while (client.Available == 0) { } // wait while there is nothing to receive

                // while the is data from the client, receive data
                while (client.Available > 0) {
                    byte[] currByte = new byte[1];
                    int received = client.Receive(currByte, currByte.Length, SocketFlags.None);
                    if (received == 1) { buffer.Add(currByte[0]); bytesReceived++; }
                }
                if (bytesReceived <= 0) {
                    Console.WriteLine("!! Error: Nothing Received! Skipping parsing.");
                    // return this.Receive(ref client);
                    return new Request(); // invalid
                }

                byte[] rcData = buffer.ToArray();
                return new Request(ref rcData);

                /*int bytesReceived = 0;
                byte[] rcData = new byte[this.bufferSize];
                bytesReceived = client.Receive(rcData);
                client.Receive(rcData,)
                if (bytesReceived <= 0) {
                    Console.WriteLine("!! Error: Nothing Received! Skipping parsing.");
                    // return this.Receive(ref client);
                    return new Request();
                }

                return new Request(ref rcData);*/
            }
            private void SendBin(ref byte[] data, Socket client) {
                int bytesSent = 0;
                if (client.Connected) {
                    bytesSent = client.Send(data);
                    if (bytesSent == -1) {
                        Console.WriteLine("!! Error: Nothing sent. Trying again.");
                        this.SendBin(ref data, client);
                    }
                } else {
                    Console.WriteLine("This client is not connected to the server!");
                }
            }
            private void Send(Response data, Socket client) {
                byte[] all = data.GetSerialized();
                this.SendBin(ref all, client);
            }

            private void Listen() {
                Console.WriteLine($"WebServer listening and running on port {this.Port}");
                while (true) {
                    Socket clientSocket = _listener.AcceptSocket();
                    ClientHandler client = new(clientSocket, this.Process);
                }
            }

            // execute
            private void Process(ClientHandler client) {
                Request req = this.Receive(client.ClientSocket);
                // if Receive returned an invalid Request
                if (req.Method == "") { client.Close(); return; }

                // Console.WriteLine(req.Method + " - " + req.Path + " - " + req.Query);
                Response res = this._router.HandleRequest(req);
                this.Send(res, client.ClientSocket);

                client.Close();
            }
        }
    }
}
