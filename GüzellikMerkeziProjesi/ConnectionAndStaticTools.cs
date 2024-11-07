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


        // Bağlantıyı açan metod
        public static void OpenConnection()
        {
            try
            {
                if (Connection.State == System.Data.ConnectionState.Closed)
                {
                    Connection.Open();
                }
            }
            catch (MySqlException ex)
            {
                // Veritabanı bağlantısı ile ilgili hata
                Console.WriteLine($"VERİTABANI BAGLANTİSİ ACİLMADİ: {ex.Message}");
                throw;  // Hata yönetimini üst katmana iletmek için
            }
        }

        // Bağlantıyı kapatan metod
        public static void CloseConnection()
        {
            try
            {
                if (Connection.State == System.Data.ConnectionState.Open)
                {
                    Connection.Close();
                }
            }
            catch (MySqlException ex)
            {
                // Bağlantı kapama hatası
                Console.WriteLine($"VERİTABANI BAGLANTİSİ KAPATİLMADİ: {ex.Message}");
                throw;  // Hata yönetimini üst katmana iletmek için
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
