using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI_Swagger.Models
{
    public class User
    {
        public string name { get; set; }
        public string login { get; set; }
        public string password { get; set; }

        public string token { get; set; }
        
    }
}
