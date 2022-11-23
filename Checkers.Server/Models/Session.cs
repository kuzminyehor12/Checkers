using System;
using System.Collections.Generic;
using System.Text;

namespace Checkers.Server.Models
{
    public class Session
    {
        public string FirstNickname { get; set; }
        public int FirstPlayerSessionVictoriesCount { get; set; }
        public string SecondNickname { get; set; }
        public int SecondPlayerSessionVictoriesCount { get; set; }
    }
}
