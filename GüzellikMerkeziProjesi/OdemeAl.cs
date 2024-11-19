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
    public partial class OdemeAl : Form
    {

        int id;

        public OdemeAl(int id)
        {
            InitializeComponent();
            this.id = id;
        }

        private void OdemeAl_Load(object sender, EventArgs e)
        {
            txtID.Text=id.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Tutar ve Ödeme tipi boş mu kontrol et
            if (string.IsNullOrWhiteSpace(txtTutar.Text) || string.IsNullOrWhiteSpace(cbOdTip.Text))
            {
                MessageBox.Show("Lütfen Ödeme tipi ve Tutar alanını doldurunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Bağlantıyı aç
                ConnectionAndStaticTools.OpenConnection();

                // SQL sorgusunu hazırla
                string query = "INSERT INTO dbalinanodemeler (ID, Tarih, Aciklama, OdemeTipi, Tutar) " +
                               "VALUES (@ID, @Tarih, @Aciklama, @OdemeTipi, @Tutar)";

                using (MySqlCommand mySqlCommand = new MySqlCommand(query, ConnectionAndStaticTools.Connection))
                {
                    // Parametreleri ekle
                    mySqlCommand.Parameters.AddWithValue("@ID", id);
                    mySqlCommand.Parameters.AddWithValue("@Tarih", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                    mySqlCommand.Parameters.AddWithValue("@Aciklama", richTxtAciklama.Text);
                    mySqlCommand.Parameters.AddWithValue("@OdemeTipi", cbOdTip.Text);

                    // Tutarı güvenli şekilde parse et
                    if (float.TryParse(txtTutar.Text, out float tutar))
                    {
                        mySqlCommand.Parameters.AddWithValue("@Tutar", tutar);
                    }
                    else
                    {
                        MessageBox.Show("Geçersiz tutar girişi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Sorguyu çalıştır
                    int rowsAffected = mySqlCommand.ExecuteNonQuery();

                    // Eğer kayıt başarılı olduysa kullanıcıya bilgi ver
                    if (rowsAffected > 0)
                    {
                        // Başarıyla ödeme kaydedildi
                        raporaEkle();
                        temizle();
                        MessageBox.Show("Ödeme başarıyla alınmıştır.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Danışan bilgilerini getir ve formu kapat
                        DanisanBilgileri danisanBilgileri = new DanisanBilgileri(id);
                        ConnectionAndStaticTools.danisanGetir(danisanBilgileri, id);
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Ödeme kaydedilemedi. Lütfen tekrar deneyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Veritabanı hatasını özel olarak yakala
                MessageBox.Show("ODEME AL BUTON Veritabanı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Genel hata yakalama
                MessageBox.Show("ODEME AL BUTON hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Bağlantıyı kapat
                ConnectionAndStaticTools.CloseConnection();
            }
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

        private void raporaEkle()
        {
            try
            {
                // Danışanın isim ve soyadını al
                string[] isimSoyad = danisanIsimSoyadGetir();

                // Bağlantıyı aç
                ConnectionAndStaticTools.OpenConnection();

                // SQL komutunu oluştur
                string query = "INSERT INTO dbrapor (ID, `Adi Soyadi`, Tarih, OdemeTipi, Tutar) " +
                               "VALUES (@ID, @AdiSoyadi, @Tarih, @OdemeTipi, @Tutar)";

                using (MySqlCommand cmd = new MySqlCommand(query, ConnectionAndStaticTools.Connection))
                {
                    // Parametreleri ekle
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@AdiSoyadi", isimSoyad[0]);
                    cmd.Parameters.AddWithValue("@Tarih", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@OdemeTipi", cbOdTip.Text);

                    // Tutarı güvenli şekilde parse et
                    if (float.TryParse(txtTutar.Text, out float tutar))
                    {
                        cmd.Parameters.AddWithValue("@Tutar", tutar);
                    }
                    else
                    {
                        MessageBox.Show("Geçersiz tutar girişi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // SQL komutunu çalıştır
                     cmd.ExecuteNonQuery();

                }
            }
            catch (MySqlException ex)
            {
                // Veritabanı hatası yakalama
                MessageBox.Show("ODEME AL RAPORA EKLE Veritabanı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Genel hata yakalama
                MessageBox.Show("ODEME AL RAPORA EKLE hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Bağlantıyı kapat
                ConnectionAndStaticTools.CloseConnection();
            }
        }


        private string[] danisanIsimSoyadGetir()
        {
            List<string> isimSoyadListesi = new List<string>();

            try
            {
                // Veritabanı bağlantısını aç
                ConnectionAndStaticTools.OpenConnection();

                // SQL sorgusunu hazırla
                MySqlCommand cmd = new MySqlCommand("SELECT Adi, Soyadi FROM dbdanisankayit WHERE DanisanID = @ID", ConnectionAndStaticTools.Connection);
                cmd.Parameters.AddWithValue("@ID", id);

                // Veritabanı okuma işlemini using bloğunda yönet
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    // Okunan her bir satır için işlemler
                    while (reader.Read())
                    {
                        string adi = reader["Adi"].ToString();
                        string soyadi = reader["Soyadi"].ToString();
                        string isimSoyad = adi + " " + soyadi;
                        isimSoyadListesi.Add(isimSoyad);
                    }
                }
            }
            catch (MySqlException ex)
            {
                // MySQL hata yönetimi
                MessageBox.Show("ODEME ALMA DANISAN ISIM SOYAD GETIR Veritabanı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Genel hata yönetimi
                MessageBox.Show("ODEME ALMA DANISAN ISIM SOYAD GETIR Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Bağlantıyı kapat
                ConnectionAndStaticTools.CloseConnection();
            }

            return isimSoyadListesi.ToArray();
        }



        private void temizle()
        {
            txtID.Text = "";
            richTxtAciklama.Text = "";
            cbOdTip.Text = "";
            txtTutar.Text = "";
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
