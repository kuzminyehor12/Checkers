using System;
using System.Collections.Generic;
using System.Text;

namespace Checkers.Client.Networking
{
    public interface IClient
    {
        void Connect(string ip, string port);
        void ConnectCallback(IAsyncResult asyncResult);
    }
}
