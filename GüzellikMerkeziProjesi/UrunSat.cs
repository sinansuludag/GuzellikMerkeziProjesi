using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace GüzellikMerkeziProjesi
{
    public partial class UrunSat : Form
    {
        int id;


        public UrunSat(int id)
        {
            InitializeComponent();
            this.id = id;
        }

        private void UrunSat_Load(object sender, EventArgs e)
        {
            txtID.Text = id.ToString();
        }
        public void OpenConnection()
        {
            if (ConnectionAndStaticTools.Connection.State == System.Data.ConnectionState.Closed)
            {
                ConnectionAndStaticTools.Connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (ConnectionAndStaticTools.Connection.State == System.Data.ConnectionState.Open)
            {
                ConnectionAndStaticTools.Connection.Close();
            }
        }

        private void btnSat_Click(object sender, EventArgs e)
        {
            // Fiyat alanının boş olup olmadığını kontrol et
            if (string.IsNullOrWhiteSpace(txtUrunFiyati.Text))
            {
                MessageBox.Show("Lütfen geçerli bir fiyat giriniz.");
                return;
            }

            // Fiyatı float'a dönüştürmeye çalış
            float tutar;
            if (!float.TryParse(txtUrunFiyati.Text, out tutar))
            {
                MessageBox.Show("Geçersiz fiyat girildi. Lütfen geçerli bir fiyat giriniz.");
                return;
            }

            // KacinciGelis değerini al
            int kacinciGelis = GetNextKacinciGelis();

            try
            {
                // Veritabanı bağlantısını aç
                OpenConnection();

                // Veritabanına ürün bilgilerini ekleme işlemi
                MySqlCommand mySqlCommand = new MySqlCommand("INSERT INTO dbpaketbilgisi (ID, Tarih, Aciklama, Durum, Tutar, KacinciGelis) VALUES (@ID, @Tarih, @Aciklama, @Durum, @Tutar, @KacinciGelis)", ConnectionAndStaticTools.Connection);
                mySqlCommand.Parameters.AddWithValue("@ID", id);
                mySqlCommand.Parameters.AddWithValue("@Tarih", tarihDataTimePic.Value.ToString("yyyy-MM-dd"));  // Tarih formatı ekleniyor
                mySqlCommand.Parameters.AddWithValue("@Aciklama", txtUrunAdi.Text);
                mySqlCommand.Parameters.AddWithValue("@Durum", ""); // Durum boş bırakılıyor
                mySqlCommand.Parameters.AddWithValue("@Tutar", tutar);  // Fiyatı float olarak ekliyoruz
                mySqlCommand.Parameters.AddWithValue("@KacinciGelis", kacinciGelis);  // KacinciGelis değeri ekleniyor
                mySqlCommand.ExecuteNonQuery();
            }
            catch(MySqlException sqlEx)
            {
                MessageBox.Show("SAT BUTON Veritabanı Hatası: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("SAT BUTON Hata: " + ex.Message);
            }
            finally
            {
                // Veritabanı bağlantısını kapat
                CloseConnection();
            }

            // İşlem tamamlandıktan sonra ek işlemler
            oncekiTutarGetir();
            temizle();
            MessageBox.Show("Ürün başarıyla satıldı.");
        }


        private int GetNextKacinciGelis()
        {
            int kacinciGelis = 100;

            try
            {
                // Veritabanından en büyük KacinciGelis değerini al
                OpenConnection();
                MySqlCommand command = new MySqlCommand("SELECT MAX(KacinciGelis) FROM dbpaketbilgisi", ConnectionAndStaticTools.Connection);
                object result = command.ExecuteScalar();
                CloseConnection();

                // Eğer sonuç null değilse ve 100'den küçükse, başlangıç değeri olarak 100 kullan
                if (result != DBNull.Value && result != null)
                {
                    int maxValue = Convert.ToInt32(result);
                    kacinciGelis = maxValue < 100 ? 100 : maxValue + 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("GET NEXT KACINCI GELIS Hata: " + ex.Message);
            }

            return kacinciGelis;
        }


        private void AltForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Form kapanmasını engelle
            e.Cancel = true;

            // DanisanBilgileri formunu aç
            DanisanBilgileri danisanBilgileri = new DanisanBilgileri(id);
            ConnectionAndStaticTools.danisanGetir(danisanBilgileri, id);
            this.Hide(); // Alt formu gizle
        }


        public void oncekiTutarGetir()
        {
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            List<string> dizi = new List<string>();
            string diziBirlestirGuncelle = "";

            try
            {
                connection = ConnectionAndStaticTools.Connection;

                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM dbdanisankayit WHERE DanisanID = @DanisanId", connection))
                {
                    cmd.Parameters.AddWithValue("@DanisanId", id);

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string islem = reader["İslem"].ToString();
                        string[] islemDizi = islem.Split(':');
                        string[] diziVirgulleAyir;
                        string ayrilanDizi = string.Empty;

                        foreach (string s in islemDizi)
                        {
                            diziVirgulleAyir = s.Split(',');
                            ayrilanDizi = string.Join(",", diziVirgulleAyir);
                            diziBirlestirGuncelle += ":" + ayrilanDizi;
                        }

                        // Fiyat kontrolü
                        if (float.TryParse(txtUrunFiyati.Text, out float ucret))
                        {
                            // Paket bilgisi oluşturuluyor
                            PaketBilgisi paketBilgisi = new PaketBilgisi("", 0, ucret);
                            dizi.Add(paketBilgisi.ToString());

                            // Dizi elemanlarını birleştiriyoruz
                            string birlesikString = string.Join(":", dizi);
                            diziBirlestirGuncelle += ":" + birlesikString;

                            // Veritabanı bağlantısını kapatıyoruz
                            CloseConnection();

                            // Toplam tutarı güncelleme
                            toplamTutarGuncelle(diziBirlestirGuncelle);
                        }
                        else
                        {
                            MessageBox.Show("Lütfen geçerli bir sayı giriniz");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda yazdır
                MessageBox.Show("URUN SAT ONCEKI TUTARI GETIR Hata: " + ex.Message);
            }
            finally
            {
                // Veritabanı okuma işlemi bittiğinde, reader'ı kapatıyoruz
                if (reader != null)
                {
                    reader.Close();
                }

                // Bağlantıyı kapatıyoruz
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }




        public void toplamTutarGuncelle(string islem)
        {
            MySqlConnection connection = null;
            MySqlCommand mySqlCommand = null;

            try
            {
                // Veritabanı bağlantısını açıyoruz
                connection = ConnectionAndStaticTools.Connection;
                OpenConnection();

                // Güncelleme sorgusunu hazırlıyoruz
                mySqlCommand = new MySqlCommand("UPDATE dbdanisankayit SET İslem = @İslem WHERE DanisanID = @ID", connection);

                // Parametreleri ekliyoruz
                mySqlCommand.Parameters.AddWithValue("@ID", id);
                mySqlCommand.Parameters.AddWithValue("@İslem", islem);

                // Sorguyu çalıştırıyoruz
                mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Hata durumunda mesaj gösteriyoruz
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                // Bağlantıyı kapatıyoruz
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    CloseConnection();
                }

                // Command nesnesini temizliyoruz
                if (mySqlCommand != null)
                {
                    mySqlCommand.Dispose();
                }
            }
        }


        private void temizle()
        {
            txtID.Text = "";
            txtUrunAdi.Text = "";
            txtUrunFiyati.Text = "";
        }

        private void txtTutar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Enter tuşunu bastırmayı engelle
            }
        }
    }
}
