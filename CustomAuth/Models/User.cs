using System;
using System.Collections.Generic;
using System.Text;

namespace PawnShop.Oracle.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public bool Confirmed { get; set; }
        public string Password { get; set; }
    }
}
