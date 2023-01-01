using System;
using System.Collections.Generic;
using System.Text;

namespace PawnShop.Forms.Validation
{
    public class FormValidationException : Exception
    {
        public FormValidationException() : base()
        {
            
        }

        public FormValidationException(string msg) : base(msg)
        {
            
        }

        public FormValidationException(string msg, Exception innerException) : base(msg, innerException)
        {
            
        }
    }
}
