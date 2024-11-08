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
            if (txtUrunFiyati.Text == "")
            {
                MessageBox.Show("Lütfen geçerli bir fiyat giriniz.");
                return;
            }

            // KacinciGelis değerini al
            int kacinciGelis = GetNextKacinciGelis();

            // Ürün bilgilerini veritabanına ekleme işlemi
            OpenConnection();
            MySqlCommand mySqlCommand = new MySqlCommand("INSERT INTO dbpaketbilgisi (ID, Tarih, Aciklama, Durum, Tutar, KacinciGelis) VALUES (@ID, @Tarih, @Aciklama, @Durum, @Tutar, @KacinciGelis)", ConnectionAndStaticTools.Connection);
            mySqlCommand.Parameters.AddWithValue("@ID", id);
            mySqlCommand.Parameters.AddWithValue("@Tarih", tarihDataTimePic.Value);
            mySqlCommand.Parameters.AddWithValue("@Aciklama", txtUrunAdi.Text);
            mySqlCommand.Parameters.AddWithValue("@Durum", "");
            mySqlCommand.Parameters.AddWithValue("@Tutar", float.Parse(txtUrunFiyati.Text));
            mySqlCommand.Parameters.AddWithValue("@KacinciGelis", kacinciGelis);
            mySqlCommand.ExecuteNonQuery();
            CloseConnection();

            // İşlem tamamlandıktan sonra
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
                    PaketBilgisi paketBilgisi;

                        while (reader.Read())
                        {   string islem = reader["İslem"].ToString();
                            string[] islemDizi=islem.Split(':');
                            string[] diziVirgulleAyir;
                            string ayrilanDizi;
                            foreach (string s in islemDizi)
                            {
                               diziVirgulleAyir = s.Split(',');
                             ayrilanDizi = string.Join(",", diziVirgulleAyir);
                            diziBirlestirGuncelle += ":" + ayrilanDizi;
                            }
                            if ( float.TryParse(txtUrunFiyati.Text, out float ucret))
                            {
                                paketBilgisi = new PaketBilgisi("", 0, ucret);
                                dizi.Add(paketBilgisi.ToString());
                                string birlesikString = string.Join(":", dizi);
                                diziBirlestirGuncelle += ":" + birlesikString;
                             CloseConnection();
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
                // Hata işleme burada yapılır
                Console.WriteLine("Hata: " + ex.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }



        public void toplamTutarGuncelle(string islem)
        {
            try
            {
                OpenConnection();
                MySqlCommand mySqlCommand = new MySqlCommand("Update dbdanisankayit set İslem=@İslem where DanisanID=@ID", ConnectionAndStaticTools.Connection);
                mySqlCommand.Parameters.AddWithValue("@ID", id);
                mySqlCommand.Parameters.AddWithValue("@İslem", islem);
                mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata:" + ex.Message);
            }
            finally
            {
                CloseConnection();
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
