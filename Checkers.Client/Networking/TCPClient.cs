using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Checkers.Client.Networking
{
    public class TCPClient : IClient
    {
        private static TCPClient _instance;
        private static readonly object _lock = new object();
        
        public TcpClient Client { get; set; }
        public static TCPClient Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance is null)
                    {
                        _instance = new TCPClient();
                    }

                    return _instance;
                }
            }
        }

        public TCPClient()
        {
            Client = new TcpClient();
        }

        public void Connect(string ip, string port)
        {
            try
            {
                Client.BeginConnect(IPAddress.Parse(ip), int.Parse(port), ConnectCallback, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                Client.Dispose();
                Client = null;
            }
        }

        public void ConnectCallback(IAsyncResult asyncResult)
        {
            try
            {
                Client.EndConnect(asyncResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
