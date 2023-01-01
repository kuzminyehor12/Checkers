using Checkers.Server.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Checkers.Extra.DataManagement
{
    public interface ISessionService
    {
        Session GetSession();
        void CreateSession(Session model);
        void UpdateSession(Session model);
        void RemoveSession();
    }
}
