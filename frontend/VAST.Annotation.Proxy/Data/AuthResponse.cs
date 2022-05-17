using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAST.Annotation.Proxy.Data
{

    public class AuthenticationResponse
    {
        public string refresh { get; set; }
        public string access { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public AuthenticationData data { get; set; }
    }

    public class AuthenticationData
    {
        public JwtToken jwtToken { get; set; }
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public object permissions { get; set; }
        public DateTime last_login { get; set; }
    }

    public class JwtToken
    {
        public string refresh { get; set; }
        public string access { get; set; }
    }
}
