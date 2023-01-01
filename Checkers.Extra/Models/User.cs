using System;
using System.Collections.Generic;
using System.Text;

namespace Checkers.Server.Models
{
    public class User
    {
        public string Nickname { get; set; }
        public int VictoriesQuantity { get; set; }
        public int Points { get; set; }
    }
}
