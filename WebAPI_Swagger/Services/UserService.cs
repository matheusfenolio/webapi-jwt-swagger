using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebAPI_Swagger.Models;

namespace WebAPI_Swagger.Services
{
    public class UserService : IUserService
    {
        private User _logged;

        public UserService()
        {
            _logged = new User();
        }

        public bool AddUser(User newUser)
        {
            try
            {
                string sql = String.Format(@"insert into LOGIN values ('{0}', '{1}', '{2}')", newUser.name, newUser.login, newUser.password);

                SQLMethods.ExecQuery(sql);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateUser(User newUser)
        {
            try
            {
                string sql = String.Format(@"update LOGIN
                                                    set NOME = '{0}',
                                                        USUARIO = '{1}',
                                                        SENHA = '{2}'
                                                    where USUARIO = '{3}'
                                                      and SENHA = '{4}'", newUser.name, newUser.login, newUser.password, _logged.login, _logged.password);

                SQLMethods.ExecQuery(sql);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemoveUser(Login deleteUser)
        {
            try
            {
                string sql = String.Format(@"delete from LOGIN where USUARIO = '{0}' and SENHA = '{1}'", deleteUser.login, deleteUser.password);

                SQLMethods.ExecQuery(sql);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Login(Login login)
        {
            string sql = String.Format(@"select NOME as 'name', USUARIO as 'login', SENHA as 'password' from LOGIN where USUARIO = '{0}' and SENHA = '{1}'", login.login, login.password);

            DataTable dt = SQLMethods.GetDT(sql);

            if(dt.Rows.Count == 1)
            {
                User usr = new User();
                usr.name = (String)dt.Rows[0]["name"];
                usr.login = (String)dt.Rows[0]["login"];
                usr.password = (String)dt.Rows[0]["password"];

                if (usr == null)
                {
                    return false;
                }

                ChangeLogged(usr);

                return true;
            }
            else
            {
                return false;
            }
        }

        public User ChangeLogged(User newUser)
        {
            _logged = newUser;

            return GetLogged();
        }

        public User GetLogged()
        {
            return _logged;
        }
    }
}
