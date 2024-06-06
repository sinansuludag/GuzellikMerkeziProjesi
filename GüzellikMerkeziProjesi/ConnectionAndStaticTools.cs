using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace GüzellikMerkeziProjesi
{
    public class ConnectionAndStaticTools
    {
        static MySqlConnection connection=new MySqlConnection(ConfigurationManager.ConnectionStrings["Baglanti"].ConnectionString);
        public static MySqlConnection Connection
        {
            get { return connection; }
        }

        public static void OpenConnection()
        {
            if (ConnectionAndStaticTools.Connection.State == System.Data.ConnectionState.Closed)
            {
                ConnectionAndStaticTools.Connection.Open();
            }
        }

        public static void CloseConnection()
        {
            if (ConnectionAndStaticTools.Connection.State == System.Data.ConnectionState.Open)
            {
                ConnectionAndStaticTools.Connection.Close();
            }
        }

        public static void danisanGetir(DanisanBilgileri danisanBilgileri,int id)
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
