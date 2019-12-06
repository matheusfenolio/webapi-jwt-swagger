using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI_Swagger.Models;

namespace WebAPI_Swagger.Services
{
    public interface IUserService
    {
        bool AddUser(User user);
        bool RemoveUser(Login login);
        bool UpdateUser(User login);
        bool Login(Login login);
        User ChangeLogged(User user);
        User GetLogged();
    }
}
