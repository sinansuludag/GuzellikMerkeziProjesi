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
    public partial class NotOlustur : Form
    {
        
        int id;

        public NotOlustur(int id)
        {
            InitializeComponent();
            this.id = id;
            richTxtNotlar.Focus();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // Not alanı boş mu kontrol et
            if (string.IsNullOrWhiteSpace(richTxtNotlar.Text))
            {
                MessageBox.Show("Lütfen not alanını doldurunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Bağlantıyı aç
                ConnectionAndStaticTools.OpenConnection();

                // SQL sorgusunu hazırla
                string query = "INSERT INTO dbnotlar (ID, Notlar) VALUES (@ID, @Notlar)";

                using (MySqlCommand mySqlCommand = new MySqlCommand(query, ConnectionAndStaticTools.Connection))
                {
                    // Parametreleri ekle
                    mySqlCommand.Parameters.AddWithValue("@ID", id);
                    mySqlCommand.Parameters.AddWithValue("@Notlar", richTxtNotlar.Text);

                    // Sorguyu çalıştır
                    int rowsAffected = mySqlCommand.ExecuteNonQuery();

                    // Eğer kayıt başarılı olduysa kullanıcıya bilgi ver
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Notunuz başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Not kaydedilemedi. Lütfen tekrar deneyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                // Not alanını temizle
                temizle();

                // Danışan bilgilerini getir ve formu kapat
                DanisanBilgileri danisanBilgileri = new DanisanBilgileri(id);
                ConnectionAndStaticTools.danisanGetir(danisanBilgileri, id);
                this.Hide();
            }
            catch (MySqlException ex)
            {
                // Veritabanı hatasını özel olarak yakala
                MessageBox.Show("NOT OLUSTUR BUTON Veritabanı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Genel hata yakalama
                MessageBox.Show("NOT OLUSTUR BUTON hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Bağlantıyı kapat
                ConnectionAndStaticTools.CloseConnection();
            }
        }


        private void AltForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            // DanisanBilgileri formunu aç
            DanisanBilgileri danisanBilgileri = new DanisanBilgileri(id);
            ConnectionAndStaticTools.danisanGetir(danisanBilgileri, id);
            this.Hide(); // Alt formu gizle
        }

        private void NotOlustur_Load(object sender, EventArgs e)
        {
            txtID.Text=id.ToString();
        }

        private void temizle()
        {
            txtID.Text = "";
            richTxtNotlar.Text = "";
        }

       
    }
}
