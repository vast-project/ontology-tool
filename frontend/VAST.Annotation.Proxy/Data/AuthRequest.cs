using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAST.Annotation.Proxy.Data
{
    public class AuthRequest
    {
        public string email { get; set; }
        public string password { get; set; }
        public bool remember_me { get; set; }
    }

}
