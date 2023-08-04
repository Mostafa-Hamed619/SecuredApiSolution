using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecuredApiTest.Models
{
    public class Jwt
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public double DurationInDays { get; set; }
        public string Audience { get; set; }
    }
}
