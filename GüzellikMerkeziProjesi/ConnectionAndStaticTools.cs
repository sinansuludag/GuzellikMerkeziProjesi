using System;
using System.Configuration;
using System.Data;
using System.IO;
using MySql.Data.MySqlClient;

namespace GüzellikMerkeziProjesi
{
    public static class ConnectionAndStaticTools
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["Baglanti"].ConnectionString;

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public static void ExecuteWithConnection(Action<MySqlConnection> action)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    action(conn);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }

        //public static T ExecuteWithConnection<T>(Func<MySqlConnection, T> func)
        //{
        //    try
        //    {
        //        using (MySqlConnection conn = GetConnection())
        //        {
        //            conn.Open();
        //            return func(conn);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogError(ex);
        //        throw;
        //    }
        //}

        private static void LogError(Exception ex)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
            using (StreamWriter writer = new StreamWriter(logPath, true))
            {
                writer.WriteLine("-------- " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " --------");
                writer.WriteLine(ex.ToString());
                writer.WriteLine();
            }
        }

        public static void danisanGetir(DanisanBilgileri danisanBilgileri, int id)
        {
            danisanBilgileri.DanisanBilgileriniGoster(id);
            danisanBilgileri.seansBilgileriniGetir(id);
            danisanBilgileri.notlariListele(id);
            danisanBilgileri.taninanPaketListele(id);
            danisanBilgileri.notBilgisiniGetir(id);
            danisanBilgileri.alinanOdemeBilgisiniGetir(id);
            danisanBilgileri.Show();
        }
        
    }
}
