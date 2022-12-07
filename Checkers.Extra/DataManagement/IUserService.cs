using Checkers.Server.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Checkers.Server.DataManagement
{
    public interface IUserService
    {
        IEnumerable<User> GetUsers();
        void CreateUser(User model);
        void UpdateUser(User model);
    }
}
