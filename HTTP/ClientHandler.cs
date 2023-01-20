using System;
using System.Net.Sockets;
using System.Threading;

namespace MonsterCardGame.HTTP {
	public class ClientHandler {
		public Socket ClientSocket { get; }
		private readonly Thread _thread;
		public delegate void Func(ClientHandler client);

        public ClientHandler(Socket socket, Func func) {
            this.ClientSocket = socket;
            this._thread = new(() => { func(this); });
			this.Start();
        }

		private void Start() {
			this._thread.Start();
		}

		public void Close() {
			this.ClientSocket.Close();
		}

    }
}

