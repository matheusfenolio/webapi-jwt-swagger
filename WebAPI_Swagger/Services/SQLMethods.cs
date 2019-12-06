using System;
using System.Data;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace WebAPI_Swagger //Change to your namespace project
{
    class SQLMethods
    {
        public static string Host { get; set; }
        public static string Port { get; set; }
        public static string Instance { get; set; }
        public static string User { get; set; }
        public static string Password { get; set; }
        public static string Timeout { get; set; }
        public static string Type { get; set; }
        public static string ConnectionString { get; set; }

        public static void SetParameters()
        {
            try
            {
                string path = String.Format(@"{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "config.xml");
                XElement xml = XElement.Load(path.Replace(@"\", "\\"));


                Host = xml.Element("host").Value;
                Port = xml.Element("port").Value;
                Instance = xml.Element("instance").Value;
                User = xml.Element("user").Value;
                Password = xml.Element("password").Value;
                Timeout = xml.Element("timeout").Value;
                Type = xml.Element("type").Value;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public static string GetConnectionString()
        {
            try
            {
                SetParameters();

                if (Type == "local")
                {
                    return String.Format(@"Data Source={0};Initial Catalog={1};User ID={2}; Password={3};", Host, Instance, User, Password);
                }

                if (Type == "sqlExtern")
                {
                    return String.Format("Data Source={0},{1};Initial Catalog={2};User id={3};Password={4};Connection Timeout={5};", Host, Port, Instance, User, Password, Timeout);
                }

                if(Type == "Existent")
                {
                    if(String.IsNullOrWhiteSpace(ConnectionString))
                    {
                        throw new Exception("No ConnectiongString entered to make connection.");
                    }
                    else
                    {
                        return ConnectionString;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public static void ExecQuery(string sql)
        {
            SqlConnection con = new SqlConnection();
            SqlCommand cmd;

            con.ConnectionString = GetConnectionString();
            cmd = new SqlCommand(sql, con);

            int i = 0;

            try
            {
                con.Open();
                i = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new System.Exception(ex.Message, ex);
            }
            finally
            {
                con.Close();
            }
        }

        public static Object ExecScalar(string sql)
        {
            SqlConnection con = new SqlConnection();
            SqlCommand cmd;

            con.ConnectionString = GetConnectionString();
            cmd = new SqlCommand(sql, con);

            try
            {
                con.Open();
                object i = cmd.ExecuteScalar();
                return i;
            }
            catch (Exception ex)
            {
                throw new System.Exception(ex.Message, ex);
            }
            finally
            {
                con.Close();
            }
        }

        public static void ExecMultiple(List<String> sql)
        {
            try
            {
                SqlConnection con = new SqlConnection();
                SqlCommand cmd = new SqlCommand();
                SqlTransaction transaction;

                con.ConnectionString = GetConnectionString();

                con.Open();

                transaction = con.BeginTransaction("SQLMethods");

                try
                {


                    cmd.Connection = con;
                    cmd.Transaction = transaction;

                    foreach (String query in sql)
                    {
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    try
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message, ex);
                    }
                    catch (Exception ex2)
                    {
                        throw new Exception(ex2.Message,ex2);
                    }
                }
                finally
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            
        }

        public static object GetObject(String sql)
        {
            Object retorno = null;

            SqlConnection con = new SqlConnection();
            SqlCommand cmd;

            con.ConnectionString = GetConnectionString();

            try
            {
                cmd = new SqlCommand(sql, con);
                con.Open();
                retorno = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw new System.Exception(ex.Message, ex);
            }
            finally
            {
                con.Close();
            }

            return retorno;
        }

        public static string GetField(String sql, String field)
        {
            string retorno = string.Empty;

            SqlConnection con = new SqlConnection();
            SqlCommand cmd;

            con.ConnectionString = GetConnectionString();

            try
            {
                cmd = new SqlCommand(sql, con);
                SqlDataReader dr;
                con.Open();
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    retorno = Convert.ToString(String.Format("{0}", dr[field]));
                }
            }
            catch (Exception ex)
            {
                throw new System.Exception(ex.Message, ex);
            }
            finally
            {
                con.Close();
            }

            return retorno;
        }

        public static DataTable GetDT(String sql)
        {
            DataTable dt = new DataTable();

            SqlConnection con = new SqlConnection();
            SqlCommand cmd;

            con.ConnectionString = GetConnectionString();

            try
            {
                cmd = new SqlCommand(sql, con);
                con.Open();
                dt.Load(cmd.ExecuteReader());
                return dt;
            }
            catch (Exception ex)
            {
                throw new System.Exception(ex.Message, ex);
            }
            finally
            {
                con.Close();
            }
        }
    }
}
