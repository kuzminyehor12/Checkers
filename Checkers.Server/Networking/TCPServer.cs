using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Checkers.Server.Networking
{
    public class TCPServer : IServer
    {
        private static TCPServer _instance;
        private static readonly object _lock = new object();

        public TcpListener Listener { get; set; }
        public TcpClient Client { get; set; }
        public static TCPServer Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance is null)
                    {
                        _instance = new TCPServer();
                    }

                    return _instance;
                }
            }
        }

        public void Start(string ip, string port)
        {
            try
            {
                Listener = new TcpListener(IPAddress.Any, int.Parse(port));
                Listener.Start();

                Listener.BeginAcceptTcpClient(AcceptClientCallback, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void AcceptClientCallback(IAsyncResult asyncResult)
        {
            Client = Listener.EndAcceptTcpClient(asyncResult);
        }
    }
}
