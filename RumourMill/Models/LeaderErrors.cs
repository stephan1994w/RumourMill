using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RumourMill.Models
{
    public class LeaderErrors
    {
        public int LeaderId { get; set; }
        public string LeaderName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
        public string Role { get; set; }
        public Nullable<System.DateTime> LastAccess { get; set; }
        public string Email { get; set; }
        public string ErrorMessage { get; set; }
    }
}