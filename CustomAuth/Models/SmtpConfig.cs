using System;
using System.Collections.Generic;
using System.Text;

namespace CustomAuth.Models
{
    public class SmtpConfig
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public bool SslEnabled { get; set; }
    }
}
