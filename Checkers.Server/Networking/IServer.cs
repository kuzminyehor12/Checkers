using System;
using System.Collections.Generic;
using System.Text;

namespace Checkers.Server
{
    public interface IServer
    {
        void Start(string ip, string port);
        void AcceptClientCallback(IAsyncResult asyncResult);
    }
}
