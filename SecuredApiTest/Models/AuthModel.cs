using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecuredApiTest.Models
{
    public class AuthModel
    {
        public string Message { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool IsAuthenticated { get; set; }

        public List<string> Role { get; set; }

        public string Token { get; set; }

        public DateTime ExpiredOn { get; set; }
    }
}
