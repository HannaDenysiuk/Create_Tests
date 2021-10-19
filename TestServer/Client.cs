using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    public class Client : IDisposable
    {
        public Socket ClientSocket { get; set; }
        public void Dispose()
        {
            if (ClientSocket != null)
                ClientSocket.Close();
        }
    }
}
